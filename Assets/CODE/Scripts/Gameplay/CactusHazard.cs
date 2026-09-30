using UnityEngine;
using UnityEngine.UI;

public class CactusHazard : MonoBehaviour
{
    public float pushForce = 55f;

    [Header("Refs")]
    [SerializeField] private Rigidbody player;
    [SerializeField] private Image hitImage;
    [SerializeField] private AudioSource hitAudio;

    private float effectDuration = 0.2f;
    private float timer;
    private bool isActive;

    private void Awake()
    {
        // One-time fallback if refs were not wired in the Inspector
        if (player == null)
        {
            GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
            if (playerGo != null)
                player = playerGo.GetComponent<Rigidbody>();
        }

        if (hitImage == null || hitAudio == null)
        {
            GameObject hitGo = GameObject.Find("HIT");
            if (hitGo != null)
            {
                if (hitImage == null)
                    hitImage = hitGo.GetComponent<Image>();
                if (hitAudio == null)
                    hitAudio = hitGo.GetComponent<AudioSource>();
            }
        }
    }

    public void Update()
    {
        if (!isActive || hitImage == null)
            return;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            hitImage.enabled = false;
            isActive = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player") || player == null)
            return;

        Vector3 pushDirection = collision.transform.position - transform.position;
        pushDirection.y = 0;
        pushDirection.Normalize();

        OptionsManager.TriggerVibration();
        player.AddForce(pushDirection * pushForce, ForceMode.Impulse);
        ShowHitEffect();
    }

    public void ShowHitEffect()
    {
        if (hitImage == null)
            return;

        hitImage.enabled = true;
        timer = effectDuration;
        isActive = true;
        if (hitAudio != null)
            hitAudio.Play();
    }
}
