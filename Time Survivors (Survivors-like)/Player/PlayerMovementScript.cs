using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    // Movement
    [HideInInspector]
    public float lastHorizMoveVector;
    [HideInInspector]
    public float lastVertMoveVector;
    [HideInInspector]
    public Vector2 moveDirection;
    [HideInInspector]
    public Vector2 lastMovedVector;

    // References
    PlayerStats player;
    Rigidbody2D rb;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerStats>();
        rb = GetComponent<Rigidbody2D>();
        lastMovedVector = new Vector2(1, 0f); // Set the default direction to +x, if we dont and the player does not move, the knife will have no momentum
    }

    // Update is called once per frame
    void Update()
    {
        InputManagement();
    }

    void FixedUpdate()
    {
        Move();
    }

    void InputManagement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveY).normalized;

        if (moveDirection.x != 0)
        {
            lastHorizMoveVector = moveDirection.x;
            lastMovedVector = new Vector2(lastHorizMoveVector, 0f); // Last moved x
        }

        if (moveDirection.y != 0)
        {
            lastVertMoveVector = moveDirection.y;
            lastMovedVector = new Vector2(0f, lastVertMoveVector); // Last moved y
        }
        
        if (moveDirection.x != 0 && moveDirection.y != 0)
        {
            lastMovedVector = new Vector2(lastHorizMoveVector, lastVertMoveVector); // While moving
        }
    }

    void Move()
    {
        if (GameManager.instance.isGameOver)
        {
            return;
        }

        rb.velocity = new Vector2(moveDirection.x * player.CurrentMoveSpeed, moveDirection.y * player.CurrentMoveSpeed);

    }
}
