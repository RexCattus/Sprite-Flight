using UnityEngine;
using UnityEngine.InputSystem;

public class PathFindingManager : MonoBehaviour
{
    public Transform player;
    public Transform target;
    MyPathGrid grid;

    void Awake()
    {
        grid = GetComponent<MyPathGrid>();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 mousePos = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                target.position = hit.point;
                grid.FindPath(player.position, target.position);
            }
        }
    }
}
