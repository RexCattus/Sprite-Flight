using UnityEngine;

public class InstantJumpSkill : BaseSkill
{
    public override void UseSkill()
    {
        if (CanUseSkill())
        {
            rb.AddForce(transform.right * 20f, ForceMode2D.Impulse);
            currentCoolDown = coolDownTime;
        }
    }
}
