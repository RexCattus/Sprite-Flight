using UnityEngine;

public class Endless_BG : MonoBehaviour
{
    public float spd = 5f;
    private float chieurong;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        chieurong=GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.left * spd * Time.deltaTime);
        if(transform.position.x<= -chieurong)
        {
            transform.Translate(Vector3.right*chieurong*2);
        }
    }
}
