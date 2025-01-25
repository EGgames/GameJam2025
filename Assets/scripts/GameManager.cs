using UnityEngine;
using TMPro;
using UnityEngine.SocialPlatforms.Impl;

public class GameManager : MonoBehaviour
{
    [Header("Referencias a Elementos de la interfaz gráfica")]
    [Tooltip("Elementos que sean necesarios de la UI")]
    public TMP_Text scoreUI;
    public TMP_Text livesUI;
    public TMP_Text timeUI;

    [Header("Textos por defecto en la UI")]
    [Tooltip("Textos escritos manualmente para administrar posteriormente control de localización")]
    public string scoreTxt;
    public string livesTxt;
    public string timeTxt;


    private int initialLives = 3;
    private int initialScore = 0;
    private int initialTime = 0;
    private float currentTime = 0;

    void Awake()
    {
        //Inicia con contadores por defecto
        scoreUI.text = scoreTxt + initialScore.ToString();
        livesUI.text = livesTxt + initialLives.ToString();
        timeUI.text = timeTxt + initialTime.ToString();
    }

    /// <summary>
    /// Controlador de tiempo transcurrido.
    /// </summary>
    private void Timer()
    {
        currentTime += Time.deltaTime;
        timeUI.text = timeTxt + currentTime.ToString("f0");
    }


    void Update()
    {
        Timer();
    }
}
