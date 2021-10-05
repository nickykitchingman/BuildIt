using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LoadBuiltInObjects : MonoBehaviour
{
    private static Dictionary<string, Type> _triggers;
    private static Dictionary<string, Type> Triggers
    {
        get {
            if (_triggers == null)
                InitialiseTriggers();
            return _triggers;
        } set {
            _triggers = value;
        }
    }

    private static Dictionary<string, GameObject> _customObjectsDict;
    public static Dictionary<string, GameObject> CustomObjectsDict
    {
        get {
            if (_customObjectsDict == null)
                InitialiseCustomObjects();
            return _customObjectsDict;

        } private set {
            _customObjectsDict = value;
        }
    }


    private static void InitialiseTriggers()
    {
        //Initialise triggers dictionary
        Triggers = new Dictionary<string, Type>
        {
            { "CollisionPlane", typeof(CollisionPlane) }
        };
    }

    private static void InitialiseCustomObjects()
    {
        //Hold object in dictionary for more appropriate access
        CustomObjectsDict = new Dictionary<string, GameObject>();

        foreach (GameObject CustomObject in Resources.LoadAll("Objects/Shapes", typeof(GameObject)).Cast<GameObject>())
        {
            //Add each value from list with its name
            string Name = CustomObject.name;
            CustomObjectsDict.Add(Name, CustomObject);
        }
    } 

    public static Trigger LoadTrigger(string name)
    {
        Trigger NewTrigger = Activator.CreateInstance(Triggers[name]) as Trigger;
        NewTrigger.Name = NewTrigger.ThisPlane.name = name;

        //Spawn relative to camera
        Ray ray = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        NewTrigger.ThisPlane.transform.position = ray.GetPoint(5);
        //Add properties and set type to trigger
        NewTrigger.ThisPlane.AddComponent<Properties>().SetType("Trigger");
        NewTrigger.ThisPlane.GetComponent<Properties>().IsStatic = true;
        NewTrigger.ThisPlane.GetComponent<Properties>().SetShapeType(NewTrigger.type);
        NewTrigger.ThisPlane.GetComponent<Properties>().BaseType = name;

        return NewTrigger;
    }

    public static GameObject LoadCustom(string name, Element element)
    {
        //Create new object
        GameObject InBuiltObject = Instantiate(MyAssets.CustomObjectsDict[name]);
        //Add mesh collider if missing
        if (InBuiltObject.GetComponent<Collider>() == null)
            InBuiltObject.AddComponent<MeshCollider>().convex = true;
        //Set name with appropriate index
        int index = SceneAssets.currentAssets.GetNameIndex(name);
        InBuiltObject.name = index == 1 ? name : name + (index - 1).ToString();

        //Add material and properties 
        InBuiltObject.GetComponent<Renderer>().material = element.material;
        InBuiltObject.AddComponent<Properties>().SetType("LevelObject");
        InBuiltObject.GetComponent<Properties>().element = element;
        InBuiltObject.GetComponent<Properties>().SetShapeType("Normal");
        InBuiltObject.GetComponent<Properties>().BaseType = name;

        return InBuiltObject;
    }

    public static GameObject Unpack(ParentData CustomObject)
    {
        //Create new base object
        GameObject newObject = LoadCustom(CustomObject.BaseType, CustomObject.element);
        //Set name
        newObject.name = CustomObject.Name;
        //Set position
        newObject.transform.position = CustomObject.Position;
        //Set rotation
        newObject.transform.rotation = CustomObject.Rotation;
        //Set Scale
        if (!ContainsZero(CustomObject.Scale))
            newObject.transform.localScale = CustomObject.Scale;
        //Set properties
        CustomObject.UpdateProperties(newObject.GetComponent<Properties>());
        //Add rigidbody if object is not static
        if (!CustomObject.IsStatic)
        {
            var rb = newObject.AddComponent<Rigidbody>();
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
        }

        return newObject;
    }

    public static Trigger Unpack(TriggerData trigger)
    {
        //Create new base trigger
        Trigger newTrigger = LoadTrigger(trigger.BaseType);
        //Set name
        newTrigger.Name = trigger.Name;
        newTrigger.ThisPlane.name = trigger.Name;
        //Set position
        newTrigger.ThisPlane.transform.position = trigger.Position;
        //Set rotation
        newTrigger.ThisPlane.transform.rotation = trigger.Rotation;
        //Set Scale
        if (!ContainsZero(trigger.Scale))
            newTrigger.ThisPlane.transform.localScale = trigger.Scale;
        //Set properties
        trigger.UpdateProperties(newTrigger.ThisPlane.GetComponent<Properties>());

        return newTrigger;
    }

    public static Condition Unpack(ConditionData condition)
    {
        //Create new base condition
        Condition newCondition = new Condition
        {
            //Add properties
            trigger = SceneAssets.currentAssets.FindTrigger(condition.trigger),
            collisionType = condition.collisionType,
            collider = condition.collider,
            objective = condition.objective
        };

        return newCondition;
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
