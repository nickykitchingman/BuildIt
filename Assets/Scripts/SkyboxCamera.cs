using UnityEngine;

public class SkyboxCamera : MonoBehaviour
{
    public PlayerControls Player;
    public bool Rotate;
    public float SkyboxRotateSpeed = 1;
    public Vector3 RotateDirection;

    private void Start()
    {
        if (GameData.Mode != "menu")
            transform.parent.rotation = GameData.SkyboxRotation;
    }

    private void Update()
    {
        //Update skybox camera position
        if (Player != null)
        {
            transform.position = Player.transform.position;

            //Update skybox camera rotation
            transform.localRotation = Player.Rotation;
        }

        //Rotate constantly
        if (Rotate)
        {
            Vector3 SkyboxRotationDir = RotateDirection.normalized;
            transform.parent.Rotate(SkyboxRotationDir * SkyboxRotateSpeed * Time.deltaTime);
        }
    }
}
