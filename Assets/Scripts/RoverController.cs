using UnityEngine;

public class RoverController : MonoBehaviour
{
    [Header("Rover")]
    public CharacterController Controller;
    public GameObject Body;
    public GameObject Arm;
    public GameObject Head;
    public Rigidbody RoverRB;
    public float StopRange = 10f;
    public float Speed = 4f;
    public float MaxSlope = 85;
    [Header("Sensors")]
    public Transform SensorParent;
    public Transform CenterSensor;
    public Transform ForwardSensor;
    public Transform RightSensor;
    public Transform GroundCheck;
    [Header("Player")]
    public Transform Player;
    [Header("Gravity")]
    public float Gravity = -9.81f;
    public float GroundDistance = 0.5f;
    public LayerMask GroundMask;
    public float FallBoost = 2f;
    [Header("Ground")]
    public Transform Ground;

    private Vector3 Velocity;
    private bool IsGrounded;

    private Vector3 raydir;
    private float count = 0;
    private float range;
    private bool valid = true;
    private int sensorpos = 0;
    private int timecount = 0;


    private void Start()
    {
        //Force slope in bounds
        MaxSlope = Mathf.Clamp(MaxSlope, 0, 85);

        range = StopRange;
        raydir = Player.forward;
        //Spawn rover near player if first load
        if (GameData.FirstLoad)
            if (!MoveToValidPlacement())
                Body.GetComponent<Renderer>().enabled = valid = false;
    }

    private void FixedUpdate()
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

        //Move(Velocity);
        Physics.SyncTransforms();
        Controller.Move(Velocity * Time.deltaTime);

        //Follow player
        if (!WithinStoppingRange())
        {
            RotateTowardsPlayer();
            MoveForward();
            AlignWithGround();
        }

        //Look at player
        LookAtPlayer();        

        //Check active
        if (!valid || (transform.position - Player.position).magnitude > 1000f)
            if (MoveToValidPlacement())
                Body.GetComponent<Renderer>().enabled = valid = true;

