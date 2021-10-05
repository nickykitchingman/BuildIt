using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class HandleUserObjects
{
    private static List<ParentData> _objectDatas;
    private static List<string> _objectNames;
    
    public static IList<ParentData> ObjectDatas
    {
        get
        {
            LoadObjectsIntoList();
            return _objectDatas;
        }
    }

    public static IList<string> ObjectNames
    {
        get
        {
            LoadObjectsIntoList();
            return _objectNames;
        }
    }

    private static void LoadObjectsIntoList()
    {
        //Initialise names and load object datas
        List<string> Names = new List<string>();
        _objectDatas = GetListOfObjectDatas();

        //Add object names into list
        foreach (ParentData objectData in _objectDatas)
        {
            Names.Add(objectData.Name);
        }

        _objectNames = Names;
    }

    private static List<ParentData> GetListOfObjectDatas()
    {
        string FolderDestination = Application.persistentDataPath + "/Objects/";

        List<ParentData> objectDatas = new List<ParentData>();

        if (Directory.Exists(FolderDestination))
        {
            //Add file destinations, object datas and names to lists
            foreach (string myfile in Directory.EnumerateFiles(FolderDestination, "*.bytes"))
            {
                ParentData FileObjectData = FileHandler.LoadObject(myfile);
                objectDatas.Add(FileObjectData);
            }
        }

        return objectDatas;
    }

    public static GameObject LoadObject(ParentData objectData)
    {
        //Create new object
        //Give appropriate index
        int index = SceneAssets.currentAssets.GetNameIndex(objectData.Name);
        GameObject NewObject = new GameObject
        {
            name = objectData.Name + (index == 1 ? "" : (index - 1).ToString())
        };

        //Add basic components
        NewObject.AddComponent<MeshRenderer>();
        NewObject.AddComponent<Rigidbody>().isKinematic = !PhysicsControl.IsPlaying;

        //Update position
        NewObject.transform.position = objectData.Position;
        //Update rotation
        NewObject.transform.rotation = objectData.Rotation;


        //Recreate each child
        foreach (ChildData Child in objectData.Children)
        {
            GameObject newChild = GameObject.Instantiate(MyAssets.CustomObjectsDict[Child.BaseType]);
            newChild = LoadChildProperties(newChild, Child);
            newChild.transform.parent = NewObject.transform;
        }

        ////Attach each child to one another
        //Rigidbody PreviousChild = null;
        //Rigidbody[] rigidbodies = NewObject.GetComponentsInChildren<Rigidbody>();
        //if (rigidbodies.Length > 0)
        //    PreviousChild = rigidbodies[0];
        ////Add joints to all children
        //for (int i = 1; i < rigidbodies.Length; i++)
        //{
        //    rigidbodies[i].gameObject.AddComponent<FixedJoint>().connectedBody = PreviousChild;
        //    PreviousChild = rigidbodies[i];
        //}

        ////Ignore child collisions with each other
        //Collider[] colliders = NewObject.GetComponentsInChildren<Collider>();
        ////Add joints to all children
        //for (int i = 1; i < colliders.Length; i++)
        //{
        //    foreach (Collider collider in colliders)
        //        Physics.IgnoreCollision(colliders[i], collider);
        //}

        //Merge object together
        NewObject.AddComponent<MeshFilter>().mesh = MergeObject(NewObject);
        NewObject.AddComponent<MeshCollider>().convex = true;
        NewObject.GetComponent<MeshCollider>().isTrigger = true;
        //Add properties
        NewObject.AddComponent<Properties>().SetType("UserObject");
        objectData.UpdateProperties(NewObject.GetComponent<Properties>());
        if (objectData.Deactivated)
            NewObject.SetActive(false);
        //Update scale
        Vector3 scale = NewObject.GetComponent<Properties>().Scale;
        if (ContainsZero(scale))
            NewObject.GetComponent<Properties>().Scale = Vector3.one;
        NewObject.transform.localScale = scale;

        Object.Destroy(NewObject.GetComponent<Renderer>());
        return NewObject;
    }

    private static GameObject LoadChildProperties(GameObject newChild, ChildData ChildData)
    {
        Object.Destroy(newChild.GetComponent<Collider>());
        //Set child to trigger
        newChild.AddComponent<MeshCollider>().convex = true;

        //newChild.GetComponent<MeshCollider>().isTrigger = true;
        //newChild.layer = 2;

        //Add rigidbody
        //newChild.AddComponent<Rigidbody>();

        //Update position
        newChild.transform.localPosition = ChildData.Position;
        //Set Scale
        if (!ContainsZero(ChildData.Scale))
            newChild.transform.localScale = ChildData.Scale;
        //Update rotation
        newChild.transform.rotation = ChildData.Rotation;
        
        newChild.AddComponent<Properties>();
        ChildData.UpdateProperties(newChild.GetComponent<Properties>());

        //Add material
        newChild.GetComponent<Renderer>().material = ChildData.element.material;
;
        return newChild;
    }

    /// <summary>
    /// Returns combined mesh of object
    /// </summary>
    /// <param name="Object"></param>
    /// <returns></returns>
    public static Mesh MergeObject(GameObject Object)
    {
        //Set object to zero-zero so children are relative to 0
        Quaternion OrglRot = Object.transform.rotation;
        Vector3 OrglPos = Object.transform.position;
        Object.transform.rotation = Quaternion.identity;
        Object.transform.position = Vector3.zero;

        MeshFilter[] MeshFilters = Object.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] InstanceCombiners = new CombineInstance[MeshFilters.Length];

        //Create InstanceCombiner for each object
        for (int i = 0; i < MeshFilters.Length; i++)
        {
            InstanceCombiners[i].subMeshIndex = 0;
            InstanceCombiners[i].mesh = MeshFilters[i].sharedMesh;
            InstanceCombiners[i].transform = MeshFilters[i].transform.localToWorldMatrix;
        }

        //Combine InstanceCombiners
        Mesh FinalMesh = new Mesh();
        FinalMesh.CombineMeshes(InstanceCombiners);
        //Object.GetComponent<MeshFilter>().sharedMesh = FinalMesh;

        //Reset object
        Object.transform.SetPositionAndRotation(OrglPos, OrglRot);

        //return FinalMesh;
        return FinalMesh;
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
