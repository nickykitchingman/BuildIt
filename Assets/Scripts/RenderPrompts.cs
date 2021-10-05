using UnityEngine;

public class RenderPrompts : MonoBehaviour
{
    public GameObject Background;

    public void Activate(bool value)
    {
        //Set active or inactive
        if (value)
        {
            LeanTween.scale(Background.gameObject, Vector3.zero, 0f);
            Background.gameObject.SetActive(true);
            LeanTween.scale(Background.gameObject, Vector3.one, 0.2f);
        }
        else
        {
            LeanTween.scale(Background.gameObject, Vector3.zero, 0.2f).setOnComplete(Disable);
        }
    }

    public void ActivateInstant(bool value)
    {
        //Set active or inactive
        if (value)
            Background.gameObject.SetActive(true);
        else
            Background.gameObject.SetActive(false);
    }

    private void Disable()
    {
        Background.gameObject.SetActive(false);
        LeanTween.scale(Background.gameObject, Vector3.one, 0f);
    }
}
