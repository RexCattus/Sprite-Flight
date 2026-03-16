using UnityEngine;
using System.Collections.Generic;

public class AStarAgent : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float pathUpdateInterval = 0.5f;

    [Header("Awareness")]
    public float awarenessRadius = 6f;       // Bán kính vùng phát hiện
    public float loseTargetRadius = 8f;      // Bán kính bỏ đuổi (nên > awarenessRadius)
    public LayerMask wallMask;               // Layer tường để raycast
    public string targetTag = "Player";

    [Header("Debug")]
    public bool showGizmos = true;

    // State machine
    enum State { Idle, Alert, Chase }
    State currentState = State.Idle;

    private List<Node> path;
    private int targetIndex;
    private MyPathGrid grid;
    private Transform target;
    private bool hasPath = false;
    private float losePathTimer = 0f;
    private const float LOSE_PATH_TIMEOUT = 3f; // giây trước khi bỏ cuộc nếu không tìm được path

    void Start()
    {
        grid = MyPathGrid.Instance;
        if (grid == null)
        {
            Debug.LogError("AStarAgent: Không tìm thấy MyPathGrid!");
            return;
        }

        GameObject t = GameObject.FindGameObjectWithTag(targetTag);
        if (t != null) target = t.transform;

        InvokeRepeating(nameof(UpdateAwareness), 0f, pathUpdateInterval);
    }

    void UpdateAwareness()
    {
        if (target == null) return;

        float dist = Vector2.Distance(transform.position, target.position);

        switch (currentState)
        {
            case State.Idle:
                // Chỉ alert nếu player trong bán kính VÀ không bị tường chặn
                if (dist <= awarenessRadius && HasLineOfSight())
                {
                    currentState = State.Alert;
                    RequestPath();
                }
                break;

            case State.Alert:
                if (!HasLineOfSight() || dist > awarenessRadius)
                {
                    // Tường chặn hoặc quá xa → quay về Idle
                    currentState = State.Idle;
                    SetPath(null);
                }
                else
                {
                    RequestPath();
                }
                break;

            case State.Chase:
                if (dist > loseTargetRadius)
                {
                    // Quá xa → bỏ đuổi
                    currentState = State.Idle;
                    SetPath(null);
                }
                else
                {
                    RequestPath();
                }
                break;
        }
    }

    bool HasLineOfSight()
    {
        if (target == null) return false;

        Vector3 direction = target.position - transform.position;
        float distance = direction.magnitude;

        if (grid.is2D)
        {
            RaycastHit2D hit = Physics2D.Raycast(
                transform.position,
                direction.normalized,
                distance,
                wallMask
            );
            return hit.collider == null;
        }
        else
        {
            return !Physics.Raycast(
                transform.position,
                direction.normalized,
                distance,
                wallMask
            );
        }
    }

    void RequestPath()
    {
        if (grid != null && target != null)
            grid.FindPath(transform.position, target.position, this);
    }

    public void SetPath(List<Node> newPath)
    {
        path = newPath;
        targetIndex = 0;

        if (newPath != null && newPath.Count > 0)
        {
            hasPath = true;
            losePathTimer = 0f;
            if (currentState == State.Alert)
                currentState = State.Chase; // bắt đầu đuổi khi có path
        }
        else
        {
            hasPath = false;
        }
    }

    void Update()
    {
        if (currentState == State.Idle || !hasPath) return;

        // Timeout nếu không tìm được path quá lâu
        if (!hasPath)
        {
            losePathTimer += Time.deltaTime;
            if (losePathTimer >= LOSE_PATH_TIMEOUT)
            {
                currentState = State.Idle;
                losePathTimer = 0f;
            }
            return;
        }

        if (targetIndex >= path.Count)
        {
            hasPath = false;
            return;
        }

        Vector3 targetPos = path[targetIndex].worldPos;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        float distance = grid.is2D
     ? Vector2.Distance(transform.position, targetPos)
     : Vector3.Distance(transform.position, targetPos);
    }

    void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;

        // Vùng awareness (vàng)
        Gizmos.color = new Color(1f, 0.9f, 0f, 0.25f);
        Gizmos.DrawSphere(transform.position, awarenessRadius);
        Gizmos.color = new Color(1f, 0.9f, 0f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, awarenessRadius);

        // Vùng lose target (đỏ nhạt)
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.1f);
        Gizmos.DrawSphere(transform.position, loseTargetRadius);
        Gizmos.color = new Color(1f, 0.2f, 0.2f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, loseTargetRadius);

        // Raycast line of sight (xanh lá = thấy, đỏ = bị chặn)
        if (target != null)
        {
            bool canSee = HasLineOfSight();
            Gizmos.color = canSee ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, target.position);
        }
    }
}