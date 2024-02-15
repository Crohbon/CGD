using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace TarodevController {
    /// <summary>
    /// Hey!
    /// Tarodev here. I built this controller as there was a severe lack of quality & free 2D controllers out there.
    /// Right now it only contains movement and jumping, but it should be pretty easy to expand... I may even do it myself
    /// if there's enough interest. You can play and compete for best times here: https://tarodev.itch.io/
    /// If you hve any questions or would like to brag about your score, come to discord: https://discord.gg/GqeHHnhHpz
    /// </summary>
    public class PlayerController : MonoBehaviour, IPlayerController {
        // Public for external hooks
        public Vector3 Velocity { get; private set; }
        public FrameInput Input { get; private set; }
        
        private float _moveInput;
        private bool _jumpPressed;
        private bool _jumpReleased;
        
        public bool JumpingThisFrame { get; private set; }
        public bool LandingThisFrame { get; private set; }
        public bool HasLanded { get; private set; }
        public Vector3 RawMovement { get; private set; }
        public bool Grounded => _colDown;
        
        public PlayerConfiguration PlayerConfiguration;
        public bool IsHoldingWeapon { get; private set; }
        private Weapon _currentWeapon;
        private int _currentWinPoints;
        
        
        private Vector3 _lastPosition;
        private float _currentHorizontalSpeed, _currentVerticalSpeed;

        // This is horrible, but for some reason colliders are not fully established when update starts...
        private bool _active;

        private void Awake() {
            Invoke(nameof(Activate), 0.5f);
        }

        private void Activate() {
            _active = true;
        }

        #region Events

        private void OnEnable() {
            GameEvents.Instance.weaponIsEmpty.AddListener(HandleWeaponIsEmpty);
            GameEvents.Instance.weaponPickUp.AddListener(HandleWeaponPickUp);
        }

        private void OnDisable() {
            GameEvents.Instance?.weaponIsEmpty.RemoveListener(HandleWeaponIsEmpty);
            GameEvents.Instance?.weaponPickUp.RemoveListener(HandleWeaponPickUp);
        }
        
        private void HandleWeaponIsEmpty(int id) {
            if (id == PlayerConfiguration.PlayerIndex){
                IsHoldingWeapon = false;
            }
        }

        private void HandleWeaponPickUp(int index, Weapon weaponToAttach) {
            if (index != PlayerConfiguration.PlayerIndex) return;
            IsHoldingWeapon = true;
            _currentWeapon = weaponToAttach;
        }

        #endregion
        private void FixedUpdate() {
            if(!_active) return;
            // Calculate velocity
            Velocity = (transform.position - _lastPosition) / Time.deltaTime;
            _lastPosition = transform.position;

            GatherInput();
            RunCollisionChecks();
            

            CalculateWalk(); // Horizontal movement
            CalculateJumpApex(); // Affects fall speed, so calculate before gravity
            CalculateGravity(); // Vertical movement
            CalculateJump(); // Possibly overrides vertical

            MoveCharacter(); // Actually perform the axis movement
        }

        private void OnDestroy() {
            PlayerConfiguration.Input.onActionTriggered -= HandleInput;
        }

        #region Controls
        
        private PlayerControls _playerControls;
        private bool _hasDamageReductionHc = false;
        private bool _hasBulletDropHc = false;
        private bool _hasBiggerHitBoxHc = false;
        private bool _hasInvertedControlsHc = false;
        public void InitializePlayer(PlayerConfiguration playerConfiguration) {
            _playerControls = new PlayerControls();
            PlayerConfiguration = playerConfiguration;
            PlayerConfiguration.Input.onActionTriggered += HandleInput;
            _currentWinPoints = GameManager.Instance.WinPoints[playerConfiguration.PlayerIndex];
            switch (_currentWinPoints/Settings.PointsPerHandicap){
                case 1:
                    _hasDamageReductionHc = true;
                    break;
                case 2:
                    _hasDamageReductionHc = true;
                    _hasBulletDropHc = true;
                    break;
                case 3:
                    _hasDamageReductionHc = true;
                    _hasBulletDropHc = true;
                    _hasBiggerHitBoxHc = true;
                    break;
                case 4:
                    _hasInvertedControlsHc = true;
                    break;
            }
        }

        private void HandleInput(InputAction.CallbackContext context) {
            if (context.action.name == _playerControls.PlayerInput.Move.name) OnMove(context);
            if (context.action.name == _playerControls.PlayerInput.Jump.name) OnJump(context);
            if (context.action.name == _playerControls.PlayerInput.Shoot.name) OnShoot(context);
            if (context.action.name == _playerControls.PlayerInput.Pause.name) OnPause(context);
        }

        private void OnMove(InputAction.CallbackContext context) {
            _moveInput = context.ReadValue<Vector2>().x;
            if (context.canceled){
                _moveInput = 0f;
            }
        }
        
        [Header("Handicap values")]
        private int _shotAmount = 0;
        private int _jumpAmount = 0;
        
        [SerializeField] private float _maxDamageReduction;
        [SerializeField] private float _shotsWithoutDamageReduction;
        [SerializeField] private float _shotsForMaxDamageReduction;
        [SerializeField] private float _minBulletDropRange;
        [SerializeField] private float _maxBulletDropRange;
        [SerializeField] private float _shotsWithoutBulletDrop;
        [SerializeField] private float _shotsForMinBulletDropRange;
        [SerializeField] private float _jumpsWithoutHitBoxIncrease;
        [SerializeField] private float _jumpsForMaxHitBoxIncrease;
        [SerializeField] private float _baseHitBoxSizeMultiplier;
        [SerializeField] private float _maxHitBoxSizeMultiplier;
        [SerializeField] private int _shotsPerControlsInvert;

        private float _sizeMultiplier = 1f;
        
        private void OnJump(InputAction.CallbackContext context) {
            _jumpPressed = context.performed;
            _jumpReleased = context.canceled;

            _jumpAmount++;

            if (!_hasBiggerHitBoxHc || !(_jumpAmount > _jumpsWithoutHitBoxIncrease)) return;
            
            float previousMultiplier = _baseHitBoxSizeMultiplier + ((_maxHitBoxSizeMultiplier - _baseHitBoxSizeMultiplier) * Mathf.Min(1, (_jumpAmount - 1f)/_jumpsForMaxHitBoxIncrease));
            _sizeMultiplier = _baseHitBoxSizeMultiplier + ((_maxHitBoxSizeMultiplier - _baseHitBoxSizeMultiplier) * Mathf.Min(1, _jumpAmount/_jumpsForMaxHitBoxIncrease));
            IncreaseHitBox(previousMultiplier, _sizeMultiplier);
        }

        private void OnShoot(InputAction.CallbackContext context) {
            if (!IsHoldingWeapon) return;
            
            _shotAmount++;
            float damageMultiplier = 1f;
            if (_hasDamageReductionHc && _shotAmount > _shotsWithoutDamageReduction){
                damageMultiplier = 1 - (_maxDamageReduction * Mathf.Min(1,_shotAmount / _shotsForMaxDamageReduction));
            }

            float bulletDropRange = _maxBulletDropRange;
            if (_hasBulletDropHc && _shotAmount > _shotsWithoutBulletDrop){
                bulletDropRange = _minBulletDropRange + ((_maxBulletDropRange - _minBulletDropRange) * (1 - Mathf.Min(1,_shotAmount/_shotsForMinBulletDropRange)));
            }

            if (_hasInvertedControlsHc && _shotAmount % _shotsPerControlsInvert == 0){
                _hasInvertedMoveInput = !_hasInvertedMoveInput;
            }
            
            _currentWeapon.ShootWeapon(damageMultiplier, bulletDropRange);
        }

        private void OnPause(InputAction.CallbackContext context) {
            GameEvents.Instance.OnPauseGame(true);
        }

        #endregion

        #region Gather Input

        private bool _hasInvertedMoveInput = false;
        
        private void GatherInput() {
            if (_hasInvertedMoveInput){
                Input = new FrameInput {
                    JumpDown = _jumpPressed,
                    JumpUp = _jumpReleased,
                    X = -_moveInput
                };    
            }
            else{
                Input = new FrameInput {
                    JumpDown = _jumpPressed,
                    JumpUp = _jumpReleased,
                    X = _moveInput
                };
            }
            
            if (Input.JumpDown) {
                _lastJumpPressed = Time.time;
            }
        }

        #endregion

        #region Collisions

        [Header("COLLISION")] [SerializeField] private Bounds _characterBounds;
        [SerializeField] private LayerMask _groundLayer;
        [SerializeField] private int _detectorCount = 3;
        [SerializeField] private float _detectionRayLength = 0.1f;
        [SerializeField] [Range(0.1f, 0.3f)] private float _rayBuffer = 0.1f; // Prevents side detectors hitting the ground

        private RayRange _raysUp, _raysUpLeft, _raysUpRight, _raysRight, _raysDown, _raysDownLeft, _raysDownRight, _raysLeft;
        private bool _colUp, _colUpLeft, _colUpRight, _colRight, _colDown, _colDownLeft, _colDownRight, _colLeft;

        private float _timeLeftGrounded;

        // We use these raycast checks for pre-collision information
        private void RunCollisionChecks() {
            // Generate ray ranges. 
            CalculateRayRanged();

            // Ground
            LandingThisFrame = false;
            bool groundedCheck = RunDetection(_raysDown);
            switch (_colDown){
                case true when !groundedCheck:
                    _timeLeftGrounded = Time.time; // Only trigger when first leaving
                    break;
                case false when groundedCheck:
                    _coyoteUsable = true; // Only trigger when first touching
                    LandingThisFrame = true;
                    break;
            }

            if (LandingThisFrame){
                if (!HasLanded){
                    HasLanded = true;
                    GameEvents.Instance.OnPlayerLanded();
                }
            }
            else{
                HasLanded = false;
            }

            _colDown = groundedCheck;

            // The rest
            _colDownLeft = RunDetection(_raysDownLeft);
            _colDownRight = RunDetection(_raysDownRight);
            _colUp = RunDetection(_raysUp);
            _colUpLeft = RunDetection(_raysUpLeft);
            _colUpRight = RunDetection(_raysUpRight);
            _colLeft = RunDetection(_raysLeft);
            _colRight = RunDetection(_raysRight);

            bool RunDetection(RayRange range) {
                return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, _detectionRayLength, _groundLayer));
            }
        }

        private void CalculateRayRanged() {
            // This is crying out for some kind of refactor. 
            Bounds b = new Bounds(transform.position, _characterBounds.size);

            _raysDown = new RayRange(b.min.x + _rayBuffer, b.min.y, b.max.x - _rayBuffer, b.min.y, Vector2.down);
            _raysDownLeft = new RayRange(b.min.x + _rayBuffer, b.min.y, b.max.x - _rayBuffer, b.min.y, new Vector2(-1,-1));
            _raysDownRight = new RayRange(b.min.x + _rayBuffer, b.min.y, b.max.x - _rayBuffer, b.min.y, new Vector2(1,-1));
            _raysUp = new RayRange(b.min.x + _rayBuffer, b.max.y, b.max.x - _rayBuffer, b.max.y, Vector2.up);
            _raysUpLeft = new RayRange(b.min.x + _rayBuffer, b.max.y, b.max.x - _rayBuffer, b.max.y, new Vector2(-1,1));
            _raysUpRight = new RayRange(b.min.x + _rayBuffer, b.max.y, b.max.x - _rayBuffer, b.max.y, new Vector2(1,1));
            _raysLeft = new RayRange(b.min.x, b.min.y + _rayBuffer, b.min.x, b.max.y - _rayBuffer, Vector2.left);
            _raysRight = new RayRange(b.max.x, b.min.y + _rayBuffer, b.max.x, b.max.y - _rayBuffer, Vector2.right);
        }


        private IEnumerable<Vector2> EvaluateRayPositions(RayRange range) {
            for (int i = 0; i < _detectorCount; i++) {
                float t = (float)i / (_detectorCount - 1);
                yield return Vector2.Lerp(range.Start, range.End, t);
            }
        }

        private void OnDrawGizmos() {
            // Bounds
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + _characterBounds.center, _characterBounds.size);

            // Rays
            if (!Application.isPlaying) {
                CalculateRayRanged();
                Gizmos.color = Color.blue;
                foreach (RayRange range in new List<RayRange> { _raysUp, _raysRight, _raysDown, _raysLeft }) {
                    foreach (Vector2 point in EvaluateRayPositions(range)) {
                        Gizmos.DrawRay(point, range.Dir * _detectionRayLength);
                    }
                }
            }

            if (!Application.isPlaying) return;

            // Draw the future position. Handy for visualizing gravity
            Gizmos.color = Color.red;
            var move = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed) * Time.deltaTime;
            Gizmos.DrawWireCube(transform.position + move, _characterBounds.size);
        }

        private void IncreaseHitBox(float previousMultiplier, float sizeMultiplier) {
            _characterBounds.size = (_characterBounds.size/previousMultiplier) * sizeMultiplier;
            transform.localScale = (transform.localScale/previousMultiplier) * sizeMultiplier;
        }
        
        #endregion

        #region Walk

        [Header("WALKING")] [SerializeField] private float _acceleration = 90;
        [SerializeField] private float _moveClamp = 13;
        [SerializeField] private float _deAcceleration = 60f;
        [SerializeField] private float _apexBonus = 2;

        private void CalculateWalk() {
            if (Input.X != 0) {
                if (Input.X != 0) transform.localScale = new Vector3(Input.X > 0 ? 
                    _baseHitBoxSizeMultiplier * _sizeMultiplier : -_baseHitBoxSizeMultiplier * _sizeMultiplier, 
                    _baseHitBoxSizeMultiplier * _sizeMultiplier, 1);
                
                // Set horizontal move speed
                _currentHorizontalSpeed += Input.X * _acceleration * Time.deltaTime;

                // clamped by max frame movement
                _currentHorizontalSpeed = Mathf.Clamp(_currentHorizontalSpeed, -_moveClamp, _moveClamp);

                // Apply bonus at the apex of a jump
                var apexBonus = Mathf.Sign(Input.X) * _apexBonus * _apexPoint;
                _currentHorizontalSpeed += apexBonus * Time.deltaTime;
            }
            else {
                // No input. Let's slow the character down
                _currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
            }

            if (_currentHorizontalSpeed > 0 && _colRight || _currentHorizontalSpeed < 0 && _colLeft) {
                // Don't walk through walls
                _currentHorizontalSpeed = 0;
            }
        }

        #endregion

        #region Gravity

        [Header("GRAVITY")] [SerializeField] private float _fallClamp = -40f;
        [SerializeField] private float _minFallSpeed = 80f;
        [SerializeField] private float _maxFallSpeed = 120f;
        private float _fallSpeed;

        private void CalculateGravity() {
            if (_colDown) {
                // Move out of the ground
                if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
            }
            else {
                // Add downward force while ascending if we ended the jump early
                var fallSpeed = _endedJumpEarly && _currentVerticalSpeed > 0 ? _fallSpeed * _jumpEndEarlyGravityModifier : _fallSpeed;

                // Fall
                _currentVerticalSpeed -= fallSpeed * Time.deltaTime;

                // Clamp
                if (_currentVerticalSpeed < _fallClamp) _currentVerticalSpeed = _fallClamp;
            }
        }

        #endregion

        #region Jump

        [Header("JUMPING")] [SerializeField] private float _jumpHeight = 30;
        [SerializeField] private float _jumpApexThreshold = 10f;
        [SerializeField] private float _coyoteTimeThreshold = 0.1f;
        [SerializeField] private float _jumpBuffer = 0.1f;
        [SerializeField] private float _jumpEndEarlyGravityModifier = 3;
        private bool _coyoteUsable;
        private bool _endedJumpEarly = true;
        private float _apexPoint; // Becomes 1 at the apex of a jump
        private float _lastJumpPressed;
        private bool CanUseCoyote => _coyoteUsable && !_colDown && _timeLeftGrounded + _coyoteTimeThreshold > Time.time;
        private bool HasBufferedJump => _colDown && _lastJumpPressed + _jumpBuffer > Time.time;

        private void CalculateJumpApex() {
            if (!_colDown) {
                // Gets stronger the closer to the top of the jump
                _apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(Velocity.y));
                _fallSpeed = Mathf.Lerp(_minFallSpeed, _maxFallSpeed, _apexPoint);
            }
            else {
                _apexPoint = 0;
            }
        }

        private void CalculateJump() {
            // Jump if: grounded or within coyote threshold || sufficient jump buffer
            if (Input.JumpDown && CanUseCoyote || HasBufferedJump) {
                _currentVerticalSpeed = _jumpHeight;
                _endedJumpEarly = false;
                _coyoteUsable = false;
                _timeLeftGrounded = float.MinValue;
                JumpingThisFrame = true;
            }
            else {
                JumpingThisFrame = false;
            }

            // End the jump early if button released
            if (!_colDown && Input.JumpUp && !_endedJumpEarly && Velocity.y > 0) {
                // _currentVerticalSpeed = 0;
                _endedJumpEarly = true;
            }

            if (_colUp && _colUpLeft && _colUpRight) {
                if (_currentVerticalSpeed > 0) _currentVerticalSpeed = 0;
            }
        }

        #endregion

        #region Move

        [Header("MOVE")] [SerializeField, Tooltip("Raising this value increases collision accuracy at the cost of performance.")]
        private int _freeColliderIterations = 20;

        // We cast our bounds before moving to avoid future collisions
        private void MoveCharacter() {
            Vector3 pos = transform.position;
            RawMovement = new Vector3(_currentHorizontalSpeed, _currentVerticalSpeed); // Used externally
            Vector3 move = RawMovement * Time.deltaTime;
            Vector3 furthestPoint = pos + move;

            // check furthest movement. If nothing hit, move and don't do extra checks
            Collider2D hit = Physics2D.OverlapBox(furthestPoint, _characterBounds.size, 0, _groundLayer);
            if (!hit) {
                transform.position += move;
                return;
            }

            // otherwise increment away from current pos; see what closest position we can move to
            Vector3 positionToMoveTo = transform.position;
            for (int i = 1; i < _freeColliderIterations; i++) {
                // increment to check all but furthestPoint - we did that already
                float t = (float)i / _freeColliderIterations;
                Vector2 posToTry = Vector2.Lerp(pos, furthestPoint, t);

                if (Physics2D.OverlapBox(posToTry, _characterBounds.size, 0, _groundLayer)) {
                    transform.position = positionToMoveTo;

                    // We've landed on a corner or hit our head on a ledge. Nudge the player gently
                    if (i == 1) {
                        if (_currentVerticalSpeed < 0) _currentVerticalSpeed = 0;
                        Vector3 dir = transform.position - hit.transform.position;
                        dir = new Vector3(dir.x, dir.y > 0 ? 1f : -1f, 0);
                        if (!_colDownLeft && dir.x < 0 || !_colDownRight && dir.x > 0){
                            transform.position += dir.normalized * move.magnitude;
                        }
                    }

                    return;
                }

                positionToMoveTo = posToTry;
            }
        }

        #endregion
    }
}