using System.Collections;
using UnityEngine;

public class LegMover : MonoBehaviour
{
    public Transform limbSolverTarget;
    public float moveDistance;
    public LayerMask groundLayer;

    [Header("Procedural Stepping")]
    public LegMover oppositeLeg;
    public bool isMoving = false;
    public float stepDuration = 0.15f;
    public float stepHeight = 0.5f;
    
    [Header("Step Placement")]
    [Tooltip("Pushes the foot forward so it doesn't land directly under the hips")]
    public Vector2 stepOffset; 

    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector3.down, 5f, groundLayer);
        
        if (hit.collider != null)
        {
            // Add the offset so the foot steps forward
            Vector3 targetPoint = hit.point + stepOffset;
            targetPoint.y += 0.1f; // Slight offset to prevent floor clipping

            if (Vector2.Distance(limbSolverTarget.position, targetPoint) > moveDistance)
            {
                if (oppositeLeg == null || !oppositeLeg.isMoving)
                {
                    StartCoroutine(TakeStep(targetPoint));
                }
            }
        }
    }

    IEnumerator TakeStep(Vector3 targetPosition)
    {
        isMoving = true;

        Vector3 startPosition = limbSolverTarget.position;
        float timeElapsed = 0f;

        while (timeElapsed < stepDuration)
        {
            timeElapsed += Time.deltaTime;
            float normalizedTime = timeElapsed / stepDuration;

            Vector3 currentPos = Vector3.Lerp(startPosition, targetPosition, normalizedTime);
            
            // The sine wave lifts the foot into an arc
            currentPos.y += Mathf.Sin(normalizedTime * Mathf.PI) * stepHeight;

            limbSolverTarget.position = currentPos;

            yield return null;
        }

        limbSolverTarget.position = targetPosition;
        isMoving = false;
    }
}