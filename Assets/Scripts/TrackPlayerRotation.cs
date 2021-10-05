using UnityEngine;

public class TrackPlayerRotation : MonoBehaviour
{
    public PlayerControls Player;

    void Update()
    {
        transform.rotation = Player.Rotation;
    }
}
