using UnityEngine;

public class EnergySkill : BaseSkill
{
    [SerializeField] private GameObject SkillPrefab;

    public override void UseSkill()
    {
        if (CanUseSkill())
        {
            SkillPrefab.SetActive(true);
            currentCoolDown = coolDownTime;
        }
    }

}
