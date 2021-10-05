using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UI;

public class TimeScale : MonoBehaviour
{
    public Transform Player;
    public Transform Rover;
    public Notification notification;
    [Header("Buttons")]
    public GameObject PlayButton;
    public GameObject StopButton;
    [Header("Sprites")]
    public Sprite PlaySprite;
    public Sprite PauseSprite;

    private CurrentAssets currentAssets;

    void Start()
    {
        //Add reference to the current assets
        currentAssets = SceneAssets.currentAssets;
        Pause(true);
    }

    public void Play()
    {
        //Check if rendering before playing
        if (GameData.IsRenderingNew || GameData.IsRenderingOld)
        {
            notification.Notify("Cannot play while selecting object!", Color.red);
            return;
        }

        PhysicsControl.Play();
        PlayButton.GetComponent<Image>().sprite = PauseSprite;
        GameData.IsPaused = false;

        if (!GameData.GameIsPlaying)
        {
            //Save scene if playing (not from pause)
            //Save player
            GameObject Clone = new GameObject();
            Clone.transform.position = Player.position;
            Clone.transform.rotation = Player.GetComponent<PlayerControls>().Rotation;

            GameData.PreviousPoint = new SceneData(currentAssets, Clone.transform);
            StopButton.SetActive(true);
            GameData.GameIsPlaying = true;
        }
    }

    public void Pause(bool preserve)
    {
        //Pause physics and update UI graphics
        PhysicsControl.Stop(preserve);
        PlayButton.GetComponent<Image>().sprite = PlaySprite;
        GameData.IsPaused = true;
        if (GameData.GameIsPlaying)
        {
            StopButton.SetActive(true);
        }
    }

    public void Pause()
    {
        Pause(false);
    }

    public void Stop()
    {
        //Pause time
        Pause();
        StopButton.SetActive(false);
        GameData.GameIsPlaying = false;

        //Stop function if no saved scene
        if (GameData.PreviousPoint == null)
            return;

        //Save rover position
        if (Rover)
        {
            GameData.RoverPosition = Rover.position;
            GameData.RoverRotation = Rover.rotation;
        }

        //ReloadScene to when game played
        GameData.FirstLoad = false;
        setPlayerState(GameData.PreviousPoint);
        SceneSwitch.LoadWorldInstant();
    }

    public void TutorialStop()
    {
        //Pause time
        Pause();
        StopButton.SetActive(false);
        GameData.GameIsPlaying = false;

        //Stop function if no saved scene
        if (GameData.PreviousPoint == null)
            return;

        //ReloadScene to when game played
        GameData.FirstLoad = false;
        setPlayerState(GameData.PreviousPoint);
        SceneSwitch.LoadPlayTutorialInstant();
    }


    public void PlayOrPause()
    {
        //Switch between playing and pausing
        if (GameData.IsPaused)
            Play();
        else
            Pause();
    }

    private void setPlayerState(SceneData scene)
    {
        PlayerControls player = Player.GetComponent<PlayerControls>();
        if (player)
        {
            //Get current player state
            Vector3 position = player.transform.position;
            Quaternion rotation = player.Rotation;
            //Position
            scene.PlayerPosX = position.x;
            scene.PlayerPosY = position.y;
            scene.PlayerPosZ = position.z;
            //Rotation
            scene.PlayerRotX = rotation.x;
            scene.PlayerRotY = rotation.y;
            scene.PlayerRotZ = rotation.z;
            scene.PlayerRotW = rotation.w;
        }
        
    }
}

/// <summary>
/// Handles whether physics is playing or not using kinematic rigidbodies
/// </summary>
public static class PhysicsControl
{
    public static Dictionary<ObjectReference, Vector3> Velocities = new Dictionary<ObjectReference, Vector3>();
    public static Dictionary<ObjectReference, Vector3> AngularVelocities = new Dictionary<ObjectReference, Vector3>();

    /// <summary>
    /// Determines if physics is currently playing
    /// </summary>
    public static bool IsPlaying { get; private set; } = true;

    /// <summary>
    /// Plays physics
    /// </summary>
    public static void Play()
    {
        SetAllToKinematic(false);
        RestoreAllMomentums();
        IsPlaying = true;
    }

    /// <summary>
    /// Stops physics, preserving previous momentum
    /// </summary>
    public static void Stop(bool preserve)
    {
        if (!preserve)
            StoreAllMomentums();
        SetAllToKinematic(true);
        IsPlaying = false;
    }

    /// <summary>
    /// Stops physics
    /// </summary>
    public static void Stop()
    {
        Stop(false);
    }

    /// <summary>
    /// Reset all stored velocities and angular velocities
    /// </summary>
    public static void Reset()
    {
        Velocities = new Dictionary<ObjectReference, Vector3>();
        AngularVelocities = new Dictionary<ObjectReference, Vector3>();
    }

