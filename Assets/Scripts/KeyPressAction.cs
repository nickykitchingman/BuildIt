using UnityEngine;

public class KeyPressAction : UserAction
{
    public KeyCode key;

    private void Update()
    {
        if (Input.GetKeyDown(key))
            Complete = true;
    }
}
