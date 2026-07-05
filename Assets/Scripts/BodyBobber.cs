using UnityEngine;

public class BodyBobber : MonoBehaviour
{
    [Header("References")]
    public Transform leftFootTarget;
    public Transform rightFootTarget;
    
    [Header("Bob Settings")]
    public float maxDip = 0.3f; // How far down the body drops mid-stride
    public float smoothness = 10f; // How smooth the bouncing looks

    private float defaultY;

    void Start()
    {
        // Store the resting height of the character graphics
        defaultY = transform.localPosition.y;
    }

    void Update()
    {
        // 1. Calculate the horizontal distance between the two feet
        float distanceBetweenFeet = Mathf.Abs(leftFootTarget.position.x - rightFootTarget.position.x);

        // 2. Map that distance to a dip value. 
        // We multiply by a small scale factor to control the severity of the dip.
        float dipAmount = distanceBetweenFeet * maxDip;

        // 3. Calculate where the body should be right now
        float targetY = defaultY - dipAmount;

        // 4. Smoothly move the body up and down
        Vector3 targetPos = new Vector3(transform.localPosition.x, targetY, transform.localPosition.z);
        transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, Time.deltaTime * smoothness);
    }
}