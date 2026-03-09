using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public List<Node> path;
    int targetIndex;
    public float spd = 5f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (path != null && targetIndex < path.Count)
        {
            Node targetNode = path[targetIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetNode.worldPos, spd * Time.deltaTime);
            if (transform.position == targetNode.worldPos)
            {
                targetIndex++;
            }
        }
    }
}
