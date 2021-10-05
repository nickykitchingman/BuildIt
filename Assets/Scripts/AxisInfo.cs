using UnityEngine;

public class AxisInfo : MonoBehaviour
{
    public bool IsSelected { get; private set; }
    public float Magnitude { get; private set; }

    public GameObject[] ArrowHeads { get; private set; }

    private float Transparency;
    private bool MouseExited;

    void Awake()
    {
        //Set MouseIsOver intially to false
        IsSelected = false;
        //Initialise arrowheads list
        ArrowHeads = new GameObject[2];
    }

    void Start()
    {
        Transparency = GetComponent<Renderer>().material.color.a;
    }

    void Update()
    {
        if (!IsSelected && MouseExited)
            ChangeTransparency(Transparency);

        if (!Input.GetMouseButton(0))
        {
            //Set IsSelected to false if pointer up
            IsSelected = false;
        }
    }

    void OnMouseDrag()
    {
        IsSelected = true;
    }

    void OnMouseOver()
    {
        //Increase alpha when mouse over axis
        if ((!Input.GetMouseButton(0) || IsSelected) && !GameData.IsMoving)
        {
            ChangeTransparency(0.2f);
        }

        MouseExited = false;
    }

    void OnMouseExit()
    {
        //Reset material when mouse exits axis
        MouseExited = true;
    }

    void ChangeTransparency(float alpha)
    {
        //Make new colour with new alpha
        Color OriginalColour = GetComponent<Renderer>().material.color;
        Color NewColour = new Color(OriginalColour.r, OriginalColour.g, OriginalColour.b, alpha);
        //Set material colour to new colour
        for (int i = 0; i < 2; i++)
            ArrowHeads[i].GetComponent<Renderer>().material.color = NewColour;
        GetComponent<Renderer>().material.color = NewColour;
    }

    public void SetArrowHeads(GameObject arrowHead, int index)
    {
        ArrowHeads[index] = arrowHead;
    }
}
