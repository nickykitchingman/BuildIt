using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.UI;

[System.Serializable] public class UnityEventBool : UnityEvent<bool> { }

public class PreRenderObject : MonoBehaviour
{
    public Transform Player;
    public GameObject AxisArrowHead;
    public Transform Ground;
    [Header("Render Components")]
    public RenderProperties RenderOptions;
    public Numpad numpad;
    public RenderPrompts RenderPrompts;
    [Header("Spawning")]
    public bool RelativeToCamera = true;
    public float Distance = 5f;
    public float PlaneDistance = 30f;
    [Header("Limits")]
    public float MinScale = 0.1f;
    public float MaxScale = 10f;
    [Header("Activate")]
    public UnityEventBool OnActivation;
    [Header("Moving by WASD")]
    public float HorizontalSensitivity = 1.5f;
    public float VerticalSensitivity = 1.5f;
    [Header("Deacivate UI")]
    public GameObject UIParent;
    [Header("Materials")]
    public Material RenderMat;
    public Material InvalidMat;
    public bool IsMoving { get; private set; }
    public bool IsValid { get; private set; }

    [HideInInspector]
    public bool ControlsEnabled;
    private bool IsCurrentlyActive;

    [HideInInspector]
    public GameObject CurrentObject;

    private GameObject Rendering;
    private GameObject Axis;
    private GameObject[] Axes;
    private Material[] AxesMats;
    private GameObject[,] ArrowHeads;
    private Ray ViewingRay;
    private Vector3 MouseCurrentPosition;
    private Vector3 MousePreviousPosition;

    private string ShapeType;
    private int AxisModifyMode;  
    private Vector3[] Rotations;
    private bool IsDragging;
    private Vector3 NewSize;


    private void Awake()
    {
        //Initiate variables
        IsCurrentlyActive = false;
        ControlsEnabled = true;
        IsMoving = true;
        IsValid = true;
        IsDragging = false;
        NewSize = Vector3.one;
    }

