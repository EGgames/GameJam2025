using System;
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

    public SpriteRenderer dashIndicator;
    public GameObject gameOverPanel;
    public GameObject pausePanel;

    private float currentTime = 0;

    [Header("Puntaje general")]
    [Tooltip("Se administra con esta variable")]
    public int scoreGral;

    public bool isPaused;

    void Awake()
    {
        //Seteamos velocidad del juego por tiempo de ejecucion
        Time.timeScale = 1f;

        //Ocultamos GameOverPanel
        gameOverPanel.SetActive(false);

        pausePanel.SetActive(false);
        //Inicia con contadores por defecto

        // Configura el Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantener este objeto entre escenas
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

    public void ScoreCount()
    {
        scoreGral++;
    }

    public void UpdateHealthUI(int newHealth)
    {
        float fillAmount = (float)newHealth / player.maxHealth;
        healthBarFill.value = fillAmount;
    }

    public void UpdateDashIndicator(bool isReady)
    {
        dashIndicator.color = new Color(1, 1, 1, isReady ? 1 : 0.1f);
    }

    public void GameOver()
    {
        // Agregar audio listener al objeto de la cámara
        Camera.main.gameObject.AddComponent<AudioListener>();
        // Reproducir sonido de muerte del jugador
        AudioManager.Instance.PlaySFX(AudioManager.Instance.playerDeath);
        // Parar la música
        AudioManager.Instance.StopMusic();
        gameOverPanel.SetActive(true);
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
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
