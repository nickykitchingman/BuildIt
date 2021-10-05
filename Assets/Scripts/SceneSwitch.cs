using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    public Transform Player;
    public Transform Rover;
    public Transform Skybox;

    private CurrentAssets currentAssets;

    void Start()
    {
        //Add reference to the current assets
        currentAssets = SceneAssets.currentAssets;
    }

    public static void LoadWorld()
    {
        int scene = GameData.LevelMode ? 2 : 5;

        LoadScene(scene);
    }

    public static void LoadWorldInstant()
    {
        int scene = GameData.LevelMode ? 2 : 5;

        SceneManager.LoadScene(scene);
    }

    public static void LoadPlayTutorial()
    {
        int scene = 6;

        LoadScene(scene);
    }

    public static void LoadPlayTutorialInstant()
    {
        int scene = 6;

        SceneManager.LoadScene(scene);
    }

    public void LoadEditorTutorial()
    {
        int scene = 7;

        //Save current assets before loading new scene
        //Save player
        GameObject Clone = new GameObject();
        Clone.transform.position = new Vector3(-112, 3, 246);
        Vector3 rotation = new Vector3(25, 300, 0);
        Clone.transform.rotation = Quaternion.Euler(rotation);

        //Save scene
        GameData.CurrentScene = new SceneData(currentAssets, Clone.transform);
        GameData.LeftScene = true;
        GameData.FirstLoad = false;

        //Save skybox state
        if (Skybox)
            GameData.SkyboxRotation = Skybox.rotation;

        LoadScene(scene);
    }

    public void LoadObjectEditor()
    {
        int scene = 3;

        //Save current assets before loading new scene
        //Save player
        GameObject Clone = new GameObject();
        Clone.transform.position = Player.position;
        Clone.transform.rotation = Player.GetComponent<PlayerControls>().Rotation;
        
        //Save scene
        GameData.CurrentScene = new SceneData(currentAssets, Clone.transform);
        GameData.LeftScene = true;
        GameData.FirstLoad = false;
        GameData.CanSelect = true;

        //Save rover position
        if (Rover)
        {
            GameData.RoverPosition = Rover.position;
            GameData.RoverRotation = Rover.rotation;
        }
        
        //Save skybox state
        if (Skybox)
            GameData.SkyboxRotation = Skybox.rotation;

        LoadScene(scene);
    }

    public static void LoadMenu()
    {
        PhysicsControl.Reset();
        LoadScene(1);
    }

    public static void LoadDesign()
    {
        LoadScene(4);
    }
    
    public static void LoadDesignTutorial()
    {
        LoadScene(8);
    }

    public static void LoadScene(int scene)
    {
        SceneManager.LoadSceneAsync(9, LoadSceneMode.Single);
        GameData.SceneOperation = SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
        GameData.SceneOperation.allowSceneActivation = false;
    }
}
