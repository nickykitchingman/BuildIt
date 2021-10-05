using UnityEngine;

public class Properties : MonoBehaviour
{
    public Element element;

    public string Tag;

    /// <summary>
    /// Type of shape (normal, plane etc)
    /// </summary>
    public string ShapeType;

    /// <summary>
    /// Classification of object (UserObject, LevelObject, Trigger)
    /// </summary>
    public string Type;

    /// <summary>
    /// Primitive name (cube etc)
    /// </summary>
    public string BaseType;

    public bool IsStatic;

    public Vector3 Scale = Vector3.one;

    public bool Deactivated;

    public bool SetType(string type)
    {
        //Sets if valid type
        string[] validTypes = new string[] { "UserObject", "LevelObject", "Trigger" };
        foreach (string validType in validTypes)
            if (type == validType)
            {
                Type = type;
                return true;
            }
        Debug.Log("Invalid object type");
        return false;
    }

    public bool SetShapeType(string shapeType)
    {
        //Sets if valid type
        string[] validTypes = new string[] { "Normal", "Plane" };
        foreach (string validType in validTypes)
            if (shapeType == validType)
            {
                ShapeType = shapeType;
                return true;
            }
        Debug.Log("Invalid object type");
        return false;
    }

    void OnDisable()
    {
        Deactivated = true;
    }

    void OnEnable()
    {
        Deactivated = false;
    }
}
