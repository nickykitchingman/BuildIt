using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class of all Object Datas
/// </summary>
[System.Serializable]
public class ObjectData
{
    [System.NonSerialized]
    protected GameObject Object;

    private float PosX, PosY, PosZ;
    private float RotX, RotY, RotZ, RotW;
    private float ScaleX, ScaleY, ScaleZ;
    public Element element;
    public string Name;

    //Returns Pos x, y, z as a Vector3
    //Sets Vector3 position as float x, y, z
    public Vector3 Position
    {
        get { return new Vector3(PosX, PosY, PosZ); }
        set { PosX = value.x; PosY = value.y; PosZ = value.z; }
    }
    //Returns Rot x, y, z as a Quaternion
    //Sets Quaternion scale as float x, y, z
    public Quaternion Rotation
    {
        get { return new Quaternion(RotX, RotY, RotZ, RotW); }
        set { RotX = value.x; RotY = value.y; RotZ = value.z; RotW = value.w; }
    }
    //Returns Scale x, y, z as a Vector3
    //Sets Vector3 scale as float x, y, z
    public Vector3 Scale
    {
        get { return new Vector3(ScaleX, ScaleY, ScaleZ); }
        set { ScaleX = value.x; ScaleY = value.y; ScaleZ = value.z; }
    }

    public ObjectData(GameObject gameObject)
    {
        Object = gameObject;
        Position = gameObject.transform.position;
        Rotation = gameObject.transform.rotation;
        StoreProperties(gameObject.GetComponent<Properties>());
    }

    /// <summary>
    /// Store each property
    /// </summary>
    /// <param name="properties"></param>
    protected virtual void StoreProperties(Properties properties)
    {
        if (properties == null)
            return;

        element = properties.element;
        Name = properties.name;
    }

    /// <summary>
    /// Move properties onto new properties
    /// </summary>
    /// <param name="newProperties"></param>
    public virtual void UpdateProperties(Properties newProperties)
    {
        newProperties.element = element;
    }

    /// <summary>
    /// Checks if a vector contains a zero in one of the axis
    /// </summary>
    /// <param name="vector"></param>
    /// <returns></returns>
    protected static bool ContainsZero(Vector3 vector)
    {
        if (vector.x != 0)
            if (vector.y != 0)
                if (vector.z != 0)
                    return false;
        return true;
    }
}

/// <summary>
/// Stores data of standard objects
/// </summary>
[System.Serializable]
public class ParentData : ObjectData
{
    public string Tag, ObjectType, ShapeType, BaseType;
    public bool IsStatic;
    public bool Deactivated;
    public List<ChildData> Children = new List<ChildData>();

    public ParentData(GameObject gameObject) : base(gameObject)
    {
        StoreProperties(gameObject.GetComponent<Properties>());
        StoreChildren();
    }

    /// <summary>
    /// Stores data of children in serializable lists
    /// </summary>
    /// <param name="parent"></param>
    private void StoreChildren()
    {
        foreach (Transform child in Object.transform)
        {
            Children.Add(new ChildData(child.gameObject));
        }
    }

    //Stores all attributes of properties component
    protected override void StoreProperties(Properties properties)
    {
        if (properties == null)
            return;

        Name = properties.name;
        element = properties.element;
        ObjectType = properties.Type;
        ShapeType = properties.ShapeType;
        Tag = properties.Tag;
        BaseType = properties.BaseType;
        IsStatic = properties.IsStatic;
        Deactivated = properties.Deactivated;
        Scale = properties.Scale;
    }

    /// <summary>
    /// Move properties onto new properties
    /// </summary>
    /// <param name="newProperties"></param>
    public override void UpdateProperties(Properties newProperties)
    {
        newProperties.element = element;
        newProperties.IsStatic = IsStatic;
        newProperties.Tag = Tag;
        newProperties.ShapeType = ShapeType;
        newProperties.BaseType = BaseType;
        newProperties.Deactivated = Deactivated;
        if (!ContainsZero(Scale))
            newProperties.Scale = Scale;
    }
}

/// <summary>
/// Stores data of child objects
/// </summary>
[System.Serializable]
public class ChildData : ObjectData
{
    public string BaseType;

    public ChildData(GameObject gameObject) : base(gameObject)
    {
        StoreProperties(gameObject.GetComponent<Properties>());
    }


    /// <summary>
    /// Stores all attributes of properties component
    /// </summary>
    /// <param name="properties"></param>
    protected override void StoreProperties(Properties properties)
    {
        if (properties == null)
            return;

        Name = properties.name;
        element = properties.element;
        Scale = properties.Scale;
        BaseType = properties.BaseType;
    }

