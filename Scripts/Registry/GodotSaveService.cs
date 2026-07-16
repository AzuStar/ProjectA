using System;
using Godot;

namespace ProjectA.Game.Registry;

public class GodotSaveService : ISaveService
{
    private const string SaveDir = "user://save";

    public void SaveKey(string key, byte[] value)
    {
        DirAccess.MakeDirRecursiveAbsolute(ProjectSettings.GlobalizePath(SaveDir));
        using FileAccess file = FileAccess.Open(GetPath(key), FileAccess.ModeFlags.Write);
        file.StoreBuffer(value);
    }

    public byte[] LoadKey(string key)
    {
        string path = GetPath(key);
        if (!FileAccess.FileExists(path))
            return Array.Empty<byte>();

        return FileAccess.GetFileAsBytes(path);
    }

    private static string GetPath(string key) => $"{SaveDir}/{key}.bin";
}
