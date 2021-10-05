using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class MyAssets
{
    public static string LevelPath = Application.persistentDataPath + "/Levels/";
    public static string ObjectPath = Application.persistentDataPath + "/Objects/";
    public static string UserDataPath = Application.persistentDataPath + "/UserData/";
    public static List<Material> _elementMaterials;
    public static List<Texture> _elementTextures;
    public static Material _collisionMaterial;

    public static List<Material> ElementMaterials
    {
        get
        {
            if (_elementMaterials == null)
                _elementMaterials = Resources.LoadAll("Materials/ElementMats", typeof(Material)).Cast<Material>().ToList();
            return _elementMaterials;
        }
    }

    public static List<Texture> ElementTextures
    {
        get
        {
            if (_elementTextures == null)
                _elementTextures = Resources.LoadAll("Materials/ElementMats", typeof(Texture)).Cast<Texture>().ToList();
            return _elementTextures;
        }
    }
    public static Material CollisionMaterial
    {
        get
        {

            if (_collisionMaterial == null)
                _collisionMaterial = Resources.Load("Materials/BuiltInMats/Collision") as Material;
            return _collisionMaterial;
        }
    }


    private static Dictionary<string, GameObject> _customObjectsDict;
    public static Dictionary<string, GameObject> CustomObjectsDict
    {
        get
        {
            if (_customObjectsDict == null)
                InitialiseCustomObjects();
            return _customObjectsDict;

        }
        private set
        {
            _customObjectsDict = value;
        }
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
}

public static class SceneAssets
{
    public static CurrentAssets currentAssets = new CurrentAssets();
}

public struct Condition
{
    public Trigger trigger;
    public string collisionType;
    public string collider;
    public string objective;
    public bool complete;
}

[System.Serializable]
public struct ConditionData
{
    public string trigger;
    public string collisionType;
    public string collider;
    public string objective;
    public bool complete;

    public ConditionData(Condition condition)
    {
        //Store properties of condition in serializable form
        trigger = condition.trigger.Name;
        collisionType = condition.collisionType;
        collider = condition.collider;
        objective = condition.objective;
        complete = condition.complete;
    }
}

public class Trigger
{
    public GameObject ThisPlane { get; set; }
    public string Name;
    public string type;
}
public class CollisionPlane : Trigger
{
    public CollisionPlane()
    {
        //Create a primitive plane to specialise
        ThisPlane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        ThisPlane.GetComponent<Renderer>().material = MyAssets.CollisionMaterial;
        ThisPlane.GetComponent<MeshCollider>().convex = true;
        ThisPlane.GetComponent<MeshCollider>().isTrigger = true;
        ThisPlane.tag = "Collision";

        //Set the trigger type to a plane
        type = "Plane";
    }
}

public class ObjectReference
{
    private string Type;
    private int Index;

    public ObjectReference(string type, int index)
    {
        Type = type;
        Index = index;
    }

    public GameObject target { get
        {
            switch (Type)
            {
                case "UserObject":
                    return SceneAssets.currentAssets.UserObjectsInScene[Index];
                case "LevelObject":
                    return SceneAssets.currentAssets.LevelObjectsInScene[Index];
                case "Trigger":
                    return SceneAssets.currentAssets.TriggersInScene[Index].ThisPlane;
                case "Shape":
                    return GameData.CurrentChildObjects[Index];
            }
            return null;
        } }
}

/// <summary>
/// Handles user actions for undo and redo functionality
/// </summary>
public class ActionStack
{
    //Two stacks to hold completed actions and undone actions
    private Stack<GameAction> CompletedActions;
    private Stack<GameAction> UndoneActions;
    public ActionStack()
    {
        //Initialise stacks
        CompletedActions = new Stack<GameAction>();
        UndoneActions = new Stack<GameAction>();
    }

    /// <summary>
    /// Add new action 
    /// </summary>
    /// <param name="action"></param>
    public void NewAction(GameAction action)
    {
        //Add to completed actions
        CompletedActions.Push(action);
        //Clear redo stack
        for (int i = 0; i < UndoneActions.Count; i++)
        {
            GameAction undone = UndoneActions.Pop();
            //Destroy all inactive objects in undone stack by create action references
            if (undone.ActionType == "Create")
            {
                Object.Destroy(undone.Target.target);
            }
        }
    }

    /// <summary>
    /// Undo last action
    /// </summary>
    public void Undo()
    {
        if (CompletedActions.Count != 0)
        {
            //Remove from completed actions
            GameAction action = CompletedActions.Pop();
            //Undo action
            action.Undo();
            //Add to undoneactions
            UndoneActions.Push(action);
        }
    }

    /// <summary>
    /// Redo last undone action
    /// </summary>
    public void Redo()
    {
        if (UndoneActions.Count != 0)
        {
            //Remove from undone actions
            GameAction action = UndoneActions.Pop();
            //Redo
            action.Redo();
            //Add to completed actions
            CompletedActions.Push(action);
        }
    }
}

/// <summary>
/// Holds an action completed by the user
/// </summary>
public abstract class GameAction
{
    public ObjectReference Target;

    public string ActionType;

    public GameAction(ObjectReference target)
    {
        Target = target;
    }

    public abstract void Undo();

    public abstract void Redo();
}

public class Move : GameAction
{    
    public Vector3 OriginalPosition;
    public Vector3 NewPosition; 

    public Move(ObjectReference target) : base(target)
    {
        ActionType = "Move";
    }

    public override void Undo()
    {
        Target.target.transform.position = OriginalPosition;
    }

    public override void Redo()
    {
        Target.target.transform.position = NewPosition;
    }
}

public class Rotate : GameAction
{
    public Quaternion OriginalRotation;
    public Quaternion NewRotation;

    public Rotate(ObjectReference target) : base(target)
    {
        ActionType = "Rotate";
    }

    public override void Undo()
    {
        Target.target.transform.rotation = OriginalRotation;
    }

    public override void Redo()
    {
        Target.target.transform.rotation = NewRotation;
    }
}

public class Scale : GameAction
{
    public Vector3 OriginalScale;
    public Vector3 NewScale;

    public Scale(ObjectReference target) : base(target)
    {
        ActionType = "Scale";
    }

    public override void Undo()
    {
        Target.target.transform.localScale = OriginalScale;
    }

    public override void Redo()
    {
        Target.target.transform.localScale = NewScale;
    }
}

public class Create : GameAction
{
    public Create(ObjectReference target) : base(target)
    {
        ActionType = "Create";
    }

    public override void Undo()
    {
        Target.target.SetActive(false);
        //Object.Destroy(Target);
    }

    public override void Redo()
    {
        Target.target.SetActive(true);
        //Target = LoadObjects.LoadObject(TargetData);
    }
}

public class Delete : GameAction
{
    public Delete(ObjectReference target) : base(target)
    {
        ActionType = "Delete";
    }

    public override void Undo()
    {
        Target.target.SetActive(true);
        //Target = LoadObjects.LoadObject(TargetData);
    }

    public override void Redo()
    {
        Target.target.SetActive(false);
        //Object.Destroy(Target);
    }
}

public class ModifyTransform : GameAction
{
    private Move MoveAction;
    private Rotate RotateAction;
    private Scale ScaleAction;

    public ModifyTransform(ObjectReference target) : base(target)
    {
        ActionType = "ModifyTransform";
    }

    public void SetMove(Vector3 Original, Vector3 New)
    {
        MoveAction = new Move(Target)
        {
            OriginalPosition = Original,
            NewPosition = New
        };
    }

    public void SetRotation(Quaternion Original, Quaternion New)
    {
        RotateAction = new Rotate(Target)
        {
            OriginalRotation = Original,
            NewRotation = New
        };
    }

    public void SetScale(Vector3 Original, Vector3 New)
    {
        ScaleAction = new Scale(Target)
        {
            OriginalScale = Original,
            NewScale = New
        };
    }

    public override void Undo()
    {
        MoveAction.Undo();
        RotateAction.Undo();
        ScaleAction.Undo();
    }

    public override void Redo()
    {
        MoveAction.Redo();
        RotateAction.Redo();
        ScaleAction.Redo();
    }
}