using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

[System.Serializable]
public class LeaderboardData
{
    
   public List<LeaderboardEntry> entries;

   public LeaderboardData() {
   }


   public List<LeaderboardEntry> GetFromJson() {
       List<LeaderboardEntry> entries;
       if (!File.Exists(Application.persistentDataPath + "/leaderboard.json")) {
           entries = new List<LeaderboardEntry> {};
       } else {
           entries = JsonUtility.FromJson<LeaderboardData>(
               File.ReadAllText(Application.persistentDataPath + "/leaderboard.json")
            ).entries;
       }

        this.entries = entries;
        return entries;
   }

   public void SaveToJson() {
       File.WriteAllText(
           Application.persistentDataPath + "/leaderboard.json",
           JsonUtility.ToJson(this)
        );
   }
}

[System.Serializable]
public class LeaderboardEntry {
    public string Username;
    public int Score;
}