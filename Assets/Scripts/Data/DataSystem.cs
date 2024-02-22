using UnityEngine;
using System.IO;

public static class DataSystem
{
    /// <summary>
    /// 保存数据通过Json
    /// </summary>
    /// <param name="fileName"> 文件名称作为key </param>
    /// <param name="data"></param>
    public static void Save(string fileName, object data)
    {
        var jsonData = JsonUtility.ToJson(data);
        var path = Path.Combine(Application.persistentDataPath, fileName);

        File.WriteAllText(path, jsonData);

#if UNITY_EDITOR
        Debug.Log($"save the data to {path} successly.\n {jsonData}");
#endif
    }

    /// <summary>
    /// 读取数据通过Json
    /// </summary>
    /// <param name="fileName"> 数据保存的文件名 </param>
    /// <returns></returns>
    public static T Load<T>(string fileName)
    {
        var path = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            var jsonData = File.ReadAllText(path);
            var data = JsonUtility.FromJson<T>(jsonData);
            return data;
        }
        catch (System.Exception exception)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"Failed to load data form {path},\n {exception}");
#endif
            return default;
        }
    }

    public static void DeleteData(string fileName)
    {
        var path = Path.Combine(Application.persistentDataPath, fileName);

        try
        {
            File.Delete(path);
        }
        catch (System.Exception exception)
        {
#if UNITY_EDITOR
            Debug.LogWarning($"Failed to delete data form {path},\n {exception}");
#endif
        }
    }
}
