using UnityEngine;
using UnityEngine.Events;

[System.Serializable] public class TriggerCollision : UnityEvent<Trigger, Collider> { }

public class TriggerScript : MonoBehaviour
{
    public TriggerCollision CollisionEnter = new TriggerCollision();
    public Trigger trigger;

    void OnTriggerEnter(Collider other)
    {
        CollisionEnter.Invoke(trigger, other);
        Debug.Log(string.Format("Collision: {0} | {1}", name, other.name));
    }
}
