using Steamworks;
using Steamworks.Data;
using Studio23.SS2.AchievementSystem.Providers;
using UnityEngine;

public class SteamAchievementProvider : AchievementProvider
{

    public uint SteamAppID;

    public override void Initialize()
    {
        SteamClient.Init(SteamAppID);
    }

    public override void UnlockAchievement(string achievementIdentifier)
    {
        string mappedID= _mapper.IDMaps.Find(r=>r.Key == achievementIdentifier).Value;
        var achievement = new Achievement(mappedID);
        achievement.Trigger();
    }

    [ContextMenu("Reset All Stats")]
    internal void ClearAllStats()
    {
        SteamUserStats.ResetAll(true); 
        SteamUserStats.StoreStats();
        SteamUserStats.RequestCurrentStats();
    }



}
