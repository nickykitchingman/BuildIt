using System.Linq;
using UnityEngine;

public class SphereAreaAction : UserAction
{
    public Transform Area;
    public Transform Player;
    [Range(0, 100)]
    public int radius;

    private void Update()
    {
        if (Physics.OverlapSphere(Area.position, radius).Any(f => f.transform == Player))
            Complete = true;
    }
}