    /// <summary>
    /// Move properties onto new properties
    /// </summary>
    /// <param name="newProperties"></param>
    public override void UpdateProperties(Properties newProperties)
    {
        newProperties.name = Name;
        newProperties.BaseType = BaseType;
        newProperties.element = element;
        if (!ContainsZero(Scale))
            newProperties.Scale = Scale;
    }
}

/// <summary>
/// Stores data of triggers
/// </summary>
[System.Serializable]
public class TriggerData : ParentData 
{
    public string Type;

    public TriggerData (Trigger trigger) : base(trigger.ThisPlane)
    {
        Name = trigger.Name;
        Type = trigger.type;

        Position = trigger.ThisPlane.transform.position;
        Rotation = trigger.ThisPlane.transform.rotation;
        StoreProperties(trigger.ThisPlane.GetComponent<Properties>());
    }
}

/// <summary>
/// Stores data of triggers
/// </summary>
[System.Serializable]
public class TriggerData1 : ObjectData1 
{
    public string Type;

    public TriggerData1 (Trigger trigger) : base(trigger.ThisPlane)
    {
        ObjectName = trigger.Name;
        Type = trigger.type;

        StorePosition(trigger.ThisPlane.transform.position);
        StoreRotation(trigger.ThisPlane.transform.rotation);
        StoreProperties(trigger.ThisPlane.GetComponent<Properties>());
    }
}


/// <summary>
/// Stores the serializable components and data of an object
/// </summary>
[System.Serializable]
public class ObjectData1
{
    [System.NonSerialized]
    protected GameObject Object;

    public float PosX, PosY, PosZ;
    public float RotX, RotY, RotZ, RotW;
    public float ScaleX, ScaleY, ScaleZ;
    public string Tag, ObjectType, ShapeType, BaseType;
    public Element element;
    public List<ChildData2> Children;
    public bool IsStatic;
    public bool Deactivated;
    public string ObjectName;

    public Vector3 Scale
    {
        get
        {
            return new Vector3(ScaleX, ScaleY, ScaleZ);
        }
    }

    public ObjectData1(GameObject MyObject)
    {
        Object = MyObject;
        Children = new List<ChildData2>();

        ObjectName = MyObject.name;
        StorePosition(MyObject.transform.position);
        StoreRotation(MyObject.transform.rotation);
        StoreProperties(MyObject.GetComponent<Properties>());
        StoreChildren();
    }

    /// <summary>
    /// Stores vector3 position as three floats
    /// </summary>
    /// <param name="position"></param>
    protected void StorePosition(Vector3 position)
    {
        PosX = position.x;
        PosY = position.y;
        PosZ = position.z;
    }

    /// <summary>
    /// Stores quaternion rotation as four floats
    /// </summary>
    /// <param name="rotation"></param>
    protected void StoreRotation(Quaternion rotation)
    {
        RotX = rotation.x;
        RotY = rotation.y;
        RotZ = rotation.z;
        RotW = rotation.w;
    }

    /// <summary>
    /// Stores vector3 scale as three floats
    /// </summary>
    /// <param name="scale"></param>
    protected void StoreScale(Vector3 scale)
    {
        ScaleX = scale.x;
        ScaleY = scale.y;
        ScaleZ = scale.z;
    }

    /// <summary>
    /// Store each property
    /// </summary>
    /// <param name="properties"></param>
    protected void StoreProperties(Properties properties)
    {
        if (properties == null)
            return;

        element = properties.element;
        ObjectType = properties.Type;
        ShapeType = properties.ShapeType;
        Tag = properties.Tag;
        BaseType = properties.BaseType;
        IsStatic = properties.IsStatic;
        Deactivated = properties.Deactivated;
        StoreScale(properties.Scale);
    }

    /// <summary>
    /// Move properties onto new properties
    /// </summary>
    /// <param name="newProperties"></param>
    public void UpdateProperties(Properties newProperties)
    {
        newProperties.element = element;
        newProperties.IsStatic = IsStatic;
        newProperties.Tag = Tag;
        newProperties.ShapeType = ShapeType;
        newProperties.BaseType = BaseType;
        newProperties.Deactivated = Deactivated;
        if (!ContainsZero(Scale))
            newProperties.Scale = Scale;
    }

