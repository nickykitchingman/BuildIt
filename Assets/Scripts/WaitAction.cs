using System.Collections;
using UnityEngine;

public class WaitAction : UserAction
{
    [Range(0, 30)]
    public int seconds = 5;

    private void Start()
    {
        StartCoroutine(Wait());
    }

    private IEnumerator Wait()
    {
        //Wait for seconds then complete
        yield return new WaitForSecondsRealtime(seconds);
        Complete = true;
    }
}
