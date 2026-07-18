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
    public Dictionary<LevelEnum, LevelSave>  levels = new();

    public void SaveLevelResults(LevelEnum levelId, LevelSave levelSave)
    {
        if (!levels.TryGetValue(levelId, out LevelSave savedLevel))
        {
            savedLevel = levelSave;
        }
        else
        {
            savedLevel.timeStar |= levelSave.timeStar;
            savedLevel.coinsStar |= levelSave.coinsStar;
            savedLevel.chestsStar |= levelSave.chestsStar;
            savedLevel.cleared |= levelSave.cleared;
        }
        levels[levelId] = savedLevel;
        IsDirty = true;
    }

    public bool GetLevelSave(LevelEnum levelId, out LevelSave levelSave)
    {
        return levels.TryGetValue(levelId, out levelSave);
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

    [Key(3)]
    public bool cleared;

    [IgnoreMember]
    public int StarCount => (timeStar ? 1 : 0) + (coinsStar ? 1 : 0) + (chestsStar ? 1 : 0);
}
