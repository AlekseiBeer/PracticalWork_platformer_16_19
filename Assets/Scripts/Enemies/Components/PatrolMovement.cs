using UnityEngine;

public class PatrolMovement : MonoBehaviour, IMovement
{
    public Transform pointA;
    public Transform pointB;
    public float speed = 3f;
    private Vector3 targetPosition;

    private Vector3 posA;
    private Vector3 posB;

    private void Start()
    {
        posA = pointA.position;
        posB = pointB.position;

        targetPosition = posA;
    }

    public void Move()
    {
        if (pointA == null || pointB == null) return;

        posA.y = posB.y = targetPosition.y = transform.position.y;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            GetComponent<SpriteRenderer>().flipX = !GetComponent<SpriteRenderer>().flipX;
            targetPosition = Mathf.Equals(targetPosition, posA) ? posB : posA;
        }
    }
}