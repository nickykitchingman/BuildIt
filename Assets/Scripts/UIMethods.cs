using UnityEngine;
using UnityEngine.UI;

public static class UIMethods
{
    private static GameObject[] AllElements;
    private static string ElementTag = "MenuElement";


    public static void SetAllElementsActive(bool value)
    {
        AllElements = GameObject.FindGameObjectsWithTag(ElementTag);

        foreach (GameObject element in AllElements)
        {
            try
            {
                element.GetComponent<Selectable>().interactable = value;
                element.GetComponent<OutlineOnHightlight>().enabled = value;
            }
            catch (System.NullReferenceException)
            {
                Debug.Log("Missing Component", element);
            }
        }
    }    
}
