using UnityEngine;

public class PauseController : MonoBehaviour
{
    public GameObject pausePanel;
    
    public void Resume()
    {
        GameManager.Instance.TogglePause();
    }

    public void Restart()
    {
        GameManager.Instance.RestartGame();
    }
    
    public void Exit()
    {
        Application.Quit();
    }
}
