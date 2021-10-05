using UnityEngine;

public class TrackPlayer : MonoBehaviour
{
    public PlayerControls Player;

    void Update()
    {
        transform.position = Player.transform.position;
        transform.rotation = Player.Rotation;
    }
}
