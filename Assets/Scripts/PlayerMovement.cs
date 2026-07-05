using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Changed this from InputSystem_Actions to PlayerControls!
    private PlayerControls actions; 
    private Rigidbody2D rb;
    
    [Header("Movement Stats")]
    public float moveSpeed = 5f;
    public float jumpForce = 10f;

    private float moveInput;

    public float VelocityX => rb.linearVelocity.x; 

    void Awake()
    {
        // Changed this to match as well
        actions = new PlayerControls(); 
    }

    void OnEnable()
    {
        actions.Player.Enable();
        actions.Player.Move.performed += Movement;
        actions.Player.Move.canceled += Movement;
        actions.Player.Jump.performed += Jumping;
    }

    void OnDisable()
    {
        actions.Player.Disable();
        actions.Player.Move.performed -= Movement;
        actions.Player.Move.canceled -= Movement;
        actions.Player.Jump.performed -= Jumping;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Movement(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>().x;
    }

    void Jumping(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) 
        {
            rb.linearVelocityY = jumpForce; 
        }
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(moveInput * moveSpeed, rb.linearVelocityY);
    }
}