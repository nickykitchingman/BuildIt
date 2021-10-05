using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoxAreasAction : UserAction
{
    public List<MeshFilter> Boxes;
    public Transform Object;

    private bool[] completed;


    private void OnEnable()
    {
        completed = new bool[Boxes.Count];

        if (Boxes != null)
            Boxes.ForEach(f => f.gameObject.SetActive(true));
    }

    private void OnDisable()
    {
        if (Boxes != null)
            Boxes.ForEach(f => f.gameObject.SetActive(false));
    }

    private void Update()
    {
        if (Object)
        {
            RenderObject renderObj = Object.GetComponentInChildren<RenderObject>();
            if (renderObj && !renderObj.ValidPosition)
                return;

            if (Boxes != null)
            {
                //Check every box
                for (int box = 0; box < Boxes.Count; box++)
                    //Yeah, I know
                    completed[box] = Physics.OverlapBox(Boxes[box].transform.position, Boxes[box].sharedMesh.bounds.extents).Any(f => f.transform == Object);
            }
        }

        if (completed.All(f => f == true))
            Complete = true;
    }
}