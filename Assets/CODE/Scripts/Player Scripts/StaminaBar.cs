using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{
    [Header("Stamina Settings")]
    [Tooltip("The maximum stamina the player can have.")]
    public int maxStamina = 1000; // Max stamina value
    [Tooltip("The current stamina value of the player.")]
    public int currStamina;       // Current stamina value

    [Header("UI Components")]
    [Tooltip("The UI Slider component representing the stamina bar.")]
    public Slider staminaBar;      // Slider UI component for the stamina bar

    public static StaminaBar instance; // Singleton instance of the StaminaBar

    [Header("Regeneration Settings")]
    [Tooltip("The delay between stamina regeneration ticks (in seconds).")]
    public float regenDelay = 2f;   // Delay before stamina starts regenerating
    [Tooltip("The amount of stamina restored per tick.")]
    public float regenTickRate = 0.1f; // Time interval between each stamina regeneration tick
    [Tooltip("The rate at which stamina regenerates.")]
    public int regenAmountPerTick = 1; // Amount of stamina restored per tick

    private WaitForSeconds regenTick;   // Cached WaitForSeconds to reduce garbage collection
    private Coroutine regenCoroutine;   // Reference to the active stamina regeneration coroutine

    private void Awake()
    {
        // Initialize singleton instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }

        // Cache WaitForSeconds for regeneration tick interval
        regenTick = new WaitForSeconds(regenTickRate);
    }

    private void Start()
    {
        // Initialize stamina bar
        currStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;
    }

    /// <summary>
    /// Reduces stamina by a given amount. Starts stamina regeneration after use.
    /// </summary>
    /// <param name="amount">Amount of stamina to reduce.</param>
    public void UseStamina(int amount)
    {
        if (currStamina - amount >= 0)
        {
            currStamina -= amount;
            staminaBar.value = currStamina;

            // Stop existing regeneration coroutine if it's running
            if (regenCoroutine != null)
            {
                StopCoroutine(regenCoroutine);
            }

            // Start a new regeneration coroutine
            regenCoroutine = StartCoroutine(RegenStamina());
        }
        else
        {
            Debug.Log("Not enough stamina!");
        }
    }

    /// <summary>
    /// Regenerates stamina over time after a delay.
    /// </summary>
    private IEnumerator RegenStamina()
    {
        // Wait for a delay before starting regeneration
        yield return new WaitForSeconds(regenDelay);

        // Regenerate stamina gradually until full
        while (currStamina < maxStamina)
        {
            currStamina += regenAmountPerTick;
            staminaBar.value = currStamina;

            yield return regenTick;
        }

        // Nullify the coroutine reference once regeneration is complete
        regenCoroutine = null;
    }
}
