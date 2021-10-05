using UnityEngine;

public class RenderObject : MonoBehaviour
{
    public bool ValidPosition = true;
    public string ShapeType;
    public Transform Ground;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform != transform.parent)
            if (!(ShapeType == "Plane" && other.name == "Ground"))
            {
                Debug.Log(other.name + " : " + ShapeType);
                ValidPosition = false;
            }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform != transform.parent)
            ValidPosition = true;
    }

    private void Update()
    {
        if (Ground)
        {
            if (transform.position.y < Ground.position.y)
            {
                ValidPosition = false;
            }
            else if (ShapeType == "Plane")
            {
                if (transform.position.y > Ground.position.y + 0.1f)
                {
                    ValidPosition = false;
                }
                else
                    ValidPosition = true;
            }
        }
    }
}
