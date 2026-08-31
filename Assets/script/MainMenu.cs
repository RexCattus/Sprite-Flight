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
            // playBtn.clicked += () => SceneManager.LoadScene("Play");
            playBtn.clicked += () => SceneTransition.Instance.LoadScene("Play");
        }

        Button shopBtn = uiDoc.rootVisualElement.Q<Button>("Shop");
        if (shopBtn != null)
        {
            //shopBtn.clicked += () => SceneManager.LoadScene("Shop");
            shopBtn.clicked += () => SceneTransition.Instance.LoadScene("Shop");
        }

        Button quitBtn = uiDoc.rootVisualElement.Q<Button>("Quit");
        if (quitBtn != null)
        {
            quitBtn.clicked += () =>
            {
                Debug.Log("Quit game");
                Application.Quit();
            };
        }
    }
}
