using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public float thoigianmax = 0f;
    public UIDocument UIdoc; // Tham chiếu đến UI Document
    private Label MaxScore;
    private Label scoreText;
    private Button Restart;
    private VisualElement fuelFill;

    public float timeSpawn = 1.5f;
    public float gioihanY = 6.5f;
    public float vitrispawnX = 20f;
    public float fuelTimeSpawn = 4.5f; // Tần suất xuất hiện bình xăng
    private float rockTimeCount = 0f;
    private float fuelTimeCount = 0f;
    private float shieldTimeCount = 0f;
    [SerializeField] private float shieldTimeSpawn = 10f; // Tần suất xuất hiện khiên

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        thoigianmax = PlayerPrefs.GetFloat("HighScore", 0f); // Lấy điểm cao nhất đã lưu, mặc định là 0 nếu chưa có
        MaxScore = UIdoc.rootVisualElement.Q<Label>("MaxScore");
        scoreText = UIdoc.rootVisualElement.Q<Label>("ScoreLabel");
        fuelFill = UIdoc.rootVisualElement.Q<VisualElement>("Fuel_Fill");
        Restart = UIdoc.rootVisualElement.Q<Button>("Restart");
        if (Restart != null)
        {
            Restart.style.display = DisplayStyle.None;
            Restart.clicked += RestartGame;
        }
        MaxScore.text = "High Score: " + Mathf.FloorToInt(thoigianmax);
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

        shieldTimeCount += Time.deltaTime;
        if (shieldTimeCount >= shieldTimeSpawn)
        {
            shieldTimeCount = 0f;
            ShieldSpawn();
        }
    }

    private void OnEnable()
    {
        PlayerController.OnPlayerDeath += handlePlayerDeath;
        PlayerController.OnPlayerScoreUpdate += handlePlayerScoreUpdate;
        PlayerController.OnPlayerFuelUpdate += handleFuelUpdate;
    }

    private void OnDisable()
    {
        PlayerController.OnPlayerDeath -= handlePlayerDeath;
        PlayerController.OnPlayerScoreUpdate -= handlePlayerScoreUpdate;
        PlayerController.OnPlayerFuelUpdate -= handleFuelUpdate;
    }

    private void handlePlayerDeath(float score)
    {
        Restart.style.display = DisplayStyle.Flex;
        end_work(score);
    }

    private void handlePlayerScoreUpdate(float score)
    {
        scoreText.text = "Score: " + Mathf.FloorToInt(score); //Cập nhật điểm trên UI
    }

    private void handleFuelUpdate(float currentFuel, float maxFuel)
    {
        if (fuelFill != null)
        {
            float FuelPercentage = (currentFuel / maxFuel) * 100f;
            fuelFill.style.height = new Length(FuelPercentage, LengthUnit.Percent);
        }
    }

    public void end_work(float thoigian)
    {
        if (thoigian > thoigianmax)
        {
            PlayerPrefs.SetFloat("HighScore", thoigian); // Lưu điểm cao nhất mới nếu vượt qua điểm cũ

            PlayerPrefs.Save();

            MaxScore.text = "New High Score: " + Mathf.FloorToInt(thoigian); // Cập nhật điểm cao nhất trên UI
        }
        else
        {
            MaxScore.text = "High Score: " + Mathf.FloorToInt(thoigianmax); // Hiển thị điểm cao nhất hiện tại trên UI    
        }
        int earnedCoins = Mathf.FloorToInt(thoigian / 5);
        int totalCoins = PlayerPrefs.GetInt("PlayerCoins", 0);
        totalCoins += earnedCoins;
        PlayerPrefs.SetInt("PlayerCoins", totalCoins);
        PlayerPrefs.Save();
        Debug.Log("So tien kiem dc la :" + earnedCoins);
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Tải lại scene hiện tại để restart
    }

    void taoRock()
    {
        float y = Random.Range(-gioihanY, gioihanY);
        Vector3 vitriSpawn = new Vector3(vitrispawnX, y, 0f);
        // Instantiate(rock[Random.Range(0, rock.Length)], vitriSpawn, Quaternion.identity);

        int tyLe = Random.Range(1, 101);

        string rockTag = "";
        // Chia Tỉ lệ
        if (tyLe <= 45)
        {
            // Từ 1 đến 45 (Chiếm 45%)
            rockTag = "Rock0";
        }
        else if (tyLe <= 90)
        {
            // Từ 46 đến 90 (Chiếm 45%)
            rockTag = "Rock1";
        }
        else
        {
            // Từ 91 đến 100 (Chiếm 10% còn lại)
            rockTag = "Rock2";
        }

        ObjectPooler.Instance.SpawnFromPool(rockTag, vitriSpawn, Quaternion.identity);
    }

    void FuelSpawn()
    {
        float y = Random.Range(-gioihanY, gioihanY);
        Vector3 vitriSpawn = new Vector3(vitrispawnX, y, 0f);
        // Instantiate(Fuel, vitriSpawn, Quaternion.identity);
        ObjectPooler.Instance.SpawnFromPool("Fuel", vitriSpawn, Quaternion.identity);
    }

    void ShieldSpawn()
    {
        float y = Random.Range(-gioihanY, gioihanY);
        Vector3 vitriSpawn = new Vector3(vitrispawnX, y, 0f);
        // Instantiate(Shield, vitriSpawn, Quaternion.identity);
        ObjectPooler.Instance.SpawnFromPool("Shield", vitriSpawn, Quaternion.identity);
    }
}
