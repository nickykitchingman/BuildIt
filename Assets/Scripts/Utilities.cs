using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities 
{
    /// <summary>
    /// Update density of each child object
    /// </summary>
    /// <param name="transform"></param>
    public static void UpdateMasses(Transform transform)
    {
        //Iterate over each child
        foreach(Transform child in transform)
        {
            //Set density based on child's element
            if (!(child.GetComponent<Properties>() && child.GetComponent<Rigidbody>()))
            {
                Debug.Log("UPDATING DENSITY FAILED"); return;
            }
            Element element = child.GetComponent<Properties>().element;
            child.GetComponent<Rigidbody>().SetDensity(element.Density);
            child.GetComponent<Rigidbody>().mass = child.GetComponent<Rigidbody>().mass;
            Debug.Log(string.Format("{0}: {1}", child.name, child.GetComponent<Rigidbody>().mass));
        }
    }
}
