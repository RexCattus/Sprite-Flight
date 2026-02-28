using UnityEngine;

public class GameManager1 : MonoBehaviour
{
    public GameObject rock;
    public float timeSpawn=1.5f;
    public float gioihanY=10f;
    public float vitrispawnX=10f;
    private float timeCount=0f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeCount+=Time.deltaTime;
        if(timeCount>=timeSpawn)
        {
            timeCount=0f;
            taoRock();
        }
        
    }
    void taoRock()
    {
        float y=Random.Range(-gioihanY,gioihanY);
        Vector3 vitriSpawn=new Vector3(vitrispawnX,y,0f);
        Instantiate(rock,vitriSpawn,Quaternion.identity);
    }
}
