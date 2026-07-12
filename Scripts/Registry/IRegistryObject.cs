using MessagePack;

namespace ProjectA.Game.Registry;

public abstract class IRegistryObject
{
    [IgnoreMember]
    public bool IsDirty;
}
