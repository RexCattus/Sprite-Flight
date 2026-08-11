using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public UIDocument uiDoc;
    private int playerCoins;

    private Label txtCoins;
    private Label txtNote;
    private Button btnBack;
    private Button btnShip1;

    // Khai báo cho Máy bay số 2
    private Button btnShip2;
    private int ship2Price = 50;

    void Start()
    {
        var root = uiDoc.rootVisualElement;

        txtCoins = root.Q<Label>("txtCoins");
        txtNote = root.Q<Label>("Thongbao");
        btnBack = root.Q<Button>("Back");
        btnShip1 = root.Q<Button>("btnShip1");
        btnShip2 = root.Q<Button>("btnShip2");

        if(txtNote != null) txtNote.text = "";

        UpdateCoinUI();
        UpdateAllButtons();

        btnBack.clicked += () => SceneManager.LoadScene("Menu");
        btnShip1.clicked += OnShip1Clicked;
        btnShip2.clicked += OnShip2Clicked;
    }

    // Cập nhật và hiển thị tiền
    void UpdateCoinUI()
    {
        playerCoins = PlayerPrefs.GetInt("PlayerCoins", 0);
        if (txtCoins != null)
        {
            txtCoins.text = "Coins:" + playerCoins;
        }
    }



    void OnShip1Clicked()
    {
        PlayerPrefs.SetInt("SelectedShip", 1);
        PlayerPrefs.Save();

        UpdateAllButtons();
        Debug.Log("Đã trang bị máy bay số 1");
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
                txtNote.text = "";
                

                // Mở khóa 
                PlayerPrefs.SetInt("Ship2_Unlocked", 1);
                PlayerPrefs.Save();

                // Cập nhật lại UI
                UpdateCoinUI();
                UpdateAllButtons();
                Debug.Log("Đã mua thành công");
            }
            else
            {
                Debug.Log("Không đủ tiền!");
                txtNote.text = "Không đủ tiền";
            }
        }
        else // Đã mua thì select
        {
            // Số 2 là ID đại diện cho máy bay này
            PlayerPrefs.SetInt("SelectedShip", 2);
            PlayerPrefs.Save();

            UpdateAllButtons();
            Debug.Log("Đã trang bị máy bay số 2");
        }
    }

    void UpdateAllButtons()
    {
        int selectedShip = PlayerPrefs.GetInt("SelectedShip", 1);
        if (selectedShip == 1)
        {
            btnShip1.text = "Selected";
        }
        else 
        {
            btnShip1.text = "Select";
        }

        int Ship2Unlocked = PlayerPrefs.GetInt("Ship2_Unlocked", 0);
        if (Ship2Unlocked == 1)
        {
            if (selectedShip == 2)
            {
                btnShip2.text = "Selected";
            }
            else btnShip2.text = "Select";
        }
        else
        {
            btnShip2.text = "Buy:" + ship2Price;
        }
    }
}