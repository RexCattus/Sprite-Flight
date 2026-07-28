using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenu : MonoBehaviour
{
    public UIDocument uiDoc;

    void Start()
    {
        if (uiDoc == null)
        {
            uiDoc = GetComponent<UIDocument>();
        }

        Button playBtn = uiDoc.rootVisualElement.Q<Button>("Play");
        if (playBtn != null)
        {
            playBtn.clicked += PlayGame;
        }

        Button quitBtn = uiDoc.rootVisualElement.Q<Button>("Quit");
        if (quitBtn != null)
        {
            quitBtn.clicked += QuitGame;
        }
    }

    private void PlayGame()
    {
        // Chuyển sang Scene có tên là "Play"
        SceneManager.LoadScene("Play");
    }

    private void QuitGame()
    {
        Debug.Log("Đã bấm nút Quit! (Trong Editor sẽ không tắt, ra game thật mới tắt)");
        Application.Quit();
    }
}
