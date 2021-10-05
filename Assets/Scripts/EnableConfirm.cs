using UnityEngine;

public class EnableConfirm : MonoBehaviour
{
    private void OnEnable()
    {
        GameData.CanConfirm = true;
    }

    private void OnDisable()
    {
        GameData.CanConfirm = false;
    }
}
