using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public Transform canvas;
    [Header("Levels")]
    public GameObject LevelPrefab;
    public GameObject UserLevelPrefab;
    public GameObject LevelDesignPrefab;
    public Transform LevelsContent;
    public Transform UserLevelsContent;
    public Transform DesignLevelsContent;


    private Stack<string> States;
    private List<GameObject> SubMenus;
    private GameObject ActiveSubMenu;

    void Awake()
    {
        States = new Stack<string>();
        GameData.Mode = "menu";
    }

    void Start()
    {
        //Store all submenus in list
        SubMenus = FindChildrenWithTag(canvas, "SubMenu");
        //Start on main menu
        ResetMenu();
        States.Push(SubMenus[0].name);
        ActiveSubMenu = SubMenus[0];
        ActiveSubMenu.SetActive(true);
        //Load all levels into menu
        LoadLevelNames("Levels");
        LoadLevelNames("User");
        LoadLevelNames("Design");
    }

    private bool ChangeMenu(string name)
    {
        ActiveSubMenu.SetActive(false);
        //Activate UI element of given name with SubMenu tag
        foreach (GameObject Menu in SubMenus)
        {
            if (Menu.name == name)
            {
                Menu.SetActive(true);
                ActiveSubMenu = Menu;
                return true;
            }
        }
        //Return false if not found
        return false;
    }

    public void AddLowerMenu(string name)
    {
        //Try to change menu
        if (!ChangeMenu(name))
            return;

        States.Push(name);

        //Set the active submenu
        foreach (GameObject Menu in SubMenus)
            if (Menu.name == name)
                ActiveSubMenu = Menu;
    }

    public void Back()
    {   
        //Remove current state from stack
        States.Pop();

        //Set menu to previous state        
        ChangeMenu(States.Peek());
    }

    public void ResetMenu()
    {
        //Deactivate all UI elements with tag SubMenu
        foreach (GameObject SubMenu in SubMenus)
            SubMenu.SetActive(false);
    }

    public void LoadLevelNames(string mode)
    {
        Transform content = UserLevelsContent;
        GameObject prefab = UserLevelPrefab;
        //Set content and prefab 
        switch (mode)
        {
            case "Levels":
                content = LevelsContent;
                prefab = LevelPrefab;
                break;
            case "User":
                content = UserLevelsContent;
                prefab = UserLevelPrefab;
                break;
            case "Design":
                content = DesignLevelsContent;
                prefab = LevelDesignPrefab;
                break;
        }


        IList<string> levels;
        //Load level names, either game levels or user levels
        if (mode == "Levels")
            levels = LevelLoader.LevelNames;
        else
            levels = LevelLoader.UserLevelNames;
        //Create new button for each level
        foreach (string level in levels)
        {
            CreateNewLevelButton(level, content, prefab);
        }
    }

    public void RefreshLevels()
    {
        //Clear old level buttons
        LevelButton[] levelButtons = LevelsContent.GetComponentsInChildren<LevelButton>();
        LevelButton[] userLevelButtons = UserLevelsContent.GetComponentsInChildren<LevelButton>();
        LevelButton[] designButtons = DesignLevelsContent.GetComponentsInChildren<LevelButton>();

        foreach(LevelButton levelbutton in levelButtons)
            Destroy(levelbutton.gameObject);
        foreach(LevelButton userLevelbutton in userLevelButtons)
            Destroy(userLevelbutton.gameObject);
        foreach(LevelButton designbutton in designButtons)
            Destroy(designbutton.gameObject);

        //Add updated list of levels
        LoadLevelNames("Levels");
        LoadLevelNames("User");
        LoadLevelNames("Design");
    }

    private void CreateNewLevelButton(string levelName, Transform content, GameObject prefab)
    {
        //Create new button
        GameObject newLevel = Instantiate(prefab);
        //Scale proportionally
        newLevel.transform.localScale = content.root.localScale;
        //Update name and parent
        newLevel.GetComponent<LevelButton>().UpdateName(levelName);
        newLevel.transform.SetParent(content);
        //Add reference to this menu (main menu)
        newLevel.GetComponent<LevelButton>().MainMenu = this;
    }

    private List<GameObject> FindChildrenWithTag(Transform parent, string tag)
    {
        List<GameObject> Children = new List<GameObject>();
        foreach (Transform child in parent)
            if (child.tag == tag)
                Children.Add(child.gameObject);

        return Children;
    }

    public void LoadNewLevel()
    {
        GameData.LevelFileName = null;
        GameData.SetDesign(null);
        SceneSwitch.LoadDesign();
    }

    public void LoadWorld()
    {
        GameData.SetWorld();
        GameData.LevelFileName = null;
        SceneSwitch.LoadWorld();
    }

    public void LoadDesignTutorial()
    {
        GameData.LevelFileName = null;
        GameData.SetDesign(null);
        GameData.TutorialState = -1;
        SceneSwitch.LoadDesignTutorial();
    }

    public void LoadPlayTutorial()
    {
        string name = LevelLoader.TutorialNames[0];
        SceneData scene = LevelLoader.Tutorials[name];
        GameData.SetLevel(scene);
        GameData.TutorialState = -1;
        SceneSwitch.LoadPlayTutorial();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void UpdateObjects()
    {
        string FolderDestination = Application.persistentDataPath + "/Objects/";

        //Update every object save
        foreach (string path in Directory.EnumerateFiles(FolderDestination, "*.bytes"))
        {
            UpdateSaves.UpdateObjectSave(path);
        }
    }
    
    public void UpdateLevels()
    {
        string FolderDestination = Application.persistentDataPath + "/Levels/";

        //Update every level save
        foreach (string path in Directory.EnumerateFiles(FolderDestination, "*.bytes"))
        {
            UpdateSaves.UpdateLevelSave(path);
        }
    }
}
