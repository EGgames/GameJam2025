using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;
public class LoadingScreenManager : MonoBehaviour
{
    public Slider progressBar; // Barra de progreso opcional
    public TMP_Text progressText; // Texto opcional para mostrar porcentaje

    void Start()
    {
        StartCoroutine(LoadSceneAsync(SceneLoader.sceneToLoad)); // Carga la escena destino
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
        operation.allowSceneActivation = false;

        while (!operation.isDone)
        {
            // Calcula el progreso (0.9 es el máximo reportado antes de la activación)
            float progress = Mathf.Clamp01(operation.progress / 0.9f);
            if (progressBar != null)
                progressBar.value = progress;
            if (progressText != null)
                progressText.text = (progress * 100f).ToString("F0") + "%";

            // Activa la escena una vez que esté completamente cargada
            if (operation.progress >= 0.9f)
            {
                yield return new WaitForSeconds(1f); // Espera opcional
                operation.allowSceneActivation = true;
            }

            yield return null;
        }
    }
}
