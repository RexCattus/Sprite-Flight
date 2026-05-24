using UnityEngine;
using UnityEngine.UIElements;
public class GameManager : MonoBehaviour
{
    public float thoigianmax = 0f;

    public UIDocument UIdoc; // Tham chiếu đến UI Document chứa Text để hiển thị điểm số
    private Label MaxScore;
    public GameObject rock;
    public GameObject Fuel;
    public float timeSpawn = 1.5f;
    public float gioihanY = 6.5f;
    public float vitrispawnX = 20f;
    public float fuelTimeSpawn = 4.5f; // Tần suất xuất hiện bình xăng (khoảng 4.5s, ít hơn thiên thạch)
    private float rockTimeCount = 0f;
    private float fuelTimeCount = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thoigianmax = PlayerPrefs.GetFloat("HighScore", 0f); // Lấy điểm cao nhất đã lưu, mặc định là 0 nếu chưa có
        MaxScore = UIdoc.rootVisualElement.Q<Label>("MaxScore");
        end_work(thoigianmax); // Hiển thị điểm cao nhất trên UI khi bắt đầu trò chơi
    }

    // Update is called once per frame
    void Update()
    {
        rockTimeCount += Time.deltaTime;
        if (rockTimeCount >= timeSpawn)
        {
            rockTimeCount = 0f;
            taoRock();
        }

        fuelTimeCount += Time.deltaTime;
        if (fuelTimeCount >= fuelTimeSpawn)
        {
            fuelTimeCount = 0f;
            FuelSpawn();
        }
    }
    public void end_work(float thoigian)
    {
        if (thoigian > thoigianmax)
        {
            PlayerPrefs.SetFloat("HighScore", thoigian); // Lưu điểm cao nhất mới nếu vượt qua điểm cũ

            PlayerPrefs.Save();

            MaxScore.text = "New High Score: " + thoigian; // Cập nhật điểm cao nhất trên UI
        }
        else
        {
            MaxScore.text = "High Score: " + thoigianmax; // Hiển thị điểm cao nhất hiện tại trên UI    
        }
    }
    void taoRock()
    {
        float y = Random.Range(-gioihanY, gioihanY);
        Vector3 vitriSpawn = new Vector3(vitrispawnX, y, 0f);
        Instantiate(rock, vitriSpawn, Quaternion.identity);
    }
    void FuelSpawn()
    {
        float y = Random.Range(-gioihanY, gioihanY);
        Vector3 vitriSpawn = new Vector3(vitrispawnX, y, 0f);
        Instantiate(Fuel, vitriSpawn, Quaternion.identity);
    }
}
