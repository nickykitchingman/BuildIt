using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelLoader
{
    private static Dictionary<string, SceneData> _userLevels;
    private static Dictionary<string, SceneData> _levels;
    private static Dictionary<string, SceneData> _tutorials;
    public static IDictionary<string, SceneData> UserLevels
    {
        get
        {
            LoadUserLevelsIntoLists();
            return _userLevels;
        }
    }
    public static IDictionary<string, SceneData> Levels
    {
        get
        {
            LoadLevelsIntoLists();
            return _levels;
        }
    }
    public static IDictionary<string, SceneData> Tutorials
    {
        get
        {
            LoadTutorialsIntoLists();
            return _tutorials;
        }
    }

    private static List<string> _userLevelNames;
    private static List<string> _levelNames;
    private static List<string> _tutorialNames;
    public static IList<string> UserLevelNames
    {
        get
        {
            LoadUserLevelsIntoLists();
            return _userLevelNames;
        }
    }
    public static IList<string> LevelNames
    {
        get
        {
            LoadLevelsIntoLists();
            return _levelNames;
        }
    }
    public static IList<string> TutorialNames
    {
        get
        {
            LoadTutorialsIntoLists();
            return _tutorialNames;
        }
    }

    public static void Refresh()
    {
        LoadUserLevelsIntoLists();
        LoadLevelsIntoLists();
        LoadTutorialsIntoLists();
    }

    private static void LoadUserLevelsIntoLists()
    {
        //Initialise names and load object datas
        _userLevels = new Dictionary<string, SceneData>();
        _userLevelNames = new List<string>();

        //Add levels and names to dictionary and list
        foreach (SceneData level in GetListOfSceneDatas())
        {
            _userLevels.Add(level.Name, level);
            _userLevelNames.Add(level.Name);
        }
    }

    private static void LoadLevelsIntoLists()
    {
        //Initialise names and load object datas
        _levels = new Dictionary<string, SceneData>();
        _levelNames = new List<string>();

        string FolderDestination = "Levels";

        //Add levels and names to dictionary and list
        foreach (TextAsset levelAsset in Resources.LoadAll<TextAsset>(FolderDestination))
        {
            SceneData level = FileHandler.LoadLevel(levelAsset);
            _levels.Add(level.Name, level);
            _levelNames.Add(level.Name);
        }
    }

    private static void LoadTutorialsIntoLists()
    {
        //Initialise names and load object datas
        _tutorials = new Dictionary<string, SceneData>();
        _tutorialNames = new List<string>();

        string FolderDestination = "Tutorials";

        //Add levels and names to dictionary and list
        foreach (TextAsset levelAsset in Resources.LoadAll<TextAsset>(FolderDestination))
        {
            SceneData level = FileHandler.LoadLevel(levelAsset);
            _tutorials.Add(level.Name, level);
            _tutorialNames.Add(level.Name);
        }
    }

    private static List<SceneData> GetListOfSceneDatas()
    {
        List<SceneData> sceneDatas = new List<SceneData>();

        string FolderDestination = Application.persistentDataPath + "/Levels/";

        //Add file destinations, object datas and names to lists
        if (Directory.Exists(FolderDestination))
            foreach (string myfile in Directory.EnumerateFiles(FolderDestination, "*.bytes"))
            {
                SceneData FileObjectData = FileHandler.LoadLevel(myfile);
                sceneDatas.Add(FileObjectData);
            }

        return sceneDatas;
    }
}
