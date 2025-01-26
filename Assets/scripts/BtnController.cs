using UnityEngine;
using UnityEngine.SceneManagement;

public class BtnController : MonoBehaviour
{

    public LoadingScreenManager loadingScreen;


    [Header("Paneles")]
    [Tooltip("Paneles usados en menu principal")]
    public GameObject controlsPanel;


    public void PlayGame()
    {
        SceneManager.LoadScene("Loading");
    }
    
    public void ExitGame()
    {
        Application.Quit();
    }
}
