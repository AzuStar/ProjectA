using System;
using System.Collections.Generic;

namespace ProjectA.Game.Registry;

public static class TypeGuidRegistry
{
    private static readonly Dictionary<Type, Guid> _T2G = new Dictionary<Type, Guid>
    {
        [typeof(PlayerSaveRegistryV1)] = PlayerSaveRegistryV1.GUID,
    };

    private static readonly Dictionary<Guid, Type> _G2T = new Dictionary<Guid, Type>
    {
        [PlayerSaveRegistryV1.GUID] = typeof(PlayerSaveRegistryV1),
    };

    public static Guid GetGuid(Type t)
    {
        return _T2G[t];
    }

    public static Type GetType(Guid guid)
    {
        return _G2T[guid];
    }
}
