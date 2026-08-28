using UnityEngine;

public class GhostPatrol : MonoBehaviour
{
    public Transform[] points;
    public float speed = 2f;
    public float rotationSpeed = 5f;

    private int currentPoint = 0;

    void Update()
    {
        if (points.Length == 0)
            return;

        Transform target = points[currentPoint];

        transform.position = Vector3.MoveTowards(
            transform.position,
            target.position,
            speed * Time.deltaTime
        );

        Vector3 direction = target.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }

        if (Vector3.Distance(transform.position, target.position) < 0.1f)
        {
            currentPoint++;

            if (currentPoint >= points.Length)
            {
                currentPoint = 0;
            }
        }
    }
}