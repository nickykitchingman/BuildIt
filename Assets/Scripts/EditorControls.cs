using UnityEngine;

public class EditorControls : MonoBehaviour
{
    public GameObject Center;

    [Header("Sensitivity")]
    public float HorizontalSensitivity = 8f;
    public float VerticalSensitivity = 8f;

    void Update()
    {
        if (!GameData.ControlsEnabled)
            return;

        float HorizontalRotation, VerticalRotation;

        //Rotate camera using keys
        HorizontalRotation = Input.GetAxisRaw("Horizontal") * HorizontalSensitivity * Time.deltaTime * 100f;
        VerticalRotation = Input.GetAxisRaw("Vertical") * VerticalSensitivity * Time.deltaTime * 100f;

        //Rotate around center
        Center.transform.Rotate(Vector3.up, -HorizontalRotation, Space.World);
        Center.transform.Rotate(Vector3.right * VerticalRotation);
    }
}
