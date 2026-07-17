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

        // Tìm tất cả các nút bấm trong giao diện
        var buttons = uiDoc.rootVisualElement.Query<Button>().ToList();
        
        foreach (var btn in buttons)
        {
            // Bắt sự kiện cho nút Play
            if (btn.text == "Play")
            {
                btn.clicked += PlayGame;
            }
            // Bắt sự kiện cho nút Quit
            else if (btn.text == "Quit")
            {
                btn.clicked += QuitGame;
            }
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
