using UnityEngine;

public class MenuBackground : MonoBehaviour
{
    public GameObject[] backGrounds; // Mảng chứa các background
    public float timeSpawn = 1.5f;
    public float gioihanY = 6.5f;
    public float vitrispawnX = 20f;
    private float backGroundsTimeCount = 0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
         backGroundsTimeCount += Time.deltaTime;
        if (backGroundsTimeCount >= timeSpawn)
        {
            backGroundsTimeCount = 0f;
            float y = Random.Range(-gioihanY, gioihanY);
            Vector3 vitriSpawn = new Vector3(vitrispawnX, y, 0f);
            Instantiate(backGrounds[Random.Range(0, backGrounds.Length)], vitriSpawn, Quaternion.identity);
        }
    }
}