    /// <summary>
    /// Stores data of children in serializable lists
    /// </summary>
    /// <param name="parent"></param>
    private void StoreChildren()
    {
        foreach (Transform child in Object.transform)
        {
            if (child.CompareTag("Cube"))
            {
                Children.Add(new CubeData(child.gameObject));
            }

            else if (child.CompareTag("Sphere"))
            {
                Children.Add(new SphereData(child.gameObject));
            }

            else if (child.CompareTag("Cylinder"))
            {
                Children.Add(new CylinderData(child.gameObject));
            }

            else
                Children.Add(new ChildData2(child.gameObject));
        }
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


[System.Serializable]
public class ChildData2
{
    [System.NonSerialized]
    protected GameObject Child;

    public float PosX, PosY, PosZ;
    public float ScaleX, ScaleY, ScaleZ;
    public float RotX, RotY, RotZ, RotW;
    public Element element;
    public string type;
    public Vector3 Scale
    {
        get
        {
            return new Vector3(ScaleX, ScaleY, ScaleZ);
        }
    }

    public ChildData2(GameObject MyChild)
    {
        Child = MyChild;
        type = MyChild.tag;

        StorePosition(MyChild.transform.position);
        StoreScale(MyChild.transform.localScale);
        StoreRotation(MyChild.transform.rotation);
        StoreProperties(MyChild.GetComponent<Properties>());

        StoreElement();
    }

    /// <summary>
    /// Stores object's element
    /// </summary>
    protected void StoreElement()
    {
        element = Child.GetComponent<Properties>().element;
    }

    /// <summary>
    /// Stores vector3 position relative to parent object as three floats
    /// </summary>
    /// <param name="position"></param>
    protected void StorePosition(Vector3 position)
    {
        PosX = position.x;
        PosY = position.y;
        PosZ = position.z;
    }

    /// <summary>
    /// Stores vector3 scale as three floats
    /// </summary>
    /// <param name="scale"></param>
    protected void StoreScale(Vector3 scale)
    {
        ScaleX = scale.x;
        ScaleY = scale.y;
        ScaleZ = scale.z;
    }

    /// <summary>
    /// Stores quaternion rotation as four floats
    /// </summary>
    /// <param name="rotation"></param>
    protected void StoreRotation(Quaternion rotation)
    {
        RotX = rotation.x;
        RotY = rotation.y;
        RotZ = rotation.z;
        RotW = rotation.w;
    }

    /// <summary>
    /// Store each property
    /// </summary>
    /// <param name="properties"></param>
    protected void StoreProperties(Properties properties)
    {
        if (properties == null)
            return;

        type = properties.BaseType;
        StoreScale(properties.Scale);
    }

    /// <summary>
    /// Move properties onto new properties
    /// </summary>
    /// <param name="newProperties"></param>
    public void UpdateProperties(Properties newProperties)
    {
        newProperties.element = element;
        newProperties.BaseType = type;
        if (!ContainsZero(Scale))
            newProperties.Scale = Scale;
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

[System.Serializable]
public class SphereData : ChildData2
{
    public SphereData(GameObject MySphere) : base(MySphere)
    {
        StorePosition(Child.transform.position);
        StoreScale(Child.transform.localScale);
        StoreRotation(Child.transform.rotation);

        StoreElement();
    }
}

[System.Serializable]
public class CubeData : ChildData2
{
    public CubeData(GameObject MyCube) : base(MyCube)
    {
        StorePosition(Child.transform.position);
        StoreScale(Child.transform.localScale);
        StoreRotation(Child.transform.rotation);

        StoreElement();
    }
}

[System.Serializable]
public class CylinderData : ChildData2
{
    public CylinderData(GameObject MyCylinder) : base(MyCylinder)
    {
        StorePosition(Child.transform.position);
        StoreScale(Child.transform.localScale);
        StoreRotation(Child.transform.rotation);

        StoreElement();
    }
}

/*

[System.Serializable]
public class CubeData
{
    [System.NonSerialized]
    protected GameObject Cube;

    public float PosX, PosY, PosZ;
    public float ScaleX, ScaleY, ScaleZ;
    public IndividualElement element;

    public List<object> ElementProperties;
    

    public CubeData(GameObject MyCube)
    {
        Cube = MyCube;
        ElementProperties = new List<object>();

        StorePosition(MyCube.transform.position);
        StoreScale(MyCube.transform.localScale);

        StoreElement();
    }

    /// <summary>
    /// Stores properties of cube's element in a list
    /// </summary>
    private void StoreElement()
    {
        element = Cube.GetComponent<Properties>().element;
    }

    /// <summary>
    /// Stores vector3 position relative to parent object as three floats
    /// </summary>
    /// <param name="position"></param>
    private void StorePosition(Vector3 position)
    {
        PosX = position.x;
        PosY = position.y;
        PosZ = position.z;
    }

    /// <summary>
    /// Stores vector3 scale as three floats
    /// </summary>
    /// <param name="scale"></param>
    private void StoreScale(Vector3 scale)
    {
        ScaleX = scale.x;
        ScaleY = scale.y;
        ScaleZ = scale.z;
    }
}*/
