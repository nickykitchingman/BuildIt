using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


public class Expand : MonoBehaviour, IPointerEnterHandler
{
    [Range(1, 10)]
    public float MaxScale = 2;
    [Range(0, 10)]
    public int Speed = 2;
    public bool ExpandOnMouseOver;


    private float state = 0;
    private Vector3 start;
    private bool mouseOver;
    private bool stop = true;

    private void Awake()
    {
        start = transform.localScale;
        state = 0;
    }

    private void OnDisable()
    {
        //Reset rotation when disabled
        ResetScale();
    }

    private void Update()
    {
        if (ExpandOnMouseOver && mouseOver)
        {
            //Check mouse is still over element
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.position = Input.mousePosition;
            //Find all elements under mouse
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(pointerData, results);
            //Filter check any element under mouse is this gameobject
            mouseOver = results.Any(f => f.gameObject == gameObject);
        }

        if (!ExpandOnMouseOver || mouseOver)
        {
            //Wiggle if mouse over or if mouseover not set
            stop = false;
            ExpandStep(false);
        }
        else if (ExpandOnMouseOver && !mouseOver)
        {
            //Reset rotation when mouse not over
            ExpandStep(true);
        }
    }

    private void ExpandStep(bool resetScale)
    {
        //Only wiggle if not set to stop
        if (!(resetScale && stop))
        {
            //Set new rotation each frame
            Vector3 scale = transform.localScale;
            scale.x = start.x * (float)Math.Pow(MaxScale, Mathf.Sin(state));
            scale.y = start.y * (float) Math.Pow(MaxScale, Math.Sin(state));
            scale.z = start.z * (float) Math.Pow(MaxScale, Math.Sin(state));
            transform.localScale = scale;
            //Loop
            state = Mathf.Repeat(state, 2 * (float)Math.PI);
            //Add smoothly
            state += Time.deltaTime * Speed;
            //Stop after finishing cycle
            if (resetScale && state >= 2 * Math.PI)
                stop = true;
        }
        else
            ResetScale();
    }

    private void ResetScale()
    {
        //Reset rotation to original state
        transform.localScale = start;
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        //Trigger the wiggle
        mouseOver = true;
    }
}
