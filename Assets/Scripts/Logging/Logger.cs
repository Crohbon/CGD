using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Logger {
    private static int _gameCount = 0;
    private static string _gameStateLog = "GameStateLog_Game";
    private static string _handicapsLog = "HandicapsLog_Game";
    private static string _weaponLog = "WeaponLog_Game";

    public static void LogGameState(bool isRoundEnd, bool isGameEnd, List<bool> alivePlayers, List<int> winPoints) {
        TextWriter Logwriter = new StreamWriter(Application.persistentDataPath + "\\" + _gameStateLog + _gameCount + ".csv", true);
        
        for (int i = 0; i < alivePlayers.Count; i++){
            Logwriter.WriteLine(System.DateTime.Now + ", Player " + i + " is alive," + alivePlayers[i] + "," + winPoints[i]);
        }

        if (isRoundEnd) Logwriter.WriteLine("[" + System.DateTime.Now + "] Round finished");
        if (isGameEnd){
            Logwriter.WriteLine("[" + System.DateTime.Now + "] Game finished");
            _gameCount++;
        }

        Logwriter.Close();
    }

    public static void LogHandicapsAfterRound(int playerIndex, int shotAmount, int jumpAmount, bool isWinningPlayer) {
        TextWriter Logwriter = new StreamWriter(Application.persistentDataPath + "\\" + _handicapsLog + _gameCount, true);

        int playerWinPoints = isWinningPlayer
            ? GameManager.Instance.WinPoints[playerIndex] - 1
            : GameManager.Instance.WinPoints[playerIndex];
        
        switch (playerWinPoints){
            case 0:
                Logwriter.WriteLine(System.DateTime.Now + ", Player " + playerIndex + "," + shotAmount + "," + jumpAmount 
                                    + false + "," + false + "," + false + "," + false + "," + false + "," + false + "," + false + "," + false);
                break;
            case 1:
                Logwriter.WriteLine(System.DateTime.Now + ", Player " + playerIndex + "," + shotAmount + "," + jumpAmount 
                                    + (HandicapValues.ShotsWithoutDamageReduction/shotAmount >= 1) + "," 
                                    + (HandicapValues.ShotsForMaxDamageReduction/shotAmount >= 1) + "," 
                                    + false + "," + false + "," + false + "," + false + "," + false + "," + false);
                break;
            case 2:
                Logwriter.WriteLine(System.DateTime.Now + ", Player " + playerIndex + "," + shotAmount + "," + jumpAmount 
                                    + (HandicapValues.ShotsWithoutDamageReduction/shotAmount >= 1) + "," 
                                    + (HandicapValues.ShotsForMaxDamageReduction/shotAmount >= 1) + "," 
                                    + (HandicapValues.ShotsWithoutBulletDrop/shotAmount >= 1) + "," 
                                    + (HandicapValues.ShotsForMinBulletDropRange/shotAmount >= 1) + "," 
                                    + false + "," + false + "," + false + "," + false);
                break;
            case 3:
                Logwriter.WriteLine(System.DateTime.Now + ", Player " + playerIndex + "," + shotAmount + "," + jumpAmount 
                                    + (HandicapValues.ShotsWithoutDamageReduction/shotAmount >= 1) + "," 
                                    + (HandicapValues.ShotsForMaxDamageReduction/shotAmount >= 1) + "," 
                                    + (HandicapValues.ShotsWithoutBulletDrop/shotAmount >= 1) + "," 
                                    + (HandicapValues.ShotsForMinBulletDropRange/shotAmount >= 1) + "," 
                                    + (HandicapValues.JumpsWithoutHitBoxIncrease/shotAmount >= 1) + "," 
                                    + (HandicapValues.JumpsForMaxHitBoxIncrease/shotAmount >= 1) + "," 
                                    + false + "," + false);
                break;
            case 4:
                Logwriter.Write(System.DateTime.Now + ", Player " + playerIndex + "," + shotAmount + "," + jumpAmount
                                + false + "," + false + "," + false + "," + false + "," + false + "," + false + ","
                                + (HandicapValues.ShotsPerControlsInvert > jumpAmount) + ",");
                if (jumpAmount > 0){
                    Logwriter.WriteLine(HandicapValues.ShotsPerControlsInvert/jumpAmount);
                }
                else{
                    Logwriter.WriteLine(false);
                }

                
                break;
        }

        if (isWinningPlayer) Logwriter.WriteLine(System.DateTime.Now + ", Player " + playerIndex + ", Round is over"); 

        Logwriter.Close();
    }

    public static void LogWeaponUsage(int playerIndex, string weaponName, int ammoLeft) {
        TextWriter Logwriter = new StreamWriter(Application.persistentDataPath + "\\" + _weaponLog + _gameCount + ".csv", true);

        Logwriter.WriteLine(System.DateTime.Now + "," + playerIndex + "," + weaponName + "," + ammoLeft);
        
        Logwriter.Close();
    }
    
    public static void LogWeaponImpact(int gunnerIndex, string weaponName, float damageDealt, int hitPlayerIndex, bool killedPlayer) {
        TextWriter Logwriter = new StreamWriter(Application.persistentDataPath + "\\" + _weaponLog + _gameCount + ".csv", true);

        Logwriter.WriteLine(System.DateTime.Now + "," + gunnerIndex + "," + weaponName + "," + hitPlayerIndex + "," + damageDealt + "," + killedPlayer);
        
        Logwriter.Close();
    }
}
