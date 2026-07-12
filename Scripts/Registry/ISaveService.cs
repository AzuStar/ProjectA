using System.Collections.Generic;

namespace ProjectA.Game.Registry;

public interface ISaveService
{
    /// <summary>
    /// Saves the data to the persistent data path, has backup logic
    /// Saved data is saved along with checksum to verify integrity
    /// </summary>
    public void SaveKeys(Dictionary<string, byte[]> data)
    {
        foreach (KeyValuePair<string, byte[]> kv in data)
        {
            SaveKey(kv.Key, kv.Value);
        }
    }

    /// <summary>
    /// Loads the data from the persistent data path, has backup logic
    /// Loaded data is verified for integrity, if integrity fails, load from backup
    /// </summary>
    public Dictionary<string, byte[]> LoadKeys(params string[] keys)
    {
        Dictionary<string, byte[]> data = new Dictionary<string, byte[]>();
        foreach (string key in keys)
        {
            byte[] s = LoadKey(key);
            if (s != null)
                data.Add(key, s);
        }
        return data;
    }
    void SaveKey(string key, byte[] value);

    /// <summary>
    /// If it fails to load data, returned string will be null
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    byte[] LoadKey(string key);
}