        //Increase framecount
        timecount = (int)Mathf.Repeat(timecount + Time.deltaTime, 1000);
    }

    /// <summary>
    /// Turn rover towards player
    /// </summary>
    private void RotateTowardsPlayer()
    {
        //Vector3 velocity = RoverRB.velocity;
        //Vector3 angvelocity = RoverRB.angularVelocity;
        Vector3 LookPos = Player.transform.position - transform.position;
        Quaternion rotation = Quaternion.LookRotation(LookPos);
        Vector3 eulerRotation = rotation.eulerAngles;
        eulerRotation.x = 0f;

        //transform.rotation = Quaternion.Euler(eulerRotation);
        LeanTween.rotate(gameObject, eulerRotation, 0.1f);
        //RoverRB.velocity = velocity;
        //RoverRB.angularVelocity = angvelocity;
    }

    /// <summary>
    /// Turn arm and head towards the player
    /// </summary>
    private void LookAtPlayer()
    {
        Vector3 ArmLookDir = Arm.transform.position - Player.position;
        Vector3 HeadLookDir = Head.transform.position - Player.position;

        Arm.transform.rotation = Quaternion.LookRotation(transform.up, ArmLookDir);

        Vector3 rotation = Quaternion.LookRotation(HeadLookDir, transform.up).eulerAngles;
        rotation.x += 90;

        Head.transform.rotation = Quaternion.Euler(rotation);    
    }

    /// <summary>
    /// Move rover forwards at given speed
    /// </summary>
    private void MoveForward()
    {
        Vector3 movement = transform.forward * Speed * Time.deltaTime;
        if (!IsGrounded)
            movement.y = 0;

        //RoverRB.AddRelativeForce(movement);
        //Move(movement);
        Physics.SyncTransforms();
        Controller.Move(movement);
    }

    /// <summary>
    /// Checks whether the rover is within stopping range of player
    /// </summary>
    /// <returns></returns>
    private bool WithinStoppingRange()
    {
        Vector3 displacement = Player.transform.position - transform.position;
        displacement.y = 0f;
        if (displacement.magnitude < StopRange)
            return true;

        return false;
    }

    private bool MoveToValidPlacement()
    {
        Debug.Log("resetting position");

        Ray ray = new Ray(Player.position, raydir);

        float rand = Random.Range(5, range);
        transform.position = ray.GetPoint(rand);

        if (transform.position.y < Ground.position.y)

            return false;

        //New direction
        raydir.y += 0.1f;
        raydir.x += 0.001f;
        raydir.z += 0.001f;
        raydir = raydir.normalized;

        //Increase range every 1000 attempts
        count++;
        if (count > 1000)
            range++;

        //Return whether in valid position
        return Body.GetComponent<RenderObject>().ValidPosition;
    }

    private void AlignWithGround()
    {
        Ray CenterRay = new Ray(CenterSensor.position, -transform.up);
        Ray ForwardRay = new Ray(ForwardSensor.position, -transform.up);
        Ray RightRay = new Ray(RightSensor.position, -transform.up);

        float ForwardDistance = GetMaxRayDistance(CenterSensor.localPosition, ForwardSensor.localPosition);
        float RightDistance = GetMaxRayDistance(CenterSensor.localPosition, RightSensor.localPosition);

        Vector3 CenterF, CenterR, Forward, Right;
        RaycastHit Hit;
        int layerMask = ~LayerMask.GetMask("Rover");

        //Center forward, rotate sensors if no hit
        if (Physics.Raycast(CenterRay, out Hit, ForwardDistance, layerMask))
            CenterF = Hit.point;
        else
        {
            SensorParent.Rotate(Vector3.up * 90);
            sensorpos = (int)Mathf.Repeat(sensorpos + 1, 4);
            return;
        }

        //Center right
        if (Physics.Raycast(CenterRay, out Hit, RightDistance, layerMask))
            CenterR = Hit.point;
        else CenterR = Vector3.zero;

        //Forward
        if (Physics.Raycast(ForwardRay, out Hit, ForwardDistance, layerMask))
            Forward = Hit.point;
        else Forward = Vector3.zero;

        //Right
        if (Physics.Raycast(RightRay, out Hit, RightDistance, layerMask))
            Right = Hit.point;
        else Right = Vector3.zero;

        //Create quaternion rotation
        float ForwardAngle = GetAngle(CenterF, Forward);
        float RightAngle = GetAngle(CenterR, Right);

        //Adjust the rotation
        Quaternion Rotation = AdjustRotation(ForwardAngle, RightAngle);
        Vector3 eulers = Rotation.eulerAngles;
        eulers.x *= -1;
        eulers.x -= 90;
        eulers.x -= transform.rotation.eulerAngles.x;
        Rotation = Quaternion.Euler(eulers);

        //Set rotation of body (not this.transform)
        Rotation = Quaternion.RotateTowards(Body.transform.localRotation, Rotation, 100f * Time.deltaTime);
        Body.transform.localRotation = Rotation;
    }

    /// <summary>
    /// Returns vertical angle from point one to point two
    /// </summary>
    /// <param name="one"></param>
    /// <param name="two"></param>
    /// <returns></returns>
    private float GetAngle(Vector3 one, Vector3 two)
    {
        float hor, ver, x, z;

        //Horizontal separation
        x = two.x - one.x;
        z = two.z - one.z;
        hor = Mathf.Sqrt(x * x + z * z);

        //Vertical separation
        ver = two.y - one.y;

        //Angle between
        return Mathf.Atan2(ver, hor) * 180 / Mathf.PI;
    }

    /// <summary>
    /// Determines maximum value to be used in a pair of sensors
    /// </summary>
    /// <param name="one"></param>
    /// <param name="two"></param>
    /// <returns></returns>
    private float GetMaxRayDistance(Vector3 one, Vector3 two)
    {
        float x, z, hor;

        //Horizontal separation
        x = two.x - one.x;
        z = two.z - one.z;
        hor = Mathf.Sqrt(x * x + z * z);

        //Vertical distance at max angle
        return Mathf.Tan(MaxSlope / 180 * Mathf.PI) * hor;
    }

    /// <summary>
    /// Adjust a rotation depending on sensor rotation
    /// </summary>
    /// <param name="forward"></param>
    /// <param name="right"></param>
    /// <returns></returns>
    private Quaternion AdjustRotation(float forward, float right)
    {
        switch (sensorpos)
        {
            case 1:
                return Quaternion.Euler(-right, -forward, 0);
            case 2:
                return Quaternion.Euler(-forward, -right, 0);
            case 3:
                return Quaternion.Euler(right, forward, 0);
        }
        return Quaternion.Euler(forward, right, 0);
    }
}
