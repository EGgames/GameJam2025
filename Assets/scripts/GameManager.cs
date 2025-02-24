using System;
using System.Collections;
using DefaultNamespace;
using UnityEngine;
using TMPro;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

    //Inicializamos instancia Singleton
    public static GameManager Instance;

    [Header("Configuración del juego")]
    [Tooltip("Duración del juego en segundos")]
    public float gameDuration = 300f;
    [Tooltip(("Referencia al jugador"))]
    public PlayerController player;
    [Tooltip("Referencia al Big Cat")]
    public GameObject bigCat;

    [Header("Referencias a Elementos de la interfaz gráfica")]
    [Tooltip("Barra de vida")]
    public Slider healthBarFill;
    [Tooltip("Texto de tiempo")]
    public TMP_Text timeUI;
    [Tooltip("Indicador de dash")]
    public SpriteRenderer dashIndicator;
   [Tooltip("Indicador de daño al jugador")]
    public Animator damageIndicatorAnimator;
    [Tooltip("Panel de Game Over")]
    public GameObject gameOverPanel;
    [Tooltip("Panel de pausa")]
    public GameObject pausePanel;
    [Tooltip("Panel de victoria")]
    public GameObject winPanel;

    private float currentTime = 0;

    [Header("Puntaje general")]
    [Tooltip("Se administra con esta variable")]
    public int scoreGral;
    [Tooltip("Oleada actual")]
    public int currentWave;
    [Tooltip("Enemigos restantes")]
    public int enemiesLeft;
    
    [Space(10)]
    
    public bool isPaused;

    void Awake()
    {
        //Seteamos velocidad del juego por tiempo de ejecucion
        Time.timeScale = 1f;

        //Ocultamos paneles
        gameOverPanel.SetActive(false);
        pausePanel.SetActive(false);
        winPanel.SetActive(false);
        
        //Inicia con contadores por defecto

        // Configura el Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Controlador de tiempo transcurrido.
    /// </summary>
    private void Timer()
    {
        currentTime += Time.deltaTime;
        // Actualiza el texto de tiempo en la interfaz gráfica
        timeUI.text = TimeSpan.FromSeconds(gameDuration - currentTime).ToString("mm':'ss");
    }

    public void OnEnemyKilled()
    {
        scoreGral++;
        enemiesLeft--;
        if (currentWave == SpawnManager.Instance.waves.Count && enemiesLeft <= 0)
        {
            GameOver(true);
        }
    }

    public void UpdateHealthUI(int newHealth)
    {
        float fillAmount = (float)newHealth / player.maxHealth;
        healthBarFill.value = fillAmount;
    }
    
    public void ShowDamageIndicator(float duration)
    {
        damageIndicatorAnimator.SetBool("IsInvincible", true);
        StartCoroutine(HideDamageIndicator(duration));
    }
    
    private IEnumerator HideDamageIndicator(float duration)
    {
        yield return new WaitForSeconds(duration);
        damageIndicatorAnimator.SetBool("IsInvincible", false);
    }

    public void UpdateDashIndicator(bool isReady)
    {
        dashIndicator.color = new Color(1, 1, 1, isReady ? 1 : 0.1f);
    }

    public void GameOver(bool win = false)
    {
        // Parar la música
        AudioManager.Instance.StopMusic();
        
        if (!win)
        {
            // Agregar audio listener al objeto de la cámara
            Camera.main.gameObject.AddComponent<AudioListener>();
            // Reproducir sonido de muerte del jugador
            AudioManager.Instance.PlaySFX(AudioManager.Instance.playerDeath);
            gameOverPanel.SetActive(true);
        }
        else
        {
            // Reproducir sonido de muerte del jugador
            // AudioManager.Instance.PlaySFX(AudioManager.Instance.playerWin);
            winPanel.SetActive(true);
        }
        
        StartCoroutine(GameOverRoutine());
    }
    
    private IEnumerator GameOverRoutine()
    {
        // Esperamos 2 segundos antes de pausar el juego para que se termine de animar el panel de Game Over
        yield return new WaitForSeconds(2f);
        Time.timeScale = 0f;
    }

    private void Update()
    {
        if (currentTime >= gameDuration)
        {
            bigCat.SetActive(true);
        }
        
        if (currentTime < gameDuration)
        {
            Timer();
        }

        //Puntaje en vivo
        // scoreUI.text = scoreTxt + scoreGral;
    }
    
    public void TogglePause()
    {
        isPaused = !isPaused;
        Time.timeScale = isPaused ? 0 : 1;
        pausePanel.SetActive(isPaused);
        if (isPaused)
        {
            AudioManager.Instance.PauseMusic();
        }
        else
        {
            AudioManager.Instance.PlayMusic();
        }
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
