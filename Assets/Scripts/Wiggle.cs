using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


public class Wiggle : MonoBehaviour, IPointerEnterHandler
{
    [Range(0, 90)]
    public float MaxRotation = 15;
    [Range(0, 10)]
    public int Speed = 2;
    public bool RotateOnMouseOver;


    private float state = 0;
    private float start;
    private bool mouseOver;
    private bool stop = true;

    private void Awake()
    {
        start = transform.rotation.eulerAngles.z;
        state = 0;
    }

    private void OnDisable()
    {
        //Reset rotation when disabled
        ResetRotation();
    }

    private void Update()
    {
        if (RotateOnMouseOver && mouseOver)
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

        if (!RotateOnMouseOver || mouseOver)
        {
            //Wiggle if mouse over or if mouseover not set
            stop = false;
            WiggleStep(false);                
        }
        else if (RotateOnMouseOver && !mouseOver)
        {
            //Reset rotation when mouse not over
            WiggleStep(true);
        }
    }
    
    private void WiggleStep(bool resetRot)
    {
        //Only wiggle if not set to stop
        if (!(resetRot && stop))
        {
            //Set new rotation each frame
            Vector3 rotation = transform.rotation.eulerAngles;
            rotation.z = start + Mathf.Sin(state) * MaxRotation;
            transform.rotation = Quaternion.Euler(rotation);
            //Loop
            state = Mathf.Repeat(state, 2 * (float)Math.PI);
            //Add smoothly
            state += Time.deltaTime * Speed;
            //Stop after finishing cycle
            if (resetRot && state >= 2 * Math.PI)
                stop = true;
        }
        else
            ResetRotation();
    }

    private void ResetRotation()
    {
        //Reset rotation to original state
        Vector3 rotation = transform.rotation.eulerAngles;
        rotation.z = start;
        transform.rotation = Quaternion.Euler(rotation);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        //Trigger the wiggle
        mouseOver = true;
    }
}