    private void Start()
    {
        //Initiate rendering object
        Rendering = new GameObject("Rendering");

        CurrentObject = new GameObject("CurrentRenderedObject");
        CurrentObject.transform.parent = Rendering.transform;
        CurrentObject.AddComponent<MeshFilter>();
        CurrentObject.AddComponent<MeshCollider>().convex = true;
        CurrentObject.GetComponent<MeshCollider>().isTrigger = true;
        CurrentObject.AddComponent<RenderObject>().Ground = Ground;
        CurrentObject.AddComponent<Properties>();
        CurrentObject.AddComponent<Rigidbody>().useGravity = false;
        
        MouseCurrentPosition = MousePreviousPosition = Vector3.zero;

        Rotations = new Vector3[] { CurrentObject.transform.right, Vector3.up, CurrentObject.transform.forward };
        AxisModifyMode = 0;

        //Axis template rendered on object
        Axis = new GameObject();
        Axis.transform.parent = Rendering.transform;
        Axis.layer = 9;
        Axis.AddComponent<AxisInfo>();
        //Set up line renderer
        Axis.AddComponent<LineRenderer>();
        Axis.GetComponent<LineRenderer>().positionCount = 2;
        Axis.GetComponent<LineRenderer>().startWidth = 0.1f;
        Axis.GetComponent<LineRenderer>().endWidth = 0.1f;
        //Set up collider
        Axis.AddComponent<CapsuleCollider>();
        Axis.GetComponent<CapsuleCollider>().radius = 0.3f;
        Axis.GetComponent<CapsuleCollider>().height = 4.5f;
        Axis.GetComponent<CapsuleCollider>().center = CurrentObject.transform.position;
        

        //Duplicate axis template for y and z
        Axes = new GameObject[] { Axis, Instantiate(Axis, Rendering.transform), Instantiate(Axis, Rendering.transform) };
        AxesMats = Resources.LoadAll("Materials/BuiltInMats/Axes", typeof(Material)).Cast<Material>().ToArray();

        //Add materials, names and collider directions to axes
        for (int i = 0; i < 3; i++)
        {
            Axes[i].GetComponent<LineRenderer>().material = AxesMats[i];
            Axes[i].GetComponent<LineRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            Axes[i].GetComponent<LineRenderer>().receiveShadows = false;
            Axes[i].name = (i == 0 ? "X" : i == 1 ? "Y" : "Z") + "axis";
            Axes[i].GetComponent<CapsuleCollider>().direction = i;
        }
        
        //Add arrowheads
        ArrowHeads = new GameObject[3, 2];
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                //Clone arrowhead
                ArrowHeads[i, j] = Instantiate(AxisArrowHead, Axes[i].transform);
                //Name
                ArrowHeads[i,j].name = (j == 1 ? "Up" : "Down") + " Arrow";
                //Material
                ArrowHeads[i, j].GetComponent<Renderer>().material = AxesMats[i];
                //Visible through everything - second camera
                ArrowHeads[i, j].layer = 9;
                //Set to axis
                Axes[i].GetComponent<AxisInfo>().SetArrowHeads(ArrowHeads[i, j], j);
                //Remove shadows
                ArrowHeads[i, j].GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                ArrowHeads[i, j].GetComponent<Renderer>().receiveShadows = false;
            }
        }
        
        //Add handler to numpad
        numpad.AddListeners(ChangeMode);

        //Start deactivated
        ActivateInstant(false);
    }

    private void Update()
    {
        //Set axis of rotation of mouse using numberpad
        if (GameData.NumpadEnabled)
        {
            for (int i = 0; i <= 9; i++)
            {
                if (Input.GetKeyDown("" + i))
                {
                    if (i == 0)
                        ChangeMode(9);
                    else
                        ChangeMode(i - 1);
                }
            }
        }

        //If not active, end update function
        if (!(IsCurrentlyActive && ControlsEnabled))
            return;

        ViewingRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));

        //Render a plane only on the ground
        if (ShapeType == "Plane")
        {
            float Distance = (Rendering.transform.position - Player.position).magnitude;
            if (Distance >= PlaneDistance)
            {
                GameData.ControlsEnabled = true;
                JumpPlaneToView();
            }
            else
            {
                GameData.ControlsEnabled = false;
                float HorizontalMove = Input.GetAxis("Horizontal") * HorizontalSensitivity;
                HorizontalMove *= Time.deltaTime * 100f;

                float VerticalMove = Input.GetAxis("Vertical") * VerticalSensitivity;
                VerticalMove *= Time.deltaTime * 100f;

                MovePlane(HorizontalMove, VerticalMove);
                PointCameraAtObject();
            }
        }

        //Move when axis dragged
        Vector3[] directions = new Vector3[] { CurrentObject.transform.right, CurrentObject.transform.up, CurrentObject.transform.forward };
        for (int i = 0; i < Axes.Length; i++)
        {
            GameObject axis = Axes[i];
            AxisInfo axisInfo = axis.GetComponent<AxisInfo>();
            Vector3 MouseChange;


            //Move by dragging axes
            if (axisInfo.IsSelected)
            {
                //Positions of line renderer (ends of axis)
                Vector3 position1 = axis.GetComponent<LineRenderer>().GetPosition(0);
                Vector3 position2 = axis.GetComponent<LineRenderer>().GetPosition(1);
                //Direction of axis in screenview
                Vector3 screenPosition1 = Camera.main.WorldToScreenPoint(position1);
                Vector3 screenPosition2 = Camera.main.WorldToScreenPoint(position2);
                Vector3 screenDirection = (screenPosition2 - screenPosition1).normalized;
                //Change of mouse positions in screenview
                MouseCurrentPosition = Input.mousePosition;
                if (IsDragging && !(ShapeType == "Plane" && axis.name == "Yaxis"))
                    MouseChange = MouseCurrentPosition - MousePreviousPosition;
                else
                    MouseChange = Vector3.zero;

                //Move object in direction of axis by component of mouse movement in axis' direction
                Rendering.transform.Translate(directions[i] * DotProduct(MouseChange, screenDirection) * Time.deltaTime, Space.World);
                //Update previous mouse position
                MousePreviousPosition = MouseCurrentPosition;
                //Set IsDragging to eliminate moving error when selecting axis
                if (Input.GetMouseButton(0))
                    IsDragging = true;
                else
                    IsDragging = false;
            }
        }

        //Rotate object by mouse scroll
        if (Input.GetAxis("Mouse ScrollWheel") != 0f && GameData.NumpadEnabled)
        {
            if (AxisModifyMode < 3)
            {
                //Rotate on axes for 1-3
                int axis = AxisModifyMode;
                if (!(ShapeType == "Plane" && axis != 1))
                    Rendering.transform.Rotate(Rotations[axis], Input.GetAxis("Mouse ScrollWheel"), Space.Self);
            }
            else if (AxisModifyMode < 6)
            {
                //Move on axis for 4-6
                int axis = AxisModifyMode - 3;
                if (!(ShapeType == "Plane" && axis == 1))
                    Rendering.transform.Translate(2 * directions[axis] * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime, Space.World);
            }
            else if (AxisModifyMode <= 9)
            {
                //Move on axis for 6-9
                int axis = AxisModifyMode - 6;
                //Scale up if positive, down if negative
                float scale = Input.GetAxis("Mouse ScrollWheel") > 0f ? 1.1f : 1 / 1.1f;
                if (Mathf.Abs(NewSize.magnitude) > MinScale || scale > 1f)
                    if (NewSize.magnitude < MaxScale || scale < 1f)
                    {
                        if (axis == 3)
                        {
                            //Scale whole object
                            Vector3 newScale = CurrentObject.transform.localScale * scale;
                            NewSize *= scale;
                            if (ShapeType == "Plane")
                            {
                                newScale[1] = 1;
                                NewSize[1] = 1;
                            }
                            CurrentObject.transform.localScale = newScale;
                        }
                        else
                        {
                            if (!(ShapeType == "Plane" && axis == 1))
                            {
                                //Scale individual axis
                                Vector3 objScale = CurrentObject.transform.localScale;
                                objScale[axis] *= scale;
                                NewSize[axis] *= scale;
                                CurrentObject.transform.localScale = objScale;
                            }
                        }
                    }
            }
        }

        //Check validity of position
        if (GameData.Mode != "editor")
            SetValid(CurrentObject.GetComponent<RenderObject>().ValidPosition);
    }

    private void LateUpdate()
    {
        //x, y, z axis on object
        Vector3 x, y, z, current;
        current = CurrentObject.transform.position;
        x = current + CurrentObject.transform.right * 2;
        y = current + CurrentObject.transform.up * 2;
        z = current + CurrentObject.transform.forward * 2;

        //Update end positions of axes
        Axes[0].GetComponent<LineRenderer>().SetPositions(new Vector3[] { 2 * current - x, x });
        Axes[1].GetComponent<LineRenderer>().SetPositions(new Vector3[] { 2 * current - y, y });
        Axes[2].GetComponent<LineRenderer>().SetPositions(new Vector3[] { 2 * current - z, z });

        //Update arrowhead positions
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                //Align the arrowheads to the axis
                Vector3 position = Axes[i].GetComponent<LineRenderer>().GetPosition(j);
                ArrowHeads[i, j].transform.position = position;
                Vector3 direction = position - CurrentObject.transform.position;
                ArrowHeads[i, j].transform.rotation = Quaternion.LookRotation(direction.normalized, ArrowHeads[i, j].transform.up);
            }
        }
    }

    /// <summary>
    /// Updates object currently being rendered
    /// </summary>
    /// <param name="NewObject"></param>
    public void UpdateObject(GameObject NewObject, bool IsNew = false)
    {
        //Reset scale and update size
        CurrentObject.transform.localScale = Vector3.one;
        NewSize = NewObject.GetComponent<Properties>().Scale;

        //Update mesh
        CurrentObject.GetComponent<MeshFilter>().sharedMesh = HandleUserObjects.MergeObject(NewObject);

        //Update collider
        DestroyImmediate(CurrentObject.GetComponent<Collider>());
        CurrentObject.AddComponent<MeshCollider>().convex = true;
        CurrentObject.GetComponent<MeshCollider>().isTrigger = true;

        //Remove renderer on current object
        DestroyImmediate(CurrentObject.GetComponent<Renderer>());

        //Add new renderer and material to current object
        CurrentObject.AddComponent<MeshRenderer>().material = RenderMat;
        CurrentObject.GetComponent<MeshRenderer>().receiveShadows = false;
        CurrentObject.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

        //Update type
        ShapeType = NewObject.GetComponent<Properties>().ShapeType;
        CurrentObject.GetComponent<RenderObject>().ShapeType = ShapeType;

        //Update rotation
        Rendering.transform.rotation = NewObject.transform.rotation;

        //Update properties
        UpdateProperties(NewObject.GetComponent<Properties>());
        RenderOptions.UpdateObject(CurrentObject, NewObject.name);

        //Update position
        if (ShapeType == "Plane" && IsNew)
            JumpPlaneToView();
        else if (!RelativeToCamera && IsNew)
            Rendering.transform.position = Vector3.zero;
        else
            Rendering.transform.position = NewObject.transform.position;

        //Update camera rotation
        PointCameraAtObjectSmooth();
    }

    public void ChangeMode(int num)
    {
        AxisModifyMode = num;
        numpad.Select(num);
    }

    public Vector3 GetPosition()
    {
        return CurrentObject.transform.position;
    }

    public Quaternion GetRotation()
    {
        return CurrentObject.transform.rotation;
    }

    public Vector3 GetScale()
    {
        return NewSize;
    }

    /// <summary>
    /// Activate or deactivate rendering
    /// </summary>
    /// <param name="value"></param>
    public void Activate(bool value)
    {
        //Tools used here
        IsCurrentlyActive = value;
        Rendering.SetActive(value);
        RenderOptions.Activate(value);
        numpad.Activate(value);
        RenderPrompts.Activate(value);
        //Invoke activation event
        OnActivation.Invoke(value);
        //Disable or enable UI
        if (UIParent)
            UIParent.SetActive(!value);
    }
    
    /// <summary>
    /// Activate or deactivate rendering without animation
    /// </summary>
    /// <param name="value"></param>
    public void ActivateInstant(bool value)
    {
        //Tools used here
        IsCurrentlyActive = value;
        Rendering.SetActive(value);
        RenderOptions.ActivateInstant(value);
        numpad.ActivateInstant(value);
        RenderPrompts.ActivateInstant(value);
        //Invoke activation event
        OnActivation.Invoke(value);
        //Disable or enable UI
        if (UIParent)
            UIParent.SetActive(!value);
    }

    public void SetControls(bool value)
    {
        ControlsEnabled = value;
    }

    private void PointCameraAtObjectSmooth()
    {
        Vector3 LookPos = Rendering.transform.position - Camera.main.transform.position;

        //Camera
        Quaternion rotation = Quaternion.LookRotation(LookPos);
        Vector3 CameraRotation = rotation.eulerAngles;
        //CameraRotation.y = Camera.main.transform.rotation.eulerAngles.y;

        //Player
        LookPos.y = 0;
        //Quaternion PlayerRotation = Quaternion.LookRotation(LookPos);
        Vector3 PlayerRotation = rotation.eulerAngles.y * Vector3.up;

        CameraRotation.y = PlayerRotation.y;
        //CameraRotation = Player.rotation.eulerAngles - CameraRotation;

        //Smooth rotate
        LeanTween.rotate(Player.gameObject, PlayerRotation, 0.2f);
        LeanTween.rotate(Camera.main.gameObject, CameraRotation, 0.2f);
    }
    
    private void PointCameraAtObject()
    {
        Vector3 LookPos = Rendering.transform.position - Player.position;
        Quaternion rotation = Quaternion.LookRotation(LookPos);

        Player.GetComponent<PlayerControls>().Rotation = rotation;
        //Camera.main.transform.LookAt(Rendering.transform);
    }

    private void SetValid(bool value)
    {
        IsValid = value;
        CurrentObject.GetComponent<Renderer>().material = value ? RenderMat : InvalidMat;
    }

    private void UpdateProperties(Properties properties)
    {
        CurrentObject.GetComponent<Properties>().IsStatic = properties.IsStatic;
        CurrentObject.GetComponent<Properties>().Tag = properties.Tag;
    }

    private Vector3 ClampYValue(Vector3 point, Vector3 boundary)
    {
        if (point.y <= boundary.y)
            return new Vector3(point.x, boundary.y + 0.1f, point.z);
        return point;
    }

    private Vector3 ZeroYValue(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
    }

    private void JumpPlaneToView()
    {
        //Update position relative to camera, then set valid or invalid
        Vector3 newPos = ViewingRay.GetPoint(PlaneDistance);
        if (newPos.y > Ground.position.y)
            SetValid(false);
        else
            SetValid(true);

        //Clamp to ground height
        Rendering.transform.position = ClampYValue(newPos, Ground.position);
    }
    
    private void MovePlane(float horizontal, float vertical)
    {
        Vector3 PlayerForward = Player.forward;
        Vector3 PlayerRight = Player.right;

        PlayerRight *= horizontal;
        PlayerForward *= vertical;

        Vector3 Move = PlayerRight + PlayerForward;

        //Clamp to ground height
        Rendering.transform.Translate(ZeroYValue(Move), Space.World);
    }

    private void CheckValidity()
    {
        if (ShapeType == "Plane")
        {
            Vector3 position = CurrentObject.transform.position;
            if (position.y < Ground.position.y - 0.1f || position.y > Ground.position.y + 0.1f)
                SetValid(false);
        }
    }

    private void ScaleLine(LineRenderer line, float scale)
    {
        if (line.positionCount != 2)
        {
            Debug.Log("Can only scale with two positions");
            return;
        }
        Vector3 one = line.GetPosition(0);
        Vector3 two = line.GetPosition(1);

        Vector3 centre = (line.GetPosition(0) + line.GetPosition(1)) / 2;

        line.SetPosition(0, (one - centre) * scale + centre);
        line.SetPosition(1, (two - centre) * scale + centre);
    }

    private Vector3 PositionRelativeToCamera(float distance)
    {
        //Set position relative to camera
        RaycastHit Hit;
        Vector2 ScreenCentre = new Vector2(Screen.width / 2, Screen.height / 2 - 1);
        ViewingRay = Camera.main.ScreenPointToRay(ScreenCentre);

        //Return closer if ray collides with an object
        if (Physics.Raycast(ViewingRay, out Hit, distance) && Hit.transform.gameObject.name.Contains("Axis"))
        {
            return Hit.point; 
        }
        return ViewingRay.GetPoint(distance);
    }

    private float DotProduct(Vector3 one, Vector3 two)
    {
        float x = one.x * two.x;
        float y = one.y * two.y;
        float z = one.z * two.y;
        return x + y + z;
    }

    private Vector3 Divide(Vector3 one, Vector3 two)
    {
        Vector3 product = new Vector3();
        product.x = one.x / two.x;
        product.y = one.y / two.y;
        product.z = one.z / two.y;
        return product;
    }

    private Vector3 VectorAbs(Vector3 vector)
    {
        vector.x = Mathf.Abs(vector.x);
        vector.y = Mathf.Abs(vector.y);
        vector.z = Mathf.Abs(vector.z);
        return vector;
    }

    private float ResultantMovement(Vector3 MouseChange, Vector3 direction)
    {
        float magnitude = MouseChange.magnitude;
        float angle = Vector3.Angle(MouseChange, direction);
        return magnitude * Mathf.Cos(angle / 180f * 3.14f);
    }
}
