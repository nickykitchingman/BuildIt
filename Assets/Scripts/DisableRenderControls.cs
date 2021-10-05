using UnityEngine;

public class DisableRenderControls : MonoBehaviour
{
    public PreRenderObject RenderScript;
    public bool NumpadEnabled = true;
    public bool ChangeState;

    private bool state;

    private void OnEnable()
    {
        state = RenderScript.ControlsEnabled;
        RenderScript.SetControls(false);
        GameData.NumpadEnabled = NumpadEnabled;
    }

    private void OnDisable()
    {
        if (!ChangeState)
            RenderScript.SetControls(state);
        GameData.NumpadEnabled = true;
    }
}
