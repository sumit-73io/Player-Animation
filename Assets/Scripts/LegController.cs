using UnityEngine;

public class LegController : MonoBehaviour
{
    [Header("References")]
    public Transform hip;
    public Transform footTarget;

    [HideInInspector]
    public bool IsStepping;

    [HideInInspector]
    public bool CanStep = true;

    [HideInInspector]
    public Vector3 CurrentFootPosition;

    private void Awake()
    {
        CurrentFootPosition = footTarget.position;
    }
}