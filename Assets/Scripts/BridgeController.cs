using UnityEngine;

public class BridgeBreaker : MonoBehaviour
{
    [Header("Bridge Settings")]
    [Tooltip("Drag the middle circle (or the specific one you want to break) here in the Inspector.")]
    public GameObject targetCircle;

    [Tooltip("The exact name of your Player's layer.")]
    public string playerLayerName = "Player";

    // A flag to ensure we only break the bridge once
    private bool hasBroken = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. Check if the bridge is already broken
        if (hasBroken) return;

        // 2. Check if the object entering the trigger is on the Player layer
        if (collision.gameObject.layer == LayerMask.NameToLayer(playerLayerName))
        {
            BreakBridge();
        }
    }

    private void BreakBridge()
    {
        if (targetCircle != null)
        {
            // 3. Get ALL HingeJoint2D components attached to the target circle
            HingeJoint2D[] hinges = targetCircle.GetComponents<HingeJoint2D>();

            // 4. Loop through them and disable them
            // foreach (HingeJoint2D hinge in hinges)
            // {
            //     hinge.enabled = false;
                
            //     // Alternatively, if you want to permanently delete the joint instead of disabling it:
            //     // Destroy(hinge);
            // }
            hinges[0].enabled = false; // Disable the first hinge joint to break the bridge

            hasBroken = true;
            Debug.Log("The bridge has snapped!");
        }
        else
        {
            Debug.LogWarning("Target Circle is missing! Please assign it in the inspector.");
        }
    }
}