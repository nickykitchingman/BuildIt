using UnityEngine;
using TMPro;

public class PlayerControls : MonoBehaviour
{
    public CharacterController Player;
    public Transform PlayerCamera;
    public Collider Ground;
    public float HeightLimit = 300f;
    [Header("Sensitivity")]
    public float HorizontalSensitivity = 1.5f;
    public float VerticalSensitivity = 1.5f;
    public float MouseSensitivity = 6f;
    [Header("Speed")]
    public float MovementSpeed = 10f;
    public float BoostSpeed = 1f;
    [Header("Interact")]
    public GameObject UIParent;
    public TextMeshProUGUI ModeText;
    [Header("ChangeMode")]
    public UnityEventString OnModeChange;
    [Header("Walking")]
    public Transform GroundCheck;
    public bool Walkmode;
    public float Gravity = -9.81f;
    public float GroundDistance = 1f;
    public LayerMask GroundMask;
    public float JumpHeight = 10;
    public float FallBoost = 2f;

    private Vector3 Velocity;
    private bool IsGrounded;
    private string Mode;

    public Quaternion Rotation
    {
        get
        {
            Vector3 rotation = Player.transform.rotation.eulerAngles;
            rotation.x = PlayerCamera.localRotation.eulerAngles.x;
            return Quaternion.Euler(rotation);
        }
        set
        {
            Vector3 rotation = value.eulerAngles;
            PlayerCamera.localRotation = Quaternion.Euler(Vector3.right * rotation.x);
            rotation.x = 0;
            Player.transform.rotation = Quaternion.Euler(rotation);
        }
    }

    void Start()
    {
        InteractMode();
    }

    void Update()
    {

        if (Walkmode)
        {
            //Gravity
            IsGrounded = Physics.CheckSphere(GroundCheck.position, GroundDistance, GroundMask);
            if (Velocity.y < 0f)
                if (IsGrounded)
                    Velocity.y = -2f;
                else
                    Velocity.y += Gravity * Time.deltaTime * FallBoost;
            else
                Velocity.y += Gravity * Time.deltaTime;

            Physics.SyncTransforms();
            Player.Move(Velocity * Time.deltaTime);
        }

        //Do nothing if disabled
        if (!GameData.ControlsEnabled)
        {
            return;
        }

        float HorizontalRotation, VerticalRotation;

        float Forwards, Sideways, Upwards;
        float ActualMovementSpeed;

        float XRotation = GetXRotation();

        switch (Mode)
        {
            case "Moving":
                ActualMovementSpeed = MovementSpeed * BoostSpeed * (Input.GetAxis("Boost") + 1);

                //Rotate camera using mouse
                HorizontalRotation = Input.GetAxis("Mouse X") * MouseSensitivity;
                VerticalRotation = Input.GetAxis("Mouse Y") * MouseSensitivity;

                Player.transform.Rotate(Vector3.up * HorizontalRotation, Space.World);

                //Clamp vertical rotation
                if (XRotation <= 80 || VerticalRotation < 0)
                    if (XRotation >= -80 || VerticalRotation > 0)
                    {
                        PlayerCamera.transform.Rotate(Vector3.left * VerticalRotation);
                    }

                //Move with keys
                Forwards = Input.GetAxis("Horizontal") * ActualMovementSpeed * Time.deltaTime * 100f;
                Sideways = Input.GetAxis("Vertical") * ActualMovementSpeed * Time.deltaTime * 100f;
                Upwards = Input.GetAxis("Jump") * ActualMovementSpeed * Time.deltaTime * 100f;

                Vector3 NewPosition = Vector3.zero;
                NewPosition += RemoveY(Player.transform.right * Forwards);
                NewPosition += RemoveY(Player.transform.forward * Sideways);

                if (Walkmode)
                {
                    if (Input.GetButtonDown("Jump") && IsGrounded)
                        Velocity.y = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }
                else
                    NewPosition.y += Upwards;

                Player.Move(NewPosition);
                break;
            case "Interact":
                //Rotate camera using keys
                //Horizontal
                HorizontalRotation = Input.GetAxis("Horizontal") * HorizontalSensitivity;
                HorizontalRotation *= Time.deltaTime * 100f;
                Player.transform.Rotate(Vector3.up * HorizontalRotation, Space.World);

                //Vertical
                VerticalRotation = Input.GetAxis("Vertical") * VerticalSensitivity;
                VerticalRotation *= Time.deltaTime * 100f;
                
                //Clamp vertical rotation
                if (XRotation <= 80 || VerticalRotation < 0)
                    if (XRotation >= -80 || VerticalRotation > 0)
                    {
                        PlayerCamera.transform.Rotate(Vector3.left * VerticalRotation);
                    }
                break;
        }
        
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            CycleMode();
        }
    }

    /// <summary>
    /// Returns the vertical rotation (x) in degrees of the player
    /// </summary>
    /// <returns></returns>
    private float GetXRotation()
    {
        //Find point in front of player
        Vector3 dir = PlayerCamera.transform.forward;
        Ray ray = new Ray(transform.position, dir);
        Vector3 target = ray.GetPoint(50);
        //Relative to player
        target -= PlayerCamera.position;

        //Use trigonometry to determine vertical angle
        float hor, ver, angle;
        ver = target.y;
        hor = Mathf.Sqrt(target.x * target.x + target.z * target.z);
        angle = Mathf.Atan2(ver, hor);
        return angle * 180 / Mathf.PI;
    }

    /// <summary>
    /// Cycles to the next mode
    /// </summary>
    private void CycleMode()
    {
        switch (Mode)
        {
            case "Moving":
                InteractMode();
                break;
            case "Interact":
                MovingMode();
                break;
        }
    }

    public void MovingMode()
    {
        //Change mode
        Mode = "Moving";
        ModeText.text = "Moving";
        //Make cursor invisible
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        //Activate menu
        UIParent.SetActive(false);
        //Update gamedata
        GameData.IsMoving = true;
        //Invoke modechange event
        OnModeChange.Invoke(Mode);
    }

    public void InteractMode()
    {
        //Change mode
        Mode = "Interact";
        ModeText.text = "Interacting";
        //Make cursor visible
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //Deactivate UI menu
        UIParent.SetActive(true);
        //Update gamedata
        GameData.IsMoving = false;
        //Invoke mode change event
        OnModeChange.Invoke(Mode);
    }

    private Vector3 RemoveY(Vector3 vector)
    {
        float x = vector.x;
        float z = vector.z;
        return new Vector3(x, 0, z);
    }
     
    public void MovePlayer(Vector3 position)
    {
        Player.transform.localPosition = position;
    }

    public Vector3 ClampToValidPosition(Vector3 position)
    {
        //Check position against ground bounds
        float x = position.x;
        float y = position.y;
        float z = position.z;

        //Limit x direction
        if (Mathf.Abs(position.x) > Ground.bounds.max.x)
            x = Ground.bounds.max.x * (position.x < 0 ? -1 : 1);

        //Limit z direction
        if (Mathf.Abs(position.z) > Ground.bounds.max.z)
            z = Ground.bounds.max.z * (position.z < 0 ? -1 : 1);

        //Limit minimum height
        if (position.y < Ground.transform.position.y + 2f)
            y = Ground.transform.position.y + 2f;

        //Limit maximum height
        if (Mathf.Abs(position.y) > HeightLimit)
            y = HeightLimit;

        return new Vector3(x, y, z);
    }
}
