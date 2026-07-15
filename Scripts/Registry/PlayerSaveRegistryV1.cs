using System;
using System.Collections.Generic;
using MessagePack;

namespace ProjectA.Game.Registry;

[MessagePackObject]
public class PlayerSaveRegistryV1 : IRegistryObject
{
    public static readonly Guid GUID = Guid.Parse("c4d10804-e5d5-4108-b962-2bc5efb60447");

    [Key(0)]
    public Dictionary<int, LevelSave> levels = new();

    public void SaveLevelStars(int levelId, LevelSave levelSave)
    {
        levels.TryGetValue(levelId, out LevelSave savedLevel);
        savedLevel.timeStar |= levelSave.timeStar;
        savedLevel.coinsStar |= levelSave.coinsStar;
        savedLevel.chestsStar |= levelSave.chestsStar;
        levels[levelId] = savedLevel;
        IsDirty = true;
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
}
