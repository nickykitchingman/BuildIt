using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisableDropdownElements : MonoBehaviour
{
    public GameObject Item;
    public List<string> NullWords;

    private void Start()
    {
        foreach (string word in NullWords)
            //Check null word is not already set
            if (!Item.GetComponents<DisableOption>().Any(f => f.NullWord == word))
            {
                //Set null word on dropdown
                DisableOption component = Item.AddComponent<DisableOption>();
                component.NullWord = word;
            }
    }
}
