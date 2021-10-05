using UnityEngine;

[System.Serializable]
public struct Elements
{
    public static Element Aluminium = new Element("Aluminium", 2710f);
    public static Element Carbon = new Element("Carbon", 1000f);
    public static Element Copper = new Element("Copper", 8960f);
    public static Element Rubber = new Element("Rubber", 1522f);
    public static Element Steel = new Element("Steel", 7700f);
    public static Element Wood = new Element("Wood", 1f);
    public static Element Stone = new Element("Stone", 2250f);
}

[System.Serializable]
public struct Element
{
    public readonly string Name;
    public readonly float Density;

    //Return the material of the element using its name
    public Material material
    {
        get
        {
            string NameCopy = Name;
            return MyAssets.ElementMaterials.Find(f => f.name == NameCopy);
        }
    }

    public Element(string name, float density)
    {
        Name = name;
        Density = density;
    }
}

/*
[System.Serializable]
public class Element 
{
    [System.NonSerialized]
    GameObject gameObject;

    public float MaterialID;
    public float Density;

    public Element(GameObject NewGameObject)
    {
        gameObject = NewGameObject;
    }

    public void SetMetal()
    {
        //Set material
        Material material = Resources.Load("Materials/ElementMats/Metal") as Material;
        gameObject.GetComponent<Renderer>().material = material;
        //Other properties
        MaterialID = 1f;
        Density = 8000f;
    }
}
*/
