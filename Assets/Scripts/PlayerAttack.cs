using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public GameObject sword;               // Reference to the sword GameObject (child of the player)
    public SpriteRenderer swordSpriteRenderer;  // Reference to the sword's SpriteRenderer
    public BoxCollider2D swordCollider;    // Reference to the sword's BoxCollider2D
    public Animator swordAnimator;         // Reference to the sword's Animator
    public float attackDuration = 0.5f;    // Duration for which the sword is visible during attack
    public LayerMask enemyLayer;           // Layer mask to detect enemies with the sword

    private bool isAttacking = false;      // Flag to check if the player is attacking
    private float attackTimer = 0f;        // Timer to track attack duration
    private bool playerFacingRight = true; // Flag to track player's facing direction

    // Start is called before the first frame update
    void Start()
    {
        // Initially set sword's Z-position in front of the player
        sword.transform.position = new Vector3(sword.transform.position.x, sword.transform.position.y, 0.1f);
        sword.SetActive(false); // Initially, the sword is not visible
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();

        // Handle sword attack timer
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDuration)
            {
                isAttacking = false;
                sword.SetActive(false); // Deactivate sword after the attack duration
                swordAnimator.ResetTrigger("Attack");  // Reset the trigger after the attack ends
            }
        }
    }

    void HandleInput()
    {
        // Trigger attack when left mouse button is clicked
        if (Input.GetMouseButtonDown(0) || Input.GetKey("space")) // Left click
        {
            // Start the attack logic
            StartAttack();

        }

        // Update the player's facing direction based on movement
        UpdatePlayerFacingDirection();

        // Flip the sword and adjust position based on player's facing direction
        FlipSword();
    }

    void StartAttack()
    {
        if (!isAttacking)
        {
            isAttacking = true;
            attackTimer = 0f;

            // Activate sword for the attack duration
            sword.SetActive(true);

            // Trigger the sword animation to play the attack animation
            swordAnimator.SetTrigger("Attack");

            // Detect enemy hit during the attack (Optional)
            DetectEnemyHit();
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red; // Set the color of the Gizmo

        float boxWidth = 1.5f;  // Width of the detection box
        float boxHeight = 1.0f; // Height of the detection box
        float boxDistance = 0.5f; // Distance of the box from the player

        // Determine the position of the detection box based on facing direction
        Vector2 boxCenter = new Vector2(
            transform.position.x + (playerFacingRight ? boxDistance : -boxDistance),
            transform.position.y
        );

        // Define the size of the detection box
        Vector2 boxSize = new Vector2(boxWidth, boxHeight);

        // Draw the detection box
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }



    void DetectEnemyHit()   
    {
        float boxWidth = 1.5f;  // Width of the detection box
        float boxHeight = 1.0f; // Height of the detection box
        float boxDistance = 0.5f; // Distance of the box from the player

        // Determine the position of the detection box based on facing direction
        Vector2 boxCenter = new Vector2(
            transform.position.x + (playerFacingRight ? boxDistance : -boxDistance),
            transform.position.y
        );

        // Create the detection area
        Vector2 boxSize = new Vector2(boxWidth, boxHeight);

        // Detect enemies in the box area
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(boxCenter, boxSize, 0f, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            if (enemy != null)
            {
                Debug.Log("Enemy Hit: " + enemy.name); // Log the enemy hit
                Destroy(enemy.gameObject); // Destroy the enemy
                FindObjectOfType<GameManager>().IncrementKillCount();
                FindObjectOfType<EnemySpawner>().OnEnemyKilled();
            }
        }
    }



    void UpdatePlayerFacingDirection()
    {
        // Check player movement direction to update facing direction
        if (Input.GetAxis("Horizontal") > 0)  // Moving right
        {
            playerFacingRight = true;
        }
        else if (Input.GetAxis("Horizontal") < 0)  // Moving left
        {
            playerFacingRight = false;
        }
    }

    void FlipSword()
    {
        if (playerFacingRight) 
        { 
            swordSpriteRenderer.flipX = false;  // Sword faces right
            sword.transform.localPosition = new Vector3(5.5f, 0.0f, 10f); // Slightly forward for right-facing
        } 
        else
        {
            swordSpriteRenderer.flipX = true;   // Sword faces left
            sword.transform.localPosition = new Vector3(-5.5f, 0.0f, 10f); // Mirror position for left-facing
        }
    }
}
