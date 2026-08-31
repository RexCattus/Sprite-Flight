using UnityEngine;
using UnityEngine.UIElements;

public class UIButtonSoundController : MonoBehaviour
{
    [SerializeField] private AudioClip clickSound;
    [Range(0f, 1f)][SerializeField] private float clickVolume = 1f;
    [SerializeField] private AudioClip hoverSound;
    [Range(0f, 1f)][SerializeField] private float hoverVolume = 0.3f;
    private UIDocument uiDocument;
    private AudioSource audioSource;

    void Awake()
    {
        uiDocument = GetComponent<UIDocument>();
        audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        uiDocument = GetComponent<UIDocument>();
        VisualElement root = uiDocument.rootVisualElement;

        // Quét toàn bộ Button trong file UXML
        root.Query<Button>().ForEach(button =>
        {
            button.clicked += OnButtonClicked;
            button.RegisterCallback<PointerEnterEvent>(OnButtonHover);
        });
    }

    void OnDisable()
    {
        if (uiDocument == null || uiDocument.rootVisualElement == null) return;

        // Hủy đăng ký để dọn dẹp bộ nhớ
        uiDocument.rootVisualElement.Query<Button>().ForEach(button =>
        {
            button.clicked -= OnButtonClicked;
            button.UnregisterCallback<PointerEnterEvent>(OnButtonHover);

        });
    }

    private void OnButtonClicked()
    {
        PlaySound(clickSound, clickVolume);
    }

    private void OnButtonHover(PointerEnterEvent evt)
    {
        PlaySound(hoverSound, hoverVolume);
    }

    private void PlaySound(AudioClip clip, float volume)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}