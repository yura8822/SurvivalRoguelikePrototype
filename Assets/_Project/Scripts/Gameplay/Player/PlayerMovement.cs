using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private PlayerStats stats;
    private Vector2 moveInput;
    private bool isEnabled = true;


    public void Initialize(PlayerStats playerStats)
    {
        stats = playerStats;

        if (stats == null)
        {
            Debug.LogError("[PlayerMovement] PlayerStats is null!");
        }
        else
        {
            Debug.Log($"[PlayerMovement] Initialized with speed: {stats.MoveSpeed}");
        }
        if (spriteRenderer == null) spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!isEnabled) return;

        HandleInput();
        Move();
        UpdateVisuals();
    }


    private void HandleInput()
    {
        moveInput.x = Input.GetAxisRaw("Horizontal");
        moveInput.y = Input.GetAxisRaw("Vertical");

        moveInput.Normalize();
    }


    private void Move()
    {
        if (stats == null) return;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            Vector3 movement = new Vector3(moveInput.x, moveInput.y, 0f) * (stats.MoveSpeed * Time.deltaTime);
            transform.position += movement;
        }
    }
    
    private void UpdateVisuals()
    {
        if (spriteRenderer == null) return;

        if (moveInput.x < -0.01f)
        {
            spriteRenderer.flipX = true;
        }
        else if (moveInput.x > 0.01f)
        {
            spriteRenderer.flipX = false;
        }
    }


    public void SetEnabled(bool enabled)
    {
        isEnabled = enabled;
      
        if (!enabled) moveInput = Vector2.zero;
    }


    public bool IsMoving()
    {
        return isEnabled && moveInput.sqrMagnitude > 0.01f;
    }


    public Vector2 GetMoveDirection()
    {
        return moveInput;
    }


    public float GetCurrentSpeed()
    {
        return stats != null ? stats.MoveSpeed : 0f;
    }
}