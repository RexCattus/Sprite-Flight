using UnityEngine;

public abstract class BaseSkill : MonoBehaviour
{
    public string skillName;
    public float coolDownTime;
    protected float currentCoolDown;

    protected Rigidbody2D rb;

    protected virtual void OnEnable()
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
    }

    protected virtual void Update()
    {
        if (currentCoolDown > 0)
        {
            currentCoolDown -= Time.deltaTime;
        }
    }

    public virtual bool CanUseSkill()
    {
        return currentCoolDown <= 0;
    }

    public abstract void UseSkill();
}
