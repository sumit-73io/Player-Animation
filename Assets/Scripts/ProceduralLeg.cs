using UnityEngine;

public class ProceduralLeg : MonoBehaviour
{
    [Header("References")]
    public Transform hipTransform;
    public Transform footTarget;
    public Transform footSpriteTransform; // Assign your foot sprite to rotate it
    public PlayerMovement player; 
    public ProceduralLeg oppositeLeg;

    [Header("Ground & Physics")]
    public LayerMask groundLayer;
    public float rayDistance = 5f;
    public float wallCheckDistance = 0.5f; // How far to look ahead for stairs

    [Header("Foot Placement")]
    public Vector3 footOffset;
    public float stepLength = 0.5f;
    public float defaultStepHeight = 0.3f;
    public float stepSpeed = 5f;
    public float velocityMultiplier = 0.15f; 

    private Vector3 currentFootPos;
    private Vector3 nextFootPos;
    private Quaternion currentFootRot;
    private Quaternion nextFootRot;
    
    private float stepProgress = 1f;
    private float dynamicStepHeight; // Changes if we need to step over something

    // NEW: Moving Platform Tracking
    private Transform connectedPlatform;
    private Vector3 lastPlatformPosition;

    public bool IsStepping => stepProgress < 1f;

    private void Start()
    {
        currentFootPos = footTarget.position;
        nextFootPos = footTarget.position;
        currentFootRot = footSpriteTransform != null ? footSpriteTransform.rotation : Quaternion.identity;
        nextFootRot = currentFootRot;
        dynamicStepHeight = defaultStepHeight;
    }

    private void Update()
    {
        // 1. Air Check: Stop IK if jumping/falling
        if (Mathf.Abs(player.GetComponent<Rigidbody2D>().linearVelocityY) > 0.1f) return;

        // 2. Moving Platform Logic: Move the planted foot with the ground
        if (connectedPlatform != null && stepProgress >= 1f)
        {
            Vector3 platformDelta = connectedPlatform.position - lastPlatformPosition;
            currentFootPos += platformDelta;
            nextFootPos += platformDelta;
            lastPlatformPosition = connectedPlatform.position;
        }

        Vector3 velocityBias = new Vector3(player.VelocityX * velocityMultiplier, 0, 0);
        Vector3 raycastOrigin = hipTransform.position + footOffset + velocityBias;

        RaycastHit2D hit = Physics2D.Raycast(raycastOrigin, Vector2.down, rayDistance, groundLayer);

        // 3. Step Calculation
        if (hit.collider != null)
        {
            Vector3 desiredPos = hit.point;
            
            // Slope Angle Calculation
            float slopeAngle = Mathf.Atan2(hit.normal.x, hit.normal.y) * Mathf.Rad2Deg;
            Quaternion desiredRot = Quaternion.Euler(0, 0, -slopeAngle);
            
            bool isMoving = Mathf.Abs(player.VelocityX) >= 0.01f;

            if (isMoving && 
                Vector3.Distance(currentFootPos, desiredPos) > stepLength &&
                stepProgress >= 1f &&
                (oppositeLeg == null || !oppositeLeg.IsStepping))
            {
                stepProgress = 0f;
                nextFootPos = desiredPos;
                nextFootRot = desiredRot;

                // Lock onto the platform we just stepped on
                connectedPlatform = hit.collider.transform;
                lastPlatformPosition = connectedPlatform.position;

                // 4. Anti-Clipping Stair Check
                float facingDirection = Mathf.Sign(player.VelocityX);
                Vector2 footRayOrigin = currentFootPos + (Vector3.up * 0.1f); 
                
                RaycastHit2D wallHit = Physics2D.Raycast(footRayOrigin, new Vector2(facingDirection, 0), wallCheckDistance, groundLayer);
                
                if (wallHit.collider != null)
                {
                    // We hit a stair! Boost the step height to clear the obstacle
                    dynamicStepHeight = defaultStepHeight + Mathf.Abs(wallHit.point.y - currentFootPos.y) + 0.1f;
                }
                else
                {
                    dynamicStepHeight = defaultStepHeight; // Flat ground
                }
            }
        }

        // 5. Execution & Animation
        if (stepProgress < 1f)
        {
            stepProgress += Time.deltaTime * stepSpeed;
            float t = Mathf.Clamp01(stepProgress);

            Vector3 footPos = Vector3.Lerp(currentFootPos, nextFootPos, t);
            
            // Use the dynamic height (taller if stepping over stairs)
            footPos.y += Mathf.Sin(t * Mathf.PI) * dynamicStepHeight; 

            footTarget.position = footPos;
            
            if (footSpriteTransform != null)
            {
                footSpriteTransform.rotation = Quaternion.Lerp(currentFootRot, nextFootRot, t);
            }

            if (t >= 1f)
            {
                currentFootPos = nextFootPos;
                currentFootRot = nextFootRot;
            }
        }
        else
        {
            footTarget.position = currentFootPos;
            if (footSpriteTransform != null)
            {
                footSpriteTransform.rotation = currentFootRot;
            }
        }
    }
}