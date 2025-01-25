using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{

    //Inicializamos instancia Singleton
    public static GameManager Instance;
    [Header("Referencias a Elementos de la interfaz gráfica")]
    [Tooltip("Elementos que sean necesarios de la UI")]
    public TMP_Text scoreUI;
    public TMP_Text livesUI;
    public TMP_Text timeUI;
    public GameObject gameOverPanel;

    [Header("Textos por defecto en la UI")]
    [Tooltip("Textos escritos manualmente para administrar posteriormente control de localización")]
    public string scoreTxt;
    public string livesTxt;
    public string timeTxt;


    private int initialLives = 3;
    private int initialScore = 0;
    private int initialTime = 0;
    private float currentTime = 0;

    [Header("Puntaje general")]
    [Tooltip("Se administra con esta variable")]
    public int scoreGral;

    void Awake()
    {
        //Seteamos velocidad del juego por tiempo de ejecucion
        Time.timeScale = 1f;

        //Ocultamos GameOverPanel
        gameOverPanel.SetActive(false);
        //Inicia con contadores por defecto
        scoreUI.text = scoreTxt + initialScore.ToString();
        livesUI.text = livesTxt + initialLives.ToString();
        timeUI.text = timeTxt + initialTime.ToString();

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
        timeUI.text = timeTxt + currentTime.ToString("f0");
    }

    public void ScoreCount()
    {
        scoreGral++;
    }



    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    private void Update()
    {
        Timer();

        //Puntaje en vivo
        scoreUI.text = scoreTxt + scoreGral;
    }
}
