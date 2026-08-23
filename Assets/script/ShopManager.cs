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
    private int ship2Price = 200;

    private Button btnShip3;
    private int ship3Price = 300;

    private Button btnShip4;
    private int ship4Price = 400;

    void Start()
    {
        var root = uiDoc.rootVisualElement;

        txtCoins = root.Q<Label>("txtCoins");
        txtNote = root.Q<Label>("Thongbao");
        btnBack = root.Q<Button>("Back");
        btnShip1 = root.Q<Button>("btnShip1");
        btnShip2 = root.Q<Button>("btnShip2");
        btnShip3 = root.Q<Button>("btnShip3");
        btnShip4 = root.Q<Button>("btnShip4");
        if(txtNote != null) txtNote.text = "";

        UpdateCoinUI();
        UpdateAllButtons();

        btnBack.clicked += () => SceneManager.LoadScene("Menu");
        btnShip1.clicked += OnShip1Clicked;
        btnShip2.clicked += OnShip2Clicked;
        btnShip3.clicked += OnShip3Clicked;
        btnShip4.clicked += OnShip4Clicked;
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

    void OnShip3Clicked()
    {
        int isUnlocked = PlayerPrefs.GetInt("Ship3_Unlocked", 0);

        if (isUnlocked == 0)
        {
            if (playerCoins >= ship3Price)
            {
                playerCoins -= ship3Price;
                PlayerPrefs.SetInt("PlayerCoins", playerCoins);
                txtNote.text = "";
                
                PlayerPrefs.SetInt("Ship3_Unlocked", 1);
                PlayerPrefs.Save();

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
        else
        {
            PlayerPrefs.SetInt("SelectedShip", 3);
            PlayerPrefs.Save();

            UpdateAllButtons();
            Debug.Log("Đã trang bị máy bay số 3");
        }
    }

    void OnShip4Clicked()
    {
        int isUnlocked = PlayerPrefs.GetInt("Ship4_Unlocked",0);

        if (isUnlocked == 0)
        {
            if (playerCoins >= ship4Price)
            {
            playerCoins -= ship4Price;
            PlayerPrefs.SetInt("PlayerCoins",playerCoins);
            txtNote.text = "";

            PlayerPrefs.SetInt("Ship4_Unlocked",1);
            PlayerPrefs.Save();

            UpdateCoinUI();
            UpdateAllButtons();
            Debug.Log("Đã mua thành công");
            
            }
            else
            {
                Debug.Log("Không đủ tiền!");
                txtNote.text= "Không đủ tiền";
            }
        }
        else 
        {
            PlayerPrefs.SetInt("SelectedShip",4);
            PlayerPrefs.Save();

            UpdateAllButtons();
            Debug.Log("Đã trang bị máy bay số 4");
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

        int Ship3Unlocked = PlayerPrefs.GetInt("Ship3_Unlocked", 0);
        if (Ship3Unlocked == 1)
        {
            if (selectedShip == 3)
            {
                btnShip3.text = "Selected";
            }
            else btnShip3.text = "Select";
        }
        else
        {
            btnShip3.text = "Buy:" + ship3Price;
        }

        int Ship4Unlocked = PlayerPrefs.GetInt("Ship4_Unlocked", 0);
        if (Ship4Unlocked == 1)
        {
            if (selectedShip == 4)
            {
                btnShip4.text = "Selected";
            }
            else btnShip4.text = "Select";
        }
        else
        {
            btnShip4.text = "Buy:" + ship4Price;
        }
    }
}