using UnityEngine;
using UnityEngine.EventSystems;

public class SelectObjects : MonoBehaviour
{
    public float Distance = 5f;
    public LayerMask Selection;
    public Notification notification;

    [Header("Scripts")]
    public PreRenderObject preRenderObject;

    private Ray SelectionDetection;
    private GameObject CurrentObject;
    private CurrentAssets currentAssets;

    void Start()
    {
        //Add reference to the current assets
        currentAssets = SceneAssets.currentAssets;
    }

    void Update()
    {
        //Change between is moving and interact mode
        if (GameData.IsMoving)
            SelectionDetection = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2 - 1));
        else
            SelectionDetection = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (GameData.IsRenderingOld)
        {
            //Confirm object load
            if (Input.GetKeyDown(KeyCode.Return) && preRenderObject.IsValid)
            {
                ConfirmObject();
            }

            //Cancel object load
            if (Input.GetMouseButtonDown(1))
            {
                preRenderObject.Activate(false);
                CurrentObject.SetActive(true);
                GameData.IsRenderingOld = false;
            }

            //Delete object
            if (Input.GetKeyDown(KeyCode.Delete))
            {
                string type = CurrentObject.GetComponent<Properties>().Type;
                int index = 0;
                //Remove from current assets
                switch (type)
                {
                    case "UserObject":
                        //currentAssets.RemoveObject(CurrentObject);
                        index = currentAssets.UserObjectsInScene.IndexOf(CurrentObject);
                        break;
                    case "LevelObject":
                        //currentAssets.RemoveLevelObject(CurrentObject);
                        index = currentAssets.LevelObjectsInScene.IndexOf(CurrentObject);
                        break;
                    case "Trigger":
                        foreach (Trigger trigger in currentAssets.TriggersInScene)
                            if (trigger.ThisPlane == CurrentObject)
                            {
                                //currentAssets.RemoveTrigger(trigger);
                                index = currentAssets.TriggersInScene.IndexOf(trigger);
                            }
                        break;
                    case "Shape":
                        for (int i = 0; i < GameData.CurrentChildObjects.Count; i++)
                        {
                            GameObject child = GameData.CurrentChildObjects[i];
                            if (child == CurrentObject)
                                index = i;
                        }
                        break;
                }

                //Destroy object
                CurrentObject.SetActive(false);
                //Destroy(CurrentObject);
                //Stop rendering
                preRenderObject.Activate(false);
                GameData.IsRenderingOld = false;

                //Create reference to object for across scenes save in undo stack
                ObjectReference reference = new ObjectReference(type, index);
                //Add to level actions
                switch (GameData.Mode)
                {
                    case "world":
                    case "levelplay":
                        GameData.LevelActions.NewAction(new Delete(reference));
                        break;

                    case "editor":
                        GameData.EditorActions.NewAction(new Delete(reference));
                        break;
                    
                    case "leveldesign":
                        GameData.DesignActions.NewAction(new Delete(reference));
                        break;
                }
            }
        }
        else
        {
            //Select object
            if ((Input.GetKeyDown(KeyCode.E) && GameData.IsMoving) || (Input.GetMouseButtonDown(0) && !GameData.IsMoving && !GameData.IsRenderingNew))
                if (EventSystem.current.currentSelectedGameObject == null && GameData.CanSelect)
                {
                    RaycastHit Hit;
                    if (Physics.Raycast(SelectionDetection, out Hit, Distance, Selection))
                    {
                    Debug.Log("select");
                        if (Hit.collider.transform.parent.gameObject.layer == 8)
                            CurrentObject = Hit.collider.gameObject;
                        else
                            CurrentObject = Hit.collider.transform.parent.gameObject;

                        if (GameData.Mode == "levelplay" && !(CurrentObject.GetComponent<Properties>().Type == "UserObject"))
                        {
                            notification.Notify("Cannot select level objects", Color.magenta);
                        }
                        else
                        {
                            //Load new object into render script
                            preRenderObject.UpdateObject(CurrentObject);

                            //Set new object to inactive, then render it
                            CurrentObject.SetActive(false);
                            preRenderObject.Activate(true);
                            GameData.IsRenderingOld = true;
                        }
                    }
                }
        }
    }

    private void ConfirmObject()
    {
        //Add to level actions
        //Get object type to be used in object reference
        string type = CurrentObject.GetComponent<Properties>().Type;
        int index = 0;
        //Find index of object in current assets to be used in object reference
        switch (type)
        {
            case "UserObject":
                index = currentAssets.UserObjectsInScene.IndexOf(CurrentObject);
                break;
            case "LevelObject":
                index = currentAssets.LevelObjectsInScene.IndexOf(CurrentObject);
                break;
            case "Trigger":
                for (int i = 0; i < currentAssets.TriggersInScene.Count;i++)
                    if (currentAssets.TriggersInScene[i].ThisPlane == CurrentObject)
                    {
                        index = i;
                    }
                break;
            case "Shape":
                for (int i = 0; i < GameData.CurrentChildObjects.Count; i++)
                {
                    GameObject child = GameData.CurrentChildObjects[i];
                    if (child == CurrentObject)
                        index = i;
                }
                break;
        }
        //Create reference to object to be used in undo across scenes
        ObjectReference reference = new ObjectReference(type, index);
        //Add transform modification action to completed stack
        ModifyTransform action = new ModifyTransform(reference);
        action.SetMove(CurrentObject.transform.position, preRenderObject.GetPosition());
        action.SetRotation(CurrentObject.transform.rotation, preRenderObject.GetRotation());
        action.SetScale(CurrentObject.GetComponent<Properties>().Scale, preRenderObject.GetScale());
        switch (GameData.Mode)
        {
            case "levelplay":
            case "world":
                GameData.LevelActions.NewAction(action);
                break;
            case "editor":
                GameData.EditorActions.NewAction(action);
                break;
            case "leveldesign":
                GameData.DesignActions.NewAction(action);
                break;
        }

        //Activate object again, update position, rotation and scale
        CurrentObject.SetActive(true);
        CurrentObject.transform.position = preRenderObject.GetPosition();
        CurrentObject.transform.rotation = preRenderObject.GetRotation();
        CurrentObject.GetComponent<Properties>().IsStatic = preRenderObject.RenderOptions.IsStatic;
        //Update scale
        Vector3 scale = preRenderObject.GetScale();
        CurrentObject.GetComponent<Properties>().Scale = scale;
        CurrentObject.transform.localScale = scale;
        //Update mass
        //Utilities.UpdateMasses(CurrentObject.transform);

        //Stop rendering
        preRenderObject.Activate(false);
        GameData.IsRenderingOld = false;
    }

    public Vector3 PositionRelativeToCamera(float distance)
    {
        //Set position relative to camera
        RaycastHit Hit;
        Vector2 ScreenCentre = new Vector2(Screen.width / 2, Screen.height / 2 - 1);
        SelectionDetection = Camera.main.ScreenPointToRay(ScreenCentre);

        //Return closer if ray collides with an object
        if (Physics.Raycast(SelectionDetection, out Hit, distance))
            return Hit.point;
        return SelectionDetection.GetPoint(distance);
    }

    public GameObject MergeObject(GameObject Object)
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
        Object.GetComponent<MeshFilter>().sharedMesh = FinalMesh;

        //Delete children
        //foreach (Transform Child in Object.transform)
        //{
        //    Destroy(Child.gameObject);
        //}

        //Reset object
        Object.transform.SetPositionAndRotation(OrglPos, OrglRot);

        //return FinalMesh;
        return Object;
    }

    private Vector3 Multiply(Vector3 one, Vector3 two)
    {
        Vector3 product = new Vector3();
        product.x = one.x * two.x;
        product.y = one.y * two.y;
        product.z = one.z * two.y;
        return product;
    }

    private Vector3 Divide(Vector3 one, Vector3 two)
    {
        Vector3 product = new Vector3();
        product.x = one.x / two.x;
        product.y = one.y / two.y;
        product.z = one.z / two.y;
        return product;
    }
}