    /// <summary>
    /// Sets every rigidbody in scene to kinematic
    /// </summary>
    private static void SetAllToKinematic(bool value)
    {
        //User objects
        for (int i = 0; i < SceneAssets.currentAssets.UserObjectsInScene.Count; i++)
        {
            GameObject gameobject = SceneAssets.currentAssets.UserObjectsInScene[i];
            //Ignore deleted objects
            if (gameobject == null)
                continue;

            Rigidbody rigidbody = gameobject.GetComponent<Rigidbody>();
            if (rigidbody)
                rigidbody.isKinematic = value;
        }

        //Level objects
        for (int i = 0; i < SceneAssets.currentAssets.LevelObjectsInScene.Count; i++)
        {
            GameObject gameobject = SceneAssets.currentAssets.LevelObjectsInScene[i];
            //Ignore deleted objects
            if (gameobject == null)
                continue;

            Rigidbody rigidbody = gameobject.GetComponent<Rigidbody>();
            if (rigidbody)
                rigidbody.isKinematic = value;
        }

        //Triggers
        for (int i = 0; i < SceneAssets.currentAssets.TriggersInScene.Count; i++)
        {
            GameObject gameobject = SceneAssets.currentAssets.TriggersInScene[i].ThisPlane;
            //Ignore deleted objects
            if (gameobject == null)
                continue;

            Rigidbody rigidbody = gameobject.GetComponent<Rigidbody>();
            if (rigidbody)
                rigidbody.isKinematic = value;
        }
    }

    /// <summary>
    /// Stores a record of each velocity and angular velocity of every rigidbody
    /// </summary>
    public static void StoreAllMomentums()
    {
        //Clear records
        Velocities = new Dictionary<ObjectReference, Vector3>();
        AngularVelocities = new Dictionary<ObjectReference, Vector3>();

        //User objects
        for (int i = 0; i < SceneAssets.currentAssets.UserObjectsInScene.Count; i++)
        {
            ObjectReference reference = new ObjectReference("UserObject", i);
            GameObject gameObject = SceneAssets.currentAssets.UserObjectsInScene[i];
            //Ignore deleted objects
            if (reference == null)
                continue;

            Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
            if (rigidbody)
            {
                Velocities.Add(reference, rigidbody.velocity);
                AngularVelocities.Add(reference, rigidbody.angularVelocity);
            }
        }

        //Level objects
        for (int i = 0; i < SceneAssets.currentAssets.LevelObjectsInScene.Count; i++)
        {
            ObjectReference reference = new ObjectReference("LevelObject", i);
            GameObject gameobject = SceneAssets.currentAssets.LevelObjectsInScene[i];
            //Ignore deleted objects
            if (gameobject == null)
                continue;

            Rigidbody rigidbody = gameobject.GetComponent<Rigidbody>();
            if (rigidbody)
            {
                Velocities.Add(reference, rigidbody.velocity);
                AngularVelocities.Add(reference, rigidbody.angularVelocity);
            }
        }

        //Triggers
        for (int i = 0; i < SceneAssets.currentAssets.TriggersInScene.Count; i++)
        {
            ObjectReference reference = new ObjectReference("Trigger", i);
            GameObject gameobject = SceneAssets.currentAssets.TriggersInScene[i].ThisPlane;
            //Ignore deleted objects
            if (gameobject == null)
                continue;

            Rigidbody rigidbody = gameobject.GetComponent<Rigidbody>();
            if (rigidbody)
            {
                Velocities.Add(reference, rigidbody.velocity);
                AngularVelocities.Add(reference, rigidbody.angularVelocity);
            }
        }
    }
    
    // <summary>
    /// Restores each velocity and angular velocity of every rigidbody
    /// </summary>
    public static void RestoreAllMomentums()
    {
        //Velocities
        foreach (KeyValuePair<ObjectReference, Vector3> velocity in Velocities)
        {
            GameObject gameObject = velocity.Key.target;
            //Ignore deleted objects
            if (velocity.Value == null || !gameObject.activeSelf)
                continue;

            //Add velocity to each gameobject
            Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
            if (rigidbody)
                rigidbody.velocity = velocity.Value;
        }
        
        //Angular velocities
        foreach (KeyValuePair<ObjectReference,Vector3> angularvelocity in AngularVelocities)
        {
            GameObject gameObject = angularvelocity.Key.target;
            //Ignore deleted objects
            if (angularvelocity.Value == null || !gameObject.activeSelf)
                continue;

            //Add angular velocity to each gameobject
            Rigidbody rigidbody = gameObject.GetComponent<Rigidbody>();
            if (rigidbody)
                rigidbody.angularVelocity = angularvelocity.Value;
        }
    }
}

