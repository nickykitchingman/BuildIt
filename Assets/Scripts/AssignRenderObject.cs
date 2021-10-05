using UnityEngine;

public class AssignRenderObject : MonoBehaviour
{
    public PreRenderObject RenderScript;

    private BoxAreaAction[] boxComponents;
    private BoxAreasAction[] boxesComponents;
    private LookAtAction[] lookComponents;
    private TrackObject[] trackComponents;
    private bool rendering;

    private void Start()
    {
        boxComponents = GetComponents<BoxAreaAction>();
        boxesComponents = GetComponents<BoxAreasAction>();
        lookComponents = GetComponents<LookAtAction>();
        trackComponents = GetComponents<TrackObject>();
    }

    private void Update()
    {
        if (GameData.IsRenderingNew || GameData.IsRenderingOld)
        {
            if (!rendering)
            {
                AssignObjectToComponents();
                rendering = true;
            }
        }
        else
            rendering = false;
    }

    private void AssignObjectToComponents()
    {
        foreach (BoxAreaAction component in boxComponents)
        {
            component.Object = RenderScript.CurrentObject.transform;
        }
        foreach (BoxAreasAction component in boxesComponents)
        {
            component.Object = RenderScript.CurrentObject.transform;
        }
        foreach (LookAtAction component in lookComponents)
        {
            component.Object = RenderScript.CurrentObject.transform;
        }
        foreach (TrackObject component in trackComponents)
        {
            component.Target = RenderScript.CurrentObject.transform;
        }
    }
}
