using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class PauseScript : MonoBehaviour
{
    public UIDocument Pausedoc;
    private VisualElement rootElement;
    private Button resume;
    private Button Quit;
    bool isPaused = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rootElement = Pausedoc.rootVisualElement;
        resume = rootElement.Q<Button>("Resume");
        Quit = rootElement.Q<Button>("Quit");
        resume.clicked += resumeGame;
        Quit.clicked += QuitGame;
        rootElement.style.display = DisplayStyle.None;
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                resumeGame();
            }
            else
            {
                pauseGame();
            }
        }
    }

    void resumeGame()
    {
        Time.timeScale = 1f;
        isPaused = false;
        rootElement.style.display = DisplayStyle.None; // Ẩn UI Pause
    }
    void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }
    void pauseGame()
    {
        Time.timeScale = 0f;
        isPaused = true;
        rootElement.style.display = DisplayStyle.Flex; // Hiển thị UI Pause
    }
}
