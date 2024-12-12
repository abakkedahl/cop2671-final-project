using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;       // Speed of the player
    public float jumpForce = 7f;       // Force applied when jumping
    private Rigidbody2D rb;            // Reference to the Rigidbody2D component
    public Animator animator;         // Reference to the Animator component
    private bool isGrounded = true;    // Check if the player is on the ground
    public float leftBoundary = -13.2f;
    public float rightBoundary = -1.1f;

    public int maxLives = 3;
    public int currentLives;

    // References to heart GameObjects
    public GameObject heart1;
    public GameObject heart2;
    public GameObject heart3;

    private AudioSource movementAudioSource;  // AudioSource for running sounds
    private AudioSource jumpAudioSource;      // AudioSource for jump sounds
    private AudioSource atkAudioSource;      // AudioSource for attack sounds

    private bool isKnockedBack = false;       // Track if player is in knockback state

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();                   // Get the Rigidbody2D component

        // Find and assign the AudioSource components
        movementAudioSource = GetComponents<AudioSource>()[0]; // First AudioSource, assumed to be for movement
        jumpAudioSource = GetComponents<AudioSource>()[1];     // Second AudioSource, assumed to be for jumping
        atkAudioSource = GetComponents<AudioSource>()[2];     // Third AudioSource, assumed to be for attack

        if (movementAudioSource == null || jumpAudioSource == null)
        {
            Debug.LogError("AudioSource components not found on this GameObject.");
        }

        currentLives = maxLives;

        // Ensure all hearts are active at the start
        heart1.SetActive(true);
        heart2.SetActive(true);
        heart3.SetActive(true);
    }

    void Update()
    {
        if (isKnockedBack) return; // Disable input during knockback

        // Get horizontal input (left/right)
        float moveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y); // Move the player
        animator.SetFloat("Speed", Mathf.Abs(moveInput));

        // Keep the player within defined boundaries
        float clampedX = Mathf.Clamp(transform.position.x, leftBoundary, rightBoundary);
        transform.position = new Vector2(clampedX, transform.position.y);

        // Trigger jump animation if player presses W key and is grounded
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse); // Apply jump force
            animator.SetBool("IsJumping", true);
            isGrounded = false; // Temporarily set to false until landing

            // Play the jump sound effect
            AudioClip jumpSound = Resources.Load<AudioClip>("SoundEffects/jump");
            if (jumpSound != null)
            {
                jumpAudioSource.PlayOneShot(jumpSound);  // Play jump sound on the second AudioSource
            }
            else
            {
                Debug.LogError("Jump sound not found in Resources folder.");
            }
        }

        // Trigger attack sound if player left clicks or presses space
        if (Input.GetMouseButtonDown(0) || Input.GetKey("space"))
        {
            // Play the attack effect
            AudioClip atkSound = Resources.Load<AudioClip>("SoundEffects/Sword_Swing");
            if (atkSound != null)
            {
                atkAudioSource.PlayOneShot(atkSound, 0.175f);  // Adjust volume (0.0 to 1.0)
            }
            else
            {
                Debug.LogError("Attack sound not found in Resources folder.");
            }
        }

        // Flip the character's direction based on movement
        if (moveInput > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
        }
        else if (moveInput < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
        }

        // Play running sound if the player is moving
        if (isGrounded && moveInput != 0)
        {
            if (!movementAudioSource.isPlaying)
            {
                movementAudioSource.Play();  // Play the running sound
            }
        }
        else
        {
            movementAudioSource.Stop();  // Stop the running sound when not moving
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player is on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true; // Player is on the ground
            animator.SetBool("IsJumping", false);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(collision);
        }
    }

    private void TakeDamage(Collision2D collision)
    {
        currentLives--;

        // Determine knockback direction
        Vector2 knockbackDirection = (transform.position.x < collision.transform.position.x) ? Vector2.left : Vector2.right;

        // Apply knockback force
        rb.velocity = Vector2.zero; // Reset velocity before applying force
        rb.AddForce(new Vector2(knockbackDirection.x * 4f, 3f), ForceMode2D.Impulse); // Knockback + slight upward force

        Debug.Log($"Player took damage! Lives remaining: {currentLives}");

        // Freeze movement temporarily
        StartCoroutine(DisableMovement(0.5f));

        // Update heart visibility
        UpdateHearts();

        if (currentLives <= 0)
        {
            Debug.Log("Game Over!");
            // Add game over logic here (e.g., disable player movement, load game over scene)
        }
    }

    private void UpdateHearts()
    {
        // Deactivate hearts based on current lives
        if (currentLives == 2)
        {
            heart3.SetActive(false);
        }
        else if (currentLives == 1)
        {
            heart2.SetActive(false);
        }
        else if (currentLives <= 0)
        {
            heart1.SetActive(false);
        }
    }

    private IEnumerator DisableMovement(float duration)
    {
        isKnockedBack = true;
        yield return new WaitForSeconds(duration);
        isKnockedBack = false;
    }
}
