using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightControl : MonoBehaviour
{
    void Update()
    {
        transform.position = Camera.main.transform.position + Vector3.up * 34;
        transform.LookAt(Camera.main.transform);
    }
}
