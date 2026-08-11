using UnityEngine;

public class ShipSpawner : MonoBehaviour
{
    public GameObject[] Ships;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int selectedShip = PlayerPrefs.GetInt("SelectedShip", 1);
        int index = selectedShip - 1; // Chuyển đổi từ 1-based index sang 0-based index
        if (index >= 0 && index < Ships.Length)
        {
            Instantiate(Ships[index], transform.position, transform.rotation);
        }
        else
        {
            Debug.LogError("Not found ship");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
