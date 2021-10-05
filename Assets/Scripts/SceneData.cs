using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SceneData 
{
    public string Name;
    public List<ParentData> Objects;
    public List<TriggerData> Triggers;
    public List<ParentData> LevelObjects;
    public List<ConditionData> Conditions;
    public List<string> Tags;

    public float PlayerPosX, PlayerPosY, PlayerPosZ;
    public float PlayerRotX, PlayerRotY, PlayerRotZ, PlayerRotW;

    [System.NonSerialized]
    private CurrentAssets SceneAssets;

    public SceneData(CurrentAssets SceneAssets, Transform Player)
    {
        //Initialise
        Objects = new List<ParentData>();
        Triggers = new List<TriggerData>();
        LevelObjects = new List<ParentData>();
        Conditions = new List<ConditionData>();
        Tags = new List<string>();

        //Save current assets
        this.SceneAssets = SceneAssets;
        //Store assets and player transform properties
        Name = GameData.LevelName;
        StoreUserObjects();
        StoreLevelObjects();
        StoreTriggers();
        StoreConditions();
        StoreTags();
        StorePlayerTransform(Player);
    }

    private void StoreUserObjects()
    {
        //Store objectdatas of user objects in scene
        foreach (GameObject Object in SceneAssets.UserObjectsInScene)
        {
            if (Object.activeSelf) 
                Objects.Add(new ParentData(Object));
        }
    }

    private void StoreLevelObjects()
    {
        //Store objectdatas of level objects in scene
        foreach (GameObject Object in SceneAssets.LevelObjectsInScene)
        {
            if (Object.activeSelf)
                LevelObjects.Add(new ParentData(Object));
        }
    }

    private void StoreTriggers()
    {
        //Store triggerdatas of objects in scene
        foreach (Trigger trigger in SceneAssets.TriggersInScene)
        {
            if (trigger.ThisPlane.activeSelf)
                Triggers.Add(new TriggerData(trigger));
        }
    }

    private void StoreConditions()
    {
        //Store conditions of level
        foreach (Condition condition in SceneAssets.LevelConditions)
        {
            Conditions.Add(new ConditionData(condition));
        }
    }
    
    private void StoreTags()
    {
        //Store tags in level
        foreach (string tag in SceneAssets.Tags)
        {
            Tags.Add(tag);
        }
    }

    private void StorePlayerTransform(Transform transform)
    {
        //Store position
        Vector3 position = transform.position;
        PlayerPosX = position.x;
        PlayerPosY = position.y;
        PlayerPosZ = position.z;

        //Store rotation
        Quaternion rotation = transform.rotation;
        PlayerRotX = rotation.x;
        PlayerRotY = rotation.y;
        PlayerRotZ = rotation.z;
        PlayerRotW = rotation.w;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(PlayerPosX, PlayerPosY, PlayerPosZ);
    }

    public Quaternion GetRotation()
    {
        return new Quaternion(PlayerRotX, PlayerRotY, PlayerRotZ, PlayerRotW);
    }
}

[System.Serializable]
public class SceneData1
{
    public string Name;
    public List<ObjectData1> Objects;
    public List<TriggerData1> Triggers;
    public List<ObjectData1> LevelObjects;
    public List<ConditionData> Conditions;
    public List<string> Tags;

    public float PlayerPosX, PlayerPosY, PlayerPosZ;
    public float PlayerRotX, PlayerRotY, PlayerRotZ, PlayerRotW;

    [System.NonSerialized]
    private CurrentAssets SceneAssets;

    public SceneData1(CurrentAssets SceneAssets, Transform Player)
    {
        //Initialise
        Objects = new List<ObjectData1>();
        Triggers = new List<TriggerData1>();
        LevelObjects = new List<ObjectData1>();
        Conditions = new List<ConditionData>();
        Tags = new List<string>();

        //Save current assets
        this.SceneAssets = SceneAssets;
        //Store assets and player transform properties
        Name = GameData.LevelName;
        StoreUserObjects();
        StoreLevelObjects();
        StoreTriggers();
        StoreConditions();
        StoreTags();
        StorePlayerTransform(Player);
    }

    private void StoreUserObjects()
    {
        //Store objectdatas of user objects in scene
        foreach (GameObject Object in SceneAssets.UserObjectsInScene)
        {
            Objects.Add(new ObjectData1(Object));
            Debug.Log(Object.name);
        }
    }

    private void StoreLevelObjects()
    {
        //Store objectdatas of level objects in scene
        foreach (GameObject Object in SceneAssets.LevelObjectsInScene)
        {
            LevelObjects.Add(new ObjectData1(Object));
        }
    }

    private void StoreTriggers()
    {
        //Store triggerdatas of objects in scene
        foreach (Trigger trigger in SceneAssets.TriggersInScene)
        {
            Triggers.Add(new TriggerData1(trigger));
        }
    }

    private void StoreConditions()
    {
        //Store conditions of level
        foreach (Condition condition in SceneAssets.LevelConditions)
        {
            Conditions.Add(new ConditionData(condition));
        }
    }
    
    private void StoreTags()
    {
        //Store tags in level
        foreach (string tag in SceneAssets.Tags)
        {
            Tags.Add(tag);
        }
    }

    private void StorePlayerTransform(Transform transform)
    {
        //Store position
        Vector3 position = transform.position;
        PlayerPosX = position.x;
        PlayerPosY = position.y;
        PlayerPosZ = position.z;

        //Store rotation
        Quaternion rotation = transform.rotation;
        PlayerRotX = rotation.x;
        PlayerRotY = rotation.y;
        PlayerRotZ = rotation.z;
        PlayerRotW = rotation.w;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(PlayerPosX, PlayerPosY, PlayerPosZ);
    }

    public Quaternion GetRotation()
    {
        return new Quaternion(PlayerRotX, PlayerRotY, PlayerRotZ, PlayerRotW);
    }
}
