using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public UIDocument uiDoc;
    private int playerCoins;

    private Label txtCoins;
    private Button btnBack;

    // Khai báo cho Máy bay số 2
    private Button btnShip2;
    private int ship2Price = 50;

    void Start()
    {
        var root = uiDoc.rootVisualElement;

        txtCoins = root.Q<Label>("txtCoins");
        btnBack = root.Q<Button>("btnBack");
        btnShip2 = root.Q<Button>("btnShip2");

        UpdateCoinUI();

        UpdateShip2Button();

        // 4. Gắn hành động khi người chơi bấm nút
        btnBack.clicked += GoBack;
        btnShip2.clicked += OnShip2Clicked;
    }

    // Hàm cập nhật và hiển thị tiền
    void UpdateCoinUI()
    {
        playerCoins = PlayerPrefs.GetInt("PlayerCoins", 0);
        if (txtCoins != null)
        {
            txtCoins.text = "Coins: " + playerCoins;
        }
    }

    // Hàm kiểm tra và đổi chữ trên Nút mua
    void UpdateShip2Button()
    {
        // 0 là chưa mua, 1 là đã mua
        int isUnlocked = PlayerPrefs.GetInt("Ship2_Unlocked", 0);

        if (isUnlocked == 1)
        {
            btnShip2.text = "Select";
        }
        else
        {
            btnShip2.text = "Buy: " + ship2Price; // Chưa mua thì hiện giá tiền
        }
    }

    // Hàm xử lý khi bấm nút Ship 2
    void OnShip2Clicked()
    {
        int isUnlocked = PlayerPrefs.GetInt("Ship2_Unlocked", 0);

        if (isUnlocked == 0) // chưa mua thì thanh toán
        {
            if (playerCoins >= ship2Price)
            {
                // Trừ tiền
                playerCoins -= ship2Price;
                PlayerPrefs.SetInt("PlayerCoins", playerCoins);

                // Mở khóa 
                PlayerPrefs.SetInt("Ship2_Unlocked", 1);
                PlayerPrefs.Save();

                // Cập nhật lại UI
                UpdateCoinUI();
                UpdateShip2Button();
                Debug.Log("Đã mua thành công Siêu Tàu Chiến!");
            }
            else
            {
                Debug.Log("Không đủ tiền!");
                btnShip2.text = "Cần thêm tiền";
            }
        }
        else // Đã mua thì select
        {
            // Số 2 là ID đại diện cho máy bay này
            PlayerPrefs.SetInt("SelectedShip", 2);
            PlayerPrefs.Save();

            btnShip2.text = "Selected!";
            Debug.Log("Đã trang bị Máy bay số 2!");
        }
    }

    void GoBack()
    {
        SceneManager.LoadScene("Menu");
    }
}