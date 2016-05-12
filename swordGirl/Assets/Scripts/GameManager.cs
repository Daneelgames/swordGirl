using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public float playerHealth = 1;
    public float playerStamina = 1;

    [SerializeField]
    private float healthRecoveryRate = .01f;
    [SerializeField]
    private float staminaRecoveryRate = .2f;

    [SerializeField]
    private Image playerHealthBar;
    [SerializeField]
    private Image playerStaminaBar;

    void Update()
    {
        StatsRecovery();
        SetBarsValues();
    }

    void StatsRecovery()
    {
        if (playerStamina < 1)
            playerStamina += staminaRecoveryRate * Time.deltaTime;
        else if (playerStamina > 1)
            playerStamina = 1;
        else if (playerStamina < 0)
            playerStamina = 0;

        if (playerHealth < .5)
            playerHealth += healthRecoveryRate * Time.deltaTime;
        else if (playerHealth > 1)
            playerHealth = 1;
    }

    void SetBarsValues()
    {
        playerHealthBar.fillAmount = Mathf.Lerp(playerHealthBar.fillAmount, playerHealth, 1f * Time.deltaTime);
        playerStaminaBar.fillAmount = Mathf.Lerp(playerStaminaBar.fillAmount, playerStamina, 1f * Time.deltaTime);
    }
}
