using UnityEngine;

public class SetUpScene : MonoBehaviour
{
    public bool DesignMode;
    public GameObject Player;
    public GameObject Rover;
    [Header("Objects")]
    public GameObject SceneObjects;
    public GameObject LevelObjects;
    public GameObject Triggers;
    [Header("Objectives")]
    public Transform ConditionsContent;
    public GameObject ObjectivePrefab;
    [Header("Scripts")]
    public World world;
    public TimeScale timeScale;
    public PreRenderObject preRenderObject;

    void Awake()
    {
        //Create new current assets when scene is loaded
        SceneAssets.currentAssets = new CurrentAssets();
    }

    void Start()
    {
        //Initialise populate class
        Populate.Player = Player;
        Populate.Rover = Rover;
        Populate.SceneObjects = SceneObjects;
        Populate.LevelObjects = LevelObjects;
        Populate.Triggers = Triggers;
        Populate.ConditionsContent = ConditionsContent;
        Populate.ObjectivePrefab = ObjectivePrefab;

        if (GameData.LeftScene && GameData.CurrentScene != null)
            //Set up scene with current scene in gamedata
            Populate.PopulateScene(GameData.CurrentScene, DesignMode);
        else if (!GameData.LeftScene && GameData.PreviousPoint != null)
        {
            //Set up scene with original scene in gamedata
            Populate.PopulateScene(GameData.PreviousPoint, DesignMode);
            if (!DesignMode)
                timeScale.Pause();
        }

        //Set left scene to false after scene loaded
        GameData.LeftScene = false;
    }
}

public static class Populate
{
    public static bool PlayingMode;
    public static GameObject SceneObjects;
    public static GameObject LevelObjects;
    public static GameObject Triggers;
    public static GameObject Player;
    public static GameObject Rover;
    public static Transform ConditionsContent;
    public static GameObject ObjectivePrefab;

    private static GameObject NewObject;
    private static Trigger NewTrigger;
    private static Condition NewCondition;

    public static void PopulateScene(SceneData scene, bool DesignMode)
    {
        //Set player position and rotation
        Player.transform.position = scene.GetPosition();
        Player.GetComponent<PlayerControls>().Rotation = scene.GetRotation();

        //Set rover position
        if (!GameData.FirstLoad && Rover)
        {
            Rover.transform.position = GameData.RoverPosition;
            Rover.transform.rotation = GameData.RoverRotation;
        }

        //Load each user object
        foreach (ParentData UserObject in scene.Objects)
        {
            //Load in every user object
            NewObject = HandleUserObjects.LoadObject(UserObject);
            //Remove rigidbody if design
            if (DesignMode)
                Object.Destroy(NewObject.GetComponent<Rigidbody>());
            NewObject.transform.parent = SceneObjects.transform;
            //Add to current assets
            SceneAssets.currentAssets.AddUserObject(NewObject);
        }

        //Load each level object
        foreach (ParentData LevelObject in scene.LevelObjects)
        {
            //Load in every level object
            NewObject = LoadBuiltInObjects.Unpack(LevelObject);
            //Remove rigidbody if design
            if (DesignMode)
                Object.Destroy(NewObject.GetComponent<Rigidbody>());
            NewObject.transform.parent = LevelObjects.transform;
            //Add to current assets
            SceneAssets.currentAssets.AddLevelObject(NewObject);
        }

        //Load each trigger
        foreach (TriggerData trigger in scene.Triggers)
        {
            //Load in every trigger
            NewTrigger = LoadBuiltInObjects.Unpack(trigger);
            //Remove rigidbody if design
            if (DesignMode)
                Object.Destroy(NewTrigger.ThisPlane.GetComponent<Rigidbody>());
            NewTrigger.ThisPlane.transform.parent = Triggers.transform;
            if (!DesignMode)
            {
                //Add script to trigger to handle conditions
                TriggerScript script = NewTrigger.ThisPlane.AddComponent<TriggerScript>();
                script.trigger = NewTrigger;
                script.CollisionEnter.AddListener(LevelComplete.HandleTriggerCollision);
            }
            //Add to current assets
            SceneAssets.currentAssets.AddTrigger(NewTrigger);
        }

        //Add conditions to conditions view
        foreach (ConditionData condition in scene.Conditions)
        {
            //Create new objective graphic for each condition
            NewCondition = LoadBuiltInObjects.Unpack(condition);
            //Add to current assets
            SceneAssets.currentAssets.LevelConditions.Add(NewCondition);
            if (!DesignMode)
            {
                GameObject ObjectiveGraphic = GameObject.Instantiate(ObjectivePrefab, ConditionsContent);
                SceneAssets.currentAssets.ObjectiveGraphics.Add(NewCondition, ObjectiveGraphic.GetComponent<Objective>());
                //Update objective graphic
                ObjectiveGraphic.GetComponent<Objective>().SetCondition(NewCondition);
            }
        }

        //Add tags to scene assets
        if (scene.Tags != null)
            foreach (string tag in scene.Tags)
                SceneAssets.currentAssets.Tags.Add(tag);
    }
}
