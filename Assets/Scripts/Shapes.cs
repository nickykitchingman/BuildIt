using UnityEngine;

public class Shapes : MonoBehaviour
{
    
    public GameObject GroupObject;
    
    private Vector3 NewPosition = Vector3.zero;
    
    public  GameObject LoadShape(string name, Element element)
    {
        //load shape from myassets
        GameObject Shape = Instantiate(MyAssets.CustomObjectsDict[name]);
        Shape.name = name;
        //Add mesh collider if missing
        if (Shape.GetComponent<Collider>() == null)
            Shape.AddComponent<MeshCollider>().convex = true;

        Shape.GetComponent<Transform>().transform.position = NewPosition;

        //Set element of object
        Shape.AddComponent(typeof(Properties));
        Shape.GetComponent<Properties>().element = element;
        Shape.GetComponent<Properties>().BaseType = name;
        Shape.GetComponent<Properties>().Type = "Shape";
        Shape.GetComponent<Renderer>().material = element.material;

        //Update next position
        NewPosition = new Vector3(NewPosition.x + 00.1f, NewPosition.y + 00.1f, NewPosition.z + 0.01f);
        //Set cube as child of object
        Shape.transform.SetParent(GroupObject.transform);

        return Shape;
    }
}