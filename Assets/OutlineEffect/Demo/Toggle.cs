using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cakeslice
{
    public class Toggle : MonoBehaviour
    {
        // Update is called once per frame
        void Update()
        {
            if(Input.GetKeyDown(KeyCode.K))
            {
                GetComponent<Outline>().enabled = !GetComponent<Outline>().enabled;
            }
        }
    }
}