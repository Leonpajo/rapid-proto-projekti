using UnityEngine;

public class FloatingBook : MonoBehaviour
{
    public float floatHeight = 0.1f;
    public float floatSpeed = 1f;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * floatSpeed) * floatHeight;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }
}