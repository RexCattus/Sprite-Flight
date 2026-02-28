using UnityEngine;
using UnityEngine.UIElements;
public class GameManager : MonoBehaviour
{
    public float thoigianmax=0f;

    public UIDocument UIdoc; // Tham chiếu đến UI Document chứa Text để hiển thị điểm số
    private Label MaxScore;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thoigianmax=PlayerPrefs.GetFloat("HighScore",0f); // Lấy điểm cao nhất đã lưu, mặc định là 0 nếu chưa có
        MaxScore=UIdoc.rootVisualElement.Q<Label>("MaxScore");
        end_work(thoigianmax); // Hiển thị điểm cao nhất trên UI khi bắt đầu trò chơi
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void end_work(float thoigian)
    {
        if (thoigian>thoigianmax)
        {
            PlayerPrefs.SetFloat("HighScore", thoigian); // Lưu điểm cao nhất mới nếu vượt qua điểm cũ

            PlayerPrefs.Save();

            MaxScore.text="New High Score: "+thoigian; // Cập nhật điểm cao nhất trên UI
        } else
        {
            MaxScore.text="High Score: "+thoigianmax; // Hiển thị điểm cao nhất hiện tại trên UI    
        }
    }
}
