using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OutlineOnHightlight : MonoBehaviour, IPointerEnterHandler
{
    public float Thickness = 0.1f;

    private bool mouseOver;
    private bool outlined;

    private void Update()
    {
        if (mouseOver)
        {
            OutlineElement();

            //Check mouse is still over element
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            //Find all elements under mouse
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            //Filter check any element under mouse is this gameobject
            mouseOver = results.Any(f => f.gameObject == gameObject);
        }
        else if (outlined)
        {
            DeOutlineElement();
            outlined = false;
        }
    }

    /// <summary>
    /// Outlines object when highlighted 
    /// </summary>
    /// <param name="eventData"></param>
    private void OutlineElement()
    {
        if (gameObject.GetComponent<Outline>() == null)
        {
            gameObject.AddComponent<Outline>();
            gameObject.GetComponent<Outline>().effectDistance = new Vector2(Thickness, -Thickness);
            outlined = true;
        }
    }

    /// <summary>
    /// Removes outline when not highlighted
    /// </summary>
    private void DeOutlineElement()
    {
        Destroy(gameObject.GetComponent<Outline>());
    }


    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        //Trigger the wiggle
        mouseOver = true;
    }
}
