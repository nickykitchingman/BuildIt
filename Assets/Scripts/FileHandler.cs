using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public static class FileHandler
{
    /// <summary>
    /// Saves an object to a file using serialisation
    /// </summary>
    /// <param name="Object"></param>
    public static bool SaveObject(GameObject Object)
    {
        string Name = Object.name;
        //Create directory of new file
        string destination = MyAssets.ObjectPath + Name + ".bytes";
        FileStream file;
        if (!Directory.Exists(MyAssets.ObjectPath))
            Directory.CreateDirectory(MyAssets.ObjectPath);

        //Overwrite or create new file at this directory
        if (File.Exists(destination))
            file = File.OpenWrite(destination);
        else
            file = File.Create(destination);

        //Save object as ObjectData type, formatted in binary
        ParentData data = new ParentData(Object);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, data);
        file.Close();
        return true;
    }

    /// <summary>
    /// Loads an object from a file using deserialisation
    /// </summary>
    /// <param name="Destination"></param>
    /// <returns></returns>
    public static ParentData LoadObject(string Destination)
    {
        FileStream file;

        //Open file if exists, return null if not
        if (!Directory.Exists(MyAssets.ObjectPath))
            Directory.CreateDirectory(MyAssets.ObjectPath);
        if (File.Exists(Destination))
            file = File.OpenRead(Destination);
        else
            return null;

        //Deserialize object data from file
        BinaryFormatter bf = new BinaryFormatter();
        ParentData data = (ParentData)bf.Deserialize(file);
        file.Close();

        return data;
    }

    /// <summary>
    /// Delete an object by name
    /// </summary>
    /// <param name="name"></param>
    public static void DeleteObject(string name)
    {
        string destination = MyAssets.ObjectPath + name + ".bytes";

        if (File.Exists(destination))
            File.Delete(destination);
    }

    /// <summary>
    /// Saves a level to a file using serialisation
    /// </summary>
    /// <param name="currentAssets"></param>
    /// <param name="Player"></param>
    public static void SaveLevel(CurrentAssets currentAssets, Transform Player)
    {
        string Name = GameData.LevelName;

        //Create directory of new file
        string destination = MyAssets.LevelPath + Name + ".bytes";
        FileStream file;

        //Overwrite or create new file at this directory
        if (!Directory.Exists(MyAssets.LevelPath))
            Directory.CreateDirectory(MyAssets.LevelPath);
        if (File.Exists(destination))
            file = File.OpenWrite(destination);
        else
            file = File.Create(destination);

        //Save level as SceneData type, serialized in binary
        SceneData Level = new SceneData(currentAssets, Player);
        BinaryFormatter bf = new BinaryFormatter();
        bf.Serialize(file, Level);
        file.Close();

        //Delete old file directory
        string olddestination = MyAssets.LevelPath + GameData.LevelFileName + ".bytes";
        if (GameData.LevelFileName != null && File.Exists(olddestination))
            File.Delete(olddestination);

        //Add new filename to gamedata
        GameData.LevelFileName = Name;
    }

    /// <summary>
    /// Loads a level from a file using deserialisation
    /// </summary>
    /// <param name="Destination"></param>
    /// <returns></returns>
    public static SceneData LoadLevel(string Destination)
    {
        FileStream file;

        //Open file if exists, return null if not
        if (File.Exists(Destination))
            file = File.OpenRead(Destination);
        else
            return null;

        //Deserialize object data from file
        BinaryFormatter bf = new BinaryFormatter();
        SceneData data = (SceneData)bf.Deserialize(file);
        file.Close();

        return data;
    }

    /// <summary>
    /// Deserialises a level from a text asset
    /// </summary>
    /// <param name="asset"></param>
    /// <returns></returns>
    public static SceneData LoadLevel(TextAsset asset)
    {
        MemoryStream stream = new MemoryStream(asset.bytes);
        //Deserialize object data from file
        BinaryFormatter bf = new BinaryFormatter();
        SceneData data = (SceneData)bf.Deserialize(stream);

        return data;
    }

    /// <summary>
    /// Delete a level by name
    /// </summary>
    /// <param name="name"></param>
    public static void DeleteLevel(string name)
    {
        string destination = MyAssets.LevelPath + name + ".bytes";

        if (Directory.Exists(MyAssets.LevelPath))
            if (File.Exists(destination))
                File.Delete(destination);

        Debug.Log(destination);
    }

    public static void SaveSettings(string contents)
    {
        string destination = MyAssets.UserDataPath + "Settings.txt";
        FileStream file;

        //Overwrite or create new file at this directory
        if (!Directory.Exists(MyAssets.UserDataPath))
            Directory.CreateDirectory(MyAssets.UserDataPath);
        if (File.Exists(destination))
        {
            File.WriteAllText(destination, string.Empty);
            file = File.OpenWrite(destination);
        }
        else
            file = File.Create(destination);

        // writing data in string
        byte[] info = new UTF8Encoding(true).GetBytes(contents);
        file.Write(info, 0, info.Length);

        file.Close();
    }

    public static string ReadSettings()
    {
        string destination = MyAssets.UserDataPath + "Settings.txt";

        //Read in settings file
        if (Directory.Exists(MyAssets.UserDataPath))
            if (File.Exists(destination))
                return File.ReadAllText(destination);

        return null;
    }
}
