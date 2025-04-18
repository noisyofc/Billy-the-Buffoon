using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class kaktus : MonoBehaviour
{
    public float pushForce = 55f;
    private Rigidbody player;

    public Image hitImage;           
    public AudioSource hitAudio;           
    public float effectDuration = 0.2f;

    private float timer;
    private bool isActive;

    public void Update()
    {
        if (player == null)
        {
            player = GameObject.Find("Player").GetComponent<Rigidbody>();
        }

        if (hitImage == null) 
        {
            hitImage = GameObject.Find("HIT").GetComponent<Image>();
        }

        if (hitAudio == null)
        {
            hitAudio = GameObject.Find("HIT").GetComponent<AudioSource>();
        }

        if (isActive)
        {
            timer -= Time.deltaTime;
            if (timer <= 0f)
            {
                hitImage.enabled = false;
                isActive = false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (player != null)
            {                
                Vector3 pushDirection = collision.transform.position - transform.position;
                pushDirection.y = 0;
                pushDirection.Normalize();

                player.AddForce(pushDirection * pushForce, ForceMode.Impulse);

                ShowHitEffect();
            }
        }
    }

    public void ShowHitEffect()
    {
        if (hitImage != null)
        {
            hitImage.enabled = true;
            timer = effectDuration;
            isActive = true;
            hitAudio.Play();
        }
    }
}
