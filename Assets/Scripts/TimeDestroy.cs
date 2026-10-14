using UnityEngine;

public class TimeDestroy : MonoBehaviour
{
    public float lifetime = 0.1f;

    public void Start()
    {
        Destroy(gameObject, lifetime);
    }
}