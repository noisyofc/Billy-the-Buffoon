using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StaminaBar : MonoBehaviour
{

    public int maxStamina = 1000;
    public int currStamina;
    public Slider staminaBar;

    public static StaminaBar instance;

    private WaitForSeconds regenTick = new WaitForSeconds(0.1f);
    private Coroutine regen;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currStamina = maxStamina;
        staminaBar.maxValue = maxStamina;
        staminaBar.value = maxStamina;
    }

    public void useStamina(int amount)
    {
        if (currStamina - amount >= 0)
        {
            currStamina -= amount;
            staminaBar.value = currStamina;

            if(regen != null)
            {
                StopCoroutine(regen);
            }
               
        regen = StartCoroutine(RegenStamina());
        }
        else
            Debug.Log("not enough");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator RegenStamina()
    {
        yield return new WaitForSeconds(2);

        while(currStamina < maxStamina)
        {
            currStamina += maxStamina / 800;
            staminaBar.value = currStamina;

            yield return regenTick;
        }
        regen = null;
    }
}
