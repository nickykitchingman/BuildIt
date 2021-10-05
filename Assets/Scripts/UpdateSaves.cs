using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class UpdateSaves
{

    private static GameObject LoadUserObject(ObjectData1 oldData)
    {
        //Load object using old format
        GameObject SavedObject = new GameObject();
        SavedObject.name = SavedObject.name;

        //Update position
        SavedObject.transform.position = new Vector3(oldData.PosX, oldData.PosY, oldData.PosZ);
        //Update rotation
        SavedObject.transform.rotation = new Quaternion(oldData.RotX, oldData.RotY, oldData.RotZ, oldData.RotW);

        //Recreate each child
        foreach (ChildData2 Child in oldData.Children)
        {
            //GameObject newChild = GameObject.CreatePrimitive(PrimitiveObjects[Child.type]);
            GameObject newChild = UnityEngine.Object.Instantiate(MyAssets.CustomObjectsDict[Child.type]);
            newChild = LoadChildProperties(newChild, Child);
            newChild.transform.parent = SavedObject.transform;
        }

        //Add properties
        SavedObject.AddComponent<Properties>().SetType("UserObject");
        oldData.UpdateProperties(SavedObject.GetComponent<Properties>());
        if (oldData.Deactivated)
            SavedObject.SetActive(false);
        //Update scale
        Vector3 scale = SavedObject.GetComponent<Properties>().Scale;
        if (ContainsZero(scale))
            SavedObject.GetComponent<Properties>().Scale = Vector3.one;
        SavedObject.transform.localScale = scale;

        return SavedObject;
    }

    private static GameObject LoadLevelObject(ObjectData1 LevelObject)
    {
        Debug.Log("loading level object");
        //Create new base object
        GameObject newObject = LoadBuiltInObjects.LoadCustom(LevelObject.BaseType, LevelObject.element);
        //Set name
        newObject.name = LevelObject.ObjectName;
        //Set position
        newObject.transform.position = new Vector3(LevelObject.PosX, LevelObject.PosY, LevelObject.PosZ);
        //Set rotation
        newObject.transform.rotation = new Quaternion(LevelObject.RotX, LevelObject.RotY, LevelObject.RotZ, LevelObject.RotW);
        //Set Scale
        if (!ContainsZero(LevelObject.Scale))
            newObject.transform.localScale = LevelObject.Scale;
        //Set properties
        LevelObject.UpdateProperties(newObject.GetComponent<Properties>());

        return newObject;
    }

    private static Trigger LoadTrigger(TriggerData1 trigger)
    {
        //Create new base trigger
        Trigger newTrigger = LoadBuiltInObjects.LoadTrigger(trigger.BaseType);
        //Set name
        newTrigger.Name = trigger.ObjectName;
        newTrigger.ThisPlane.name = trigger.ObjectName;
        //Set position
        newTrigger.ThisPlane.transform.position = new Vector3(trigger.PosX, trigger.PosY, trigger.PosZ);
        //Set rotation
        newTrigger.ThisPlane.transform.rotation = new Quaternion(trigger.RotX, trigger.RotY, trigger.RotZ, trigger.RotW);
        //Set Scale
        if (!ContainsZero(trigger.Scale))
            newTrigger.ThisPlane.transform.localScale = trigger.Scale;
        //Set properties
        trigger.UpdateProperties(newTrigger.ThisPlane.GetComponent<Properties>());

        return newTrigger;
    }

    public static void UpdateObjectSave(string path)
    {
        FileStream file;

        //Open file if it exists, end method if not
        if (File.Exists(path))
            file = File.OpenRead(path);
        else
            return;

        //Deserialize object data from file
        BinaryFormatter bf = new BinaryFormatter();
        ObjectData1 oldData;
        try
        {
            oldData = (ObjectData1)bf.Deserialize(file);
        }
        catch (InvalidCastException) { file.Close(); return; }

        //Update object into new data format
        ParentData newData = new ParentData(LoadUserObject(oldData));
        file.Close();
        
        file = File.OpenWrite(path);
        bf.Serialize(file, newData);
        file.Close();
    }

    public static void UpdateLevelSave(string path)
    {
        FileStream file;

        //Open file if it exists, end method if not
        if (File.Exists(path))
            file = File.OpenRead(path);
        else
            return;

        //Deserialize object data from file
        BinaryFormatter bf = new BinaryFormatter();
        //SceneData oldLevelData2 = (SceneData)bf.Deserialize(file);
        SceneData1 oldLevelData = (SceneData1)bf.Deserialize(file);

        CurrentAssets assets = new CurrentAssets();
        GameObject NewObject = new GameObject();
        foreach (ObjectData1 UserObject in oldLevelData.Objects)
        {
            NewObject = LoadUserObject(UserObject);
            //Add to current assets
            SceneAssets.currentAssets.AddUserObject(NewObject);
        }

        //Load each level object
        foreach (ObjectData1 LevelObject in oldLevelData.LevelObjects)
        {
            //Load in every level object
            NewObject = LoadLevelObject(LevelObject);
            //Add to current assets
            SceneAssets.currentAssets.AddLevelObject(NewObject);
        }

        //Load each trigger
        foreach (TriggerData1 trigger in oldLevelData.Triggers)
        {
            //Load in every trigger
            Trigger NewTrigger = LoadTrigger(trigger);
            //Add to current assets
            SceneAssets.currentAssets.AddTrigger(NewTrigger);
        }

        //Add conditions to conditions view
        foreach (ConditionData condition in oldLevelData.Conditions)
        {
            //Create new objective graphic for each condition
            Condition NewCondition = LoadBuiltInObjects.Unpack(condition);
            //Add to current assets
            SceneAssets.currentAssets.LevelConditions.Add(NewCondition);
        }

        //Add tags to scene assets
        if (oldLevelData.Tags != null)
            foreach (string tag in oldLevelData.Tags)
                SceneAssets.currentAssets.Tags.Add(tag);

        //Set player position and rotation
        Transform player = new GameObject().transform;
        player.position = oldLevelData.GetPosition();
        player.rotation = oldLevelData.GetRotation();

        //Save new scene
        SceneData newData = new SceneData(assets, player);
    }

    private static GameObject LoadChildProperties(GameObject newChild, ChildData2 ChildData)
    {
        UnityEngine.Object.Destroy(newChild.GetComponent<Collider>());

        //Update position
        newChild.transform.localPosition = new Vector3(ChildData.PosX, ChildData.PosY, ChildData.PosZ);
        //Set Scale
        if (!ContainsZero(ChildData.Scale))
            newChild.transform.localScale = ChildData.Scale;
        //Update rotation
        newChild.transform.rotation = new Quaternion(ChildData.RotX, ChildData.RotY, ChildData.RotZ, ChildData.RotW);

        newChild.AddComponent<Properties>();
        ChildData.UpdateProperties(newChild.GetComponent<Properties>());

        //Add material
        //newChild.GetComponent<Renderer>().material = MyAssets.ElementMaterials[ChildData.element.MaterialID];
        newChild.GetComponent<Renderer>().material = ChildData.element.material;
        return newChild;
    }

    private static bool ContainsZero(Vector3 vector)
    {
        if (vector.x != 0)
            if (vector.y != 0)
                if (vector.z != 0)
                    return false;
        return true;
    }
}
