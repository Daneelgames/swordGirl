using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public float playerHealth = 1;
    public float playerStamina = 1;

    public float bossHealth = 1;
    
    public float mouseCamSens = 3;
    public float gamepadCamSens = 15;

    public bool bossCanBeDamaged = false;

    [SerializeField]
    private float healthRecoveryRate = .01f;
    [SerializeField]
    private float staminaRecoveryRate = .2f;

    [SerializeField]
    private Image playerHealthBar;
    [SerializeField]
    private Image playerStaminaBar;

    [SerializeField]
    private Image bossHealthBar;

    [SerializeField]
    private Image fader;

    [SerializeField]
    private Text titles;

    bool bossAwake = false;
    GameObject bossHealthBack;

    bool gamepad = false;
    bool pause = false;

    private GameObject pauseMenu;

    private bool playerDead = false;

    void Start()
    {
        Cursor.visible = false;

        StartCoroutine("TitlesStart");
        fader.color = new Color(fader.color.r, fader.color.g, fader.color.b, 255);

        pauseMenu = GameObject.Find("PauseMenuPanel");
        pauseMenu.SetActive(false);

        bossHealthBack = bossHealthBar.transform.parent.gameObject;
        bossHealthBack.SetActive(false);
    }

    void Update()
    {
        StatsRecovery();
        SetBarsValues();

        if (playerDead)
        {
            fader.color = Color.Lerp(fader.color, Color.black, Time.deltaTime);
        }
        else if (fader.color.a > 0)
        {
            fader.color = Color.Lerp(fader.color, Color.clear, Time.deltaTime);
        }

        if (Input.GetButtonDown("Menu"))
        {
            TogglePause();
        }
        
        if (pause)
        {
            if (Input.GetButtonDown("Roll"))
                TogglePause();
        }

       // GameOver();
    }
    
    void StatsRecovery()
    {
        if (!playerDead)
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
    }

    void SetBarsValues()
    {
        playerHealthBar.fillAmount = Mathf.Lerp(playerHealthBar.fillAmount, playerHealth, 5f * Time.deltaTime);
        playerStaminaBar.fillAmount = Mathf.Lerp(playerStaminaBar.fillAmount, playerStamina, 5f * Time.deltaTime);

    //    if (bossAwake)
    //        bossHealthBar.fillAmount = Mathf.Lerp(bossHealthBar.fillAmount, bossHealth, 5f * Time.deltaTime);
    }
    
    public void ShowBossHealth()
    {
     //   bossHealthBack.SetActive(true);
     //   bossAwake = true;
    }
    public void TogglePause()
    {
        if (pause)
        {
            pause = false;
            pauseMenu.SetActive(false);
            AudioListener.volume = 1f;
            Time.timeScale = 1;
            Cursor.visible = false;
        }
        else
        {
            pause = true;
            pauseMenu.SetActive(true);
            AudioListener.volume = 0.25f;
            Time.timeScale = 0;
            Cursor.visible = true;
        }
    }

    public void ToggleGamepad()
    {
        if (gamepad)
        {
            gamepad = false;
            GameObject.Find("CamHolder").GetComponent<CameraController>().camRotateSpeed = mouseCamSens;
        }
        else
        {
            gamepad = true;
            GameObject.Find("CamHolder").GetComponent<CameraController>().camRotateSpeed = gamepadCamSens;
        }
    }

    public void HealPlayer()
    {
        playerHealth += 0.5f;
    }

    public void BossDamaged(float damage)
    {
        if (bossCanBeDamaged)
            bossHealth -= damage;

        if (bossHealth <= 0)
            GameOver();
    }

    public void PlayerDamaged(float dmg)
    {
        playerHealth -= dmg;

        if (playerHealth <= 0)
            GameOver();
    }
    
    void GameOver()
    {
        if (playerHealth <= 0)
        {
            GameObject.Find("Player").GetComponentInChildren<Animator>().SetBool("Alive", false);
            playerDead = true;
            print("Game Over: player lose");
            StartCoroutine("Restart");
        }
        else if (bossHealth <= 0)
        {
            GameObject.Find("AngelKing").GetComponentInChildren<Animator>().SetTrigger("Dead");
            print("Game Over: player win");
            StartCoroutine("TitlesEnd");
        }
    }

    IEnumerator Restart()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(0);
    }

    IEnumerator TitlesStart()
    {
        titles.text = "UNROTTENSKULLS";
        yield return new WaitForSeconds(2f);
        titles.text = "";
        yield return new WaitForSeconds(0.5f);
        titles.text = "GAME BY DA NEEL\n@daneelgames";
        yield return new WaitForSeconds(2f);
        titles.text = "";
    }

    IEnumerator TitlesEnd()
    {
        yield return new WaitForSeconds(3f);
        titles.text = "UNROTTENSKULLS";
        yield return new WaitForSeconds(2f);
        titles.text = "";
        yield return new WaitForSeconds(0.5f);
        titles.text = "GAME BY DA NEEL\n@daneelgames";
        yield return new WaitForSeconds(2f);
        titles.text = "";
        yield return new WaitForSeconds(0.5f);
        titles.text = "THE END\n2016";
    }
}