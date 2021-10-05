using System.Linq;
using UnityEngine;

public class BoxAreaAction : UserAction
{
    public MeshFilter Box;    
    public Transform Object;


    private void OnEnable()
    {
        if(Box)
            Box.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (Box)
            Box.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Object)
        {
            RenderObject renderObj = Object.GetComponentInChildren<RenderObject>();
            if (renderObj && !renderObj.ValidPosition)
                return;

            if (Box)
            {
                if (Physics.OverlapBox(Box.transform.position, Box.sharedMesh.bounds.extents).Any(f => f.transform == Object))
                    Complete = true;
            }
        }
    }
}