using System.Collections.Generic;
using UnityEngine;

public static class GameData 
{
    public static SceneData CurrentScene;
    public static SceneData PreviousPoint;
    public static SceneData OriginalScene;

    public static ActionStack LevelActions = new ActionStack();
    public static ActionStack EditorActions = new ActionStack();
    public static ActionStack DesignActions = new ActionStack();

    public static ParentData NewObject;
    public static Vector3 RoverPosition;
    public static Quaternion RoverRotation;

    public static bool LeftScene;
    public static bool FirstLoad = true;
    
    public static string LevelName;
    public static string LevelFileName;

    public static bool IsMoving;

    private static bool cursorFree;
    private static bool _controlsEnabled = true;
    public static bool ControlsEnabled
    {
        get { return _controlsEnabled; }
        set {
            cursorFree = Cursor.visible;
            _controlsEnabled = value;
            if (!value)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else if (!cursorFree)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }

    }

    public static bool IsRenderingOld;
    public static bool IsRenderingNew;

    public static bool CanSelect = true;

    /// <summary>
    /// Environment (levelplay, design, editor)
    /// </summary>
    public static string Mode;
    public static bool LevelMode;
    public static AsyncOperation SceneOperation;
    
    public static bool IsPaused;
    public static bool GameIsPlaying;

    public static bool MusicOn = true;
    public static bool EffectsOn = true;
    public static float MusicVolume = 0.4f; 
    public static float EffectsVolume = 0.8f; 
    public static int MusicTime = 0; 
    public static float SceneExitTime; 

    public static int TutorialState = -1;
    public static bool CanConfirm = true;
    public static bool NumpadEnabled = true;

    public static Quaternion SkyboxRotation;

    public static List<GameObject> CurrentChildObjects = new List<GameObject>();

    public static void SetLevel(SceneData scene)
    {
        //Initialise fields as a new level
        OriginalScene = scene;
        SkyboxRotation = Quaternion.identity;
        Mode = "levelplay";
        LevelMode = true;
        LevelName = scene.Name;
        LevelFileName = LevelName;
        ResetLevel();
    }
    
    public static void SetDesign(SceneData scene)
    {
        //Initialise fields as a new level
        OriginalScene = scene;
        SkyboxRotation = Quaternion.identity;
        Mode = "leveldesign";
        if (scene != null)
            LevelName = OriginalScene.Name;
        else
            LevelName = null;

        ResetLevel();
    }

    public static void SetWorld()
    {
        OriginalScene = null;
        SkyboxRotation = Quaternion.identity;
        Mode = "world";
        LevelMode = false;
        ResetLevel();
    }

    public static void ResetLevel()
    {
        //Reset fields to base level
        CurrentScene = PreviousPoint = OriginalScene;
        LevelActions = new ActionStack();
        EditorActions = new ActionStack();
        DesignActions = new ActionStack();
        LeftScene = false;
        FirstLoad = true;
        IsMoving = true;
        ControlsEnabled = true;
        IsRenderingOld = IsRenderingNew = false;
        CanSelect = true;
        IsPaused = true;
        GameIsPlaying = false;
        CurrentChildObjects = new List<GameObject>();
        MusicTime = 0;
    }
}
