using System.Collections.Generic;
using UnityEngine;

public class EnableObjects : MonoBehaviour
{
    public List<GameObject> objects;
    public bool Disable;
    public bool RetainState;

    private List<bool> activeStates;

    private void OnEnable()
    {
        activeStates = new List<bool>();

        for (int i = 0; i < objects.Count; i++)
        {
            activeStates.Add(objects[i].activeSelf);
            objects[i].SetActive(!Disable);
        }
    }

    private void OnDisable()
    {
        if (RetainState)
            for (int i = 0; i < objects.Count; i++)
                objects[i].SetActive(activeStates[i]);
    }
}