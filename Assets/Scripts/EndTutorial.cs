using UnityEngine;

public class EndTutorial : MonoBehaviour
{
    public PlayTutorial tutorial;

    private void OnEnable()
    {
        tutorial.End();
        tutorial.AllResponses(true);
    }
}
