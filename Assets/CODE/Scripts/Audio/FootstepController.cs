using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepController : MonoBehaviour
{
    public AudioSource footstepSource;   // The audio source that will play the footstep sound
    public AudioClip[] footstepSounds;   // Array of footstep sounds to randomly choose from
    public float stepInterval = 0.5f;    // Interval between footsteps (based on walking speed)
    private float stepCooldown = 0f;     // Cooldown timer to space footsteps

    private Rigidbody rb;                // Reference to the Rigidbody component

    void Start()
    {
        // Get the Rigidbody component attached to the player
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        // Check if the player is grounded and moving (based on Rigidbody velocity)
        if (IsMoving() && IsGrounded())
        {
            stepCooldown -= Time.deltaTime;

            if (stepCooldown <= 0f)
            {
                PlayFootstep();
                stepCooldown = stepInterval;  // Reset the cooldown based on the player's speed
            }
        }
    }

    bool IsMoving()
    {
        // Check if the player's velocity is above a small threshold (to avoid playing sounds when idle)
        return rb.velocity.magnitude > 0.1f;
    }

    bool IsGrounded()
    {
        // Simple check to see if the player is grounded by raycasting down
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    void PlayFootstep()
    {
        // Choose a random footstep sound from the array
        int randomIndex = Random.Range(0, footstepSounds.Length);
        footstepSource.PlayOneShot(footstepSounds[randomIndex]);
    }
}
