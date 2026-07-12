using System;
using System.Collections.Generic;

namespace ProjectA.Game.Registry;

public static class TypeGuidRegistry
{
    private static Dictionary<Type, Guid> _T2G = new Dictionary<Type, Guid> { };
    private static Dictionary<Guid, Type> _G2T = new Dictionary<Guid, Type> { };

    public static Guid GetGuid(Type t)
    {
        return _T2G[t];
    }

    public static Type GetType(Guid guid)
    {
        return _G2T[guid];
    }
}
