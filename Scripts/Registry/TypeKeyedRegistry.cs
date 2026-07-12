using System;
using System.Collections.Generic;
using MessagePack;
using MessagePack.Resolvers;

namespace ProjectA.Game.Registry;

/// <summary>
/// Fully generic local-data registry
/// </summary>
public class TypeKeyedRegistry
{
    public const string REGISTRY_KEY = "_index";

    // Explicit serialization only; relies on [MessagePackObject] + [Key]
    private static readonly IFormatterResolver RESOLVER = CompositeResolver.Create(
        GeneratedMessagePackResolver.Instance,
        BuiltinResolver.Instance,
        AttributeFormatterResolver.Instance,
        PrimitiveObjectResolver.Instance
    );

    private static readonly MessagePackSerializerOptions _mpOptions = MessagePackSerializerOptions
        .Standard.WithResolver(RESOLVER)
        .WithCompression(MessagePackCompression.Lz4BlockArray);

    private readonly Dictionary<string, IRegistryObject> _objectRegistry = new Dictionary<string, IRegistryObject>();
    private _RegistryData _registryData = new _RegistryData();

    public TypeKeyedRegistry() { }

    public T GetOrCreate<T>()
        where T : IRegistryObject, new()
    {
        var type = typeof(T);
        string key = type.FullName ?? type.Name;

        if (_objectRegistry.TryGetValue(key, out var existing))
            return (T)existing;

        var id = TypeGuidRegistry.GetGuid(type);

        var obj = new T { IsDirty = true };

        _objectRegistry[key] = obj;
        _registryData.keyToGuid[key] = id;

        return obj;
    }

    public T Get<T>()
        where T : IRegistryObject
    {
        string key = typeof(T).FullName ?? typeof(T).Name;
        return _objectRegistry.TryGetValue(key, out var value) ? (T)value : default;
    }

    public Dictionary<string, byte[]> GetKV(bool dirtyOnly = true, bool clearDirtyFlags = true)
    {
        var allKVs = new Dictionary<string, byte[]>();

        // Serialize registry index (key -> id)
        allKVs[REGISTRY_KEY] = MessagePackSerializer.Serialize(_registryData, _mpOptions);

        foreach (var kv in _objectRegistry)
        {
            if (dirtyOnly && !kv.Value.IsDirty)
                continue;

            var runtimeType = kv.Value.GetType();
            var id = TypeGuidRegistry.GetGuid(runtimeType);

            // Ensure the registry index is consistent even if keyToId was missing
            _registryData.keyToGuid[kv.Key] = id;

            // Serialize with the runtime type (trusted, in-memory)
            allKVs[kv.Key] = MessagePackSerializer.Serialize(runtimeType, kv.Value, _mpOptions);

            if (clearDirtyFlags)
                kv.Value.IsDirty = false;
        }

        return allKVs;
    }

    public void ClearDirtyFlags()
    {
        foreach (var kv in _objectRegistry)
        {
            kv.Value.IsDirty = false;
        }
    }

    #region File-Saving (which I think shouldnt be there may be move out some day pls thank you good sir) but also version needs to be ++ed
    /// <summary>
    /// Create Registry object, returns true on success
    /// </summary>
    /// <param name="saveService"></param>
    /// <returns></returns>
    public bool InitializeRegistry(ISaveService saveService)
    {
        byte[] serializedRegistryIndex = saveService.LoadKey(REGISTRY_KEY);

        if (serializedRegistryIndex == null || serializedRegistryIndex.Length == 0)
            return false;

        try
        {
            _registryData = MessagePackSerializer.Deserialize<_RegistryData>(serializedRegistryIndex, _mpOptions);
        }
        catch
        {
            return false;
        }

        foreach (var entry in _registryData.keyToGuid)
        {
            byte[] serializedRegistryObject = saveService.LoadKey(entry.Key);
            if (serializedRegistryObject == null || serializedRegistryObject.Length == 0)
                continue;

            var targetType = TypeGuidRegistry.GetType(entry.Value);
            if (targetType == null)
            {
                continue;
            }

            if (!typeof(IRegistryObject).IsAssignableFrom(targetType))
                continue;

            try
            {
                var obj = (IRegistryObject)
                    MessagePackSerializer.Deserialize(targetType, serializedRegistryObject, _mpOptions);
                if (obj == null)
                    continue;

                _objectRegistry[entry.Key] = obj;
            }
            catch
            {
                // ignore corrupted entries
            }
        }

        return true;
    }

    public void SaveDirtyKVsOnly(ISaveService saveService)
    {
        saveService.SaveKeys(GetKV());
        _registryData.version++;
    }

    // expensive
    public void SaveEverything(ISaveService saveService)
    {
        saveService.SaveKeys(GetKV(false, false));
        _registryData.version++;
    }
    #endregion

    [MessagePackObject(AllowPrivate = true)]
    internal partial class _RegistryData
    {
        [Key(0)]
        public long version;

        [Key(1)]
        public long gameVersion; // for save upgrades etc

        // map from object key to type full name
        [Key(2)]
        public Dictionary<string, Guid> keyToGuid = new Dictionary<string, Guid>();
    }
}
