using UnityEngine;

public class TrackObject : MonoBehaviour
{
    public Transform Target;
    public bool TrackPosition = true;
    public Vector3 WorldOffset;
    public Vector3 LocalOffset;
    public bool TrackRotation = true;
    public Vector3 RotationOffset;

    public bool StickToGround;

    void Update()
    {
        if (Target)
        {
            //Position
            if (TrackPosition)
            {
                Vector3 position = Target.transform.position + WorldOffset + FindLocalOffset();
                if (StickToGround)
                    position.y = 0.1f;
                transform.position = position;
            }
            //Rotation
            if (TrackRotation)
            {
                Vector3 rotation = Target.transform.rotation.eulerAngles + RotationOffset;
                transform.rotation = Quaternion.Euler(rotation);
            }
        }
    }

    private Vector3 FindLocalOffset()
    {
        Vector3 x, y, z;
        x = Target.right * LocalOffset.x;
        y = Target.up * LocalOffset.y;
        z = Target.forward * LocalOffset.z;

        return x + y + z;
    }
}