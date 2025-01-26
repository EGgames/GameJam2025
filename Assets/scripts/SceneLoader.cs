using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static string sceneToLoad; // Variable estática para almacenar la escena destino

    public void LoadSceneWithLoadingScreen(string targetScene)
    {
        sceneToLoad = targetScene; // Almacena la escena destino
        SceneManager.LoadScene("GameLoading"); // Carga la pantalla de carga
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Juego cerrado."); // Mensaje para depuración
    }
}
