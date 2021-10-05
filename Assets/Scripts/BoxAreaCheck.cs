using System.Linq;
using UnityEngine;

public class BoxAreaCheck : UserAction
{
    public Transform Area;
    public Transform Player;
    [Range(0, 100)]
    public int radius;

    private void Update()
    {
        Complete = Physics.OverlapSphere(Area.position, radius).Any(f => f.transform == Player);
    }
}