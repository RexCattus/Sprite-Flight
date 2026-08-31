using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    public static SceneTransition Instance;

    [SerializeField] private CanvasGroup fadeCanvasGroup;
    [SerializeField] private float fadeDuration = 0.4f;

    void Awake()
    {
        // Nếu ko có thì Instance sẽ liên tục được tạo mỗi khi Load Scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Do nó ko xoá nên nếu ko xoá thủ công sẽ bị trùng
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(Fade(1f, 0f));
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(TransitionRoutine(sceneName));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        // Tối màn hình
        yield return Fade(0f, 1f);

        // Load Scene
        Time.timeScale = 1f;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // Sáng màn hình
        yield return Fade(1f, 0f);
    }

    private IEnumerator Fade(float startAlpha, float targetAlpha)
    {
        float timer = 0f;
        fadeCanvasGroup.blocksRaycasts = true; // Chặn click tùm lum lúc chuyển cảnh

        while (timer < fadeDuration)
        {
            timer += Time.unscaledDeltaTime; // Dựa trên time IRL
            fadeCanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            yield return null;
        }

        fadeCanvasGroup.alpha = targetAlpha; // Chốt lại giá trị cuối cùng tránh Mathf.Lerp tính thiếu
        fadeCanvasGroup.blocksRaycasts = (targetAlpha == 1f); // Check xem đang ở mức Alpha nào để setup chặn hay gỡ chặn click 
    }
}