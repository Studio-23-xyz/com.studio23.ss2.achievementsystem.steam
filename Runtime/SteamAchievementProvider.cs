using Cysharp.Threading.Tasks;
using Steamworks;
using Steamworks.Data;
using Studio23.SS2.AchievementSystem.Data;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "Steam Achievement Provider", menuName = "Studio-23/AchievementSystem/Steam Provider", order = 1)]

public class SteamAchievementProvider : AbstractAchievementProvider
{

    public uint SteamAppID;

    public override UniTask<AchievementData> GetAchievement(string id)
    {
        string mappedID = _achievementMapper.IDMaps.Find(r => r.Key == id).Value;

        var achievement = SteamUserStats.Achievements.FirstOrDefault(a => a.Identifier == mappedID);

        if (string.IsNullOrEmpty(achievement.Identifier))
        {
            Debug.LogWarning($"Achievement with ID '{mappedID}' not found.");
            return UniTask.FromResult<AchievementData>(null);
        }

        var data = new AchievementData
        {
            Id = id,
            Name = achievement.Name,
            IsUnlocked = achievement.State,
            ProgressState = achievement.State ? "Unlocked" : "Locked",
            Progression = achievement.State ? 1f : 0f,
            UnlockedDescription = achievement.Description,
            LockedDescription = achievement.Description
        };

        return UniTask.FromResult(data);
    }

    public override UniTask<AchievementData[]> GetAllAchievements()
    {
        List<AchievementData> list = new();

        foreach (var map in _achievementMapper.IDMaps)
        {
            var achievement = SteamUserStats.Achievements.FirstOrDefault(a => a.Identifier == map.Value);

            if (string.IsNullOrEmpty(achievement.Identifier))
            {
                Debug.LogWarning($"Achievement '{map.Value}' not found.");
                continue;
            }

            var data = new AchievementData
            {
                Id = map.Key,
                Name = achievement.Name,
                IsUnlocked = achievement.State,
                ProgressState = achievement.State ? "Unlocked" : "Locked",
                Progression = achievement.State ? 1f : 0f,
                UnlockedDescription = achievement.Description,
                LockedDescription = achievement.Description
            };

            list.Add(data);
        }
        return UniTask.FromResult(list.ToArray());

    }

    public override UniTask<int> Initialize()
    {
        SteamClient.Init(SteamAppID);
        return UniTask.FromResult((int)SteamAppID);
    }

    public override UniTask<int> UpdateAchievementProgress(string id, float progression)
    {
        string mappedID = _achievementMapper.IDMaps.Find(r => r.Key == id).Value;
        var achievement = new Achievement(mappedID);
        achievement.Trigger();
        return new UniTask<int>(1);
    }



    [ContextMenu("Reset All Stats")]
    internal void ClearAllStats()
    {
        SteamUserStats.ResetAll(true); 
        SteamUserStats.StoreStats();
        SteamUserStats.RequestCurrentStats();
    }



}
