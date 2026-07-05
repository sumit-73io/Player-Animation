using UnityEngine;

public class ProceduralWalker : MonoBehaviour
{
    [Header("References")]
    public PlayerMovement player;

    public LegController leftLeg;
    public LegController rightLeg;

    public Transform balancePoint;

    [Header("Settings")]
    public float stepDistance = 0.6f;

    private LegController nextLeg;

    private void Awake()
    {
        nextLeg = leftLeg;
    }

    private void Update()
    {
        if (Mathf.Abs(player.VelocityX) < 0.01f)
            return;

        Vector3 body = balancePoint.position;

        float leftDistance =
            Vector2.Distance(body, leftLeg.CurrentFootPosition);

        float rightDistance =
            Vector2.Distance(body, rightLeg.CurrentFootPosition);

        Debug.DrawLine(body,
                       leftLeg.CurrentFootPosition,
                       Color.blue);

        Debug.DrawLine(body,
                       rightLeg.CurrentFootPosition,
                       Color.green);

        if (nextLeg == leftLeg &&
            leftDistance > stepDistance &&
            leftLeg.CanStep)
        {
            Debug.Log("LEFT STEP");

            nextLeg = rightLeg;
        }

        else if (nextLeg == rightLeg &&
                 rightDistance > stepDistance &&
                 rightLeg.CanStep)
        {
            Debug.Log("RIGHT STEP");

            nextLeg = leftLeg;
        }
    }
}