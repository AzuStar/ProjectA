using System;
using System.Collections.Generic;
using MessagePack;
using ProjectA.Game.Levels;

namespace ProjectA.Game.Registry;

[MessagePackObject]
public class PlayerSaveRegistryV1 : IRegistryObject
{
    public static readonly Guid GUID = Guid.Parse("c4d10804-e5d5-4108-b962-2bc5efb60447");

    [Key(0)]
    public Dictionary<LevelEnum, LevelSave> levels = new();

    public void SaveLevelStars(LevelEnum levelId, LevelSave levelSave)
    {
        levels.TryGetValue(levelId, out LevelSave savedLevel);
        savedLevel.timeStar |= levelSave.timeStar;
        savedLevel.coinsStar |= levelSave.coinsStar;
        savedLevel.chestsStar |= levelSave.chestsStar;
        levels[levelId] = savedLevel;
        IsDirty = true;
    }

    public int GetStarCount(LevelEnum levelId)
    {
        levels.TryGetValue(levelId, out LevelSave levelSave);
        return levelSave.StarCount;
    }

    public LevelSave GetLevelSave(LevelEnum levelId)
    {
        levels.TryGetValue(levelId, out LevelSave levelSave);
        return levelSave;
    }
}

[MessagePackObject]
public struct LevelSave
{
    [Key(0)]
    public bool timeStar;

    [Key(1)]
    public bool coinsStar;

    [Key(2)]
    public bool chestsStar;

    [IgnoreMember]
    public int StarCount => (timeStar ? 1 : 0) + (coinsStar ? 1 : 0) + (chestsStar ? 1 : 0);
}
