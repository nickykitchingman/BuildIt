using UnityEngine;

public class LookAtAction : UserAction
{
    public Transform Target;
    public Transform Object;
    [Range(0, 1)]
    public float Threshold = 0.1f;
    public bool Reverse;

    private void OnEnable()
    {
        if (Target)
            Target.gameObject.SetActive(true);
    }

    private void OnDisable()
    {
        if (Target)
            Target.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Object)
        {
            Vector3 targetDir = Target.position - Object.position;
            Vector3 dir = Reverse ? -Object.forward : Object.forward;
            Complete = DetermineComplete(targetDir, dir);
        }
    }

    private bool DetermineComplete(Vector3 targetDir, Vector3 dir)
    {
        float close = Vector3.Dot(targetDir.normalized, dir.normalized);
        return close > 1 - Threshold;
    }
}
