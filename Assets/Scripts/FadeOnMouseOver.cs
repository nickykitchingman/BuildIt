using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FadeOnMouseOver : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Image Background;
    public float Transparency;
    public bool IncludeChildren;

    private float OriginalTransparency = 1f;

    void Start()
    {
        OriginalTransparency = Background.material.color.a;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        //Fade when mouse over
        if (IncludeChildren)
        {
            foreach (Image image in Background.GetComponentsInChildren<Image>())
                ChangeTransparency(image, Transparency);
        }
        else
            ChangeTransparency(Background, Transparency);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        //Reset
        if (IncludeChildren)
        {
            foreach (Image image in Background.GetComponentsInChildren<Image>())
                ChangeTransparency(image, OriginalTransparency);
        }
        else
            ChangeTransparency(Background, OriginalTransparency);

    }

    void ChangeTransparency(Image image, float alpha)
    {
        //Make new colour with new alpha
        Color OriginalColour = image.material.color;
        Color NewColour = new Color(OriginalColour.r, OriginalColour.g, OriginalColour.b, alpha);
        //Set material colour to new colour
        Background.material.color = NewColour;
    }
}
