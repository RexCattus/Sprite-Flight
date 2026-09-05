using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class ShopManager : MonoBehaviour
{
    public UIDocument uiDoc;
    private int playerCoins;

    private Label txtCoins;
    private Button btnBack;
    private Button btnShip1;

    // Ship 2 và vv
    private Button btnShip2;
    private Label txtNote2;
    [SerializeField] private int ship2Price = 200;

    private Button btnShip3;
    private Label txtNote3;
    [SerializeField] private int ship3Price = 300;

    private Button btnShip4;
    private Label txtNote4;
    [SerializeField] private int ship4Price = 400;

    void Start()
    {
        var root = uiDoc.rootVisualElement;

        txtCoins = root.Q<Label>("txtCoins");
        btnBack = root.Q<Button>("Back");
        btnShip1 = root.Q<Button>("btnShip1");
        btnShip2 = root.Q<Button>("btnShip2");
        btnShip3 = root.Q<Button>("btnShip3");
        btnShip4 = root.Q<Button>("btnShip4");

        var ship2Element = root.Q<VisualElement>("Ship2");
        if (ship2Element != null) txtNote2 = ship2Element.Q<Label>("Thongbao");
        var ship3Element = root.Q<VisualElement>("Ship3");
        if (ship3Element != null) txtNote3 = ship3Element.Q<Label>("Thongbao");
        var ship4Element = root.Q<VisualElement>("Ship4");
        if (ship4Element != null) txtNote4 = ship4Element.Q<Label>("Thongbao");

        if (txtNote2 != null) txtNote2.text = "";
        if (txtNote3 != null) txtNote3.text = "";
        if (txtNote4 != null) txtNote4.text = "";

        UpdateCoinUI();
        UpdateAllButtons();

        //btnBack.clicked += () => SceneManager.LoadScene("Menu");
        btnBack.clicked += () => SceneTransition.Instance.LoadScene("Menu");

        btnShip1.clicked += OnShip1Clicked;
        btnShip2.clicked += OnShip2Clicked;
        btnShip3.clicked += OnShip3Clicked;
        btnShip4.clicked += OnShip4Clicked;
    }

    // Cập nhật, show tiền
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
                if (txtNote2 != null) txtNote2.text = "";


                // Unlocked 
                PlayerPrefs.SetInt("Ship2_Unlocked", 1);
                PlayerPrefs.Save();

                // Update UI
                UpdateCoinUI();
                UpdateAllButtons();
                Debug.Log("Đã mua thành công");
            }
            else
            {
                Debug.Log("Không đủ tiền!");
                if (txtNote2 != null) txtNote2.text = "Không đủ tiền";
            }
        }
        else // Đã mua thì select
        {
            // 2 = ID đại diện cho máy bay 2
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
                if (txtNote3 != null) txtNote3.text = "";

                PlayerPrefs.SetInt("Ship3_Unlocked", 1);
                PlayerPrefs.Save();

                UpdateCoinUI();
                UpdateAllButtons();
                Debug.Log("Đã mua thành công");
            }
            else
            {
                Debug.Log("Không đủ tiền!");
                if (txtNote3 != null) txtNote3.text = "Không đủ tiền";
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
        int isUnlocked = PlayerPrefs.GetInt("Ship4_Unlocked", 0);

        if (isUnlocked == 0)
        {
            if (playerCoins >= ship4Price)
            {
                playerCoins -= ship4Price;
                PlayerPrefs.SetInt("PlayerCoins", playerCoins);
                if (txtNote4 != null) txtNote4.text = "";

                PlayerPrefs.SetInt("Ship4_Unlocked", 1);
                PlayerPrefs.Save();

                UpdateCoinUI();
                UpdateAllButtons();
                Debug.Log("Đã mua thành công");

            }
            else
            {
                Debug.Log("Không đủ tiền!");
                if (txtNote4 != null) txtNote4.text = "Không đủ tiền";
            }
        }
        else
        {
            PlayerPrefs.SetInt("SelectedShip", 4);
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