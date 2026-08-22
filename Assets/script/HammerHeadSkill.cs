using UnityEngine;

public class HammerHeadSkill : BaseSkill
{
    [SerializeField] private GameObject SkillPrefab;
    public override void UseSkill()
    {
        if (CanUseSkill())
        {
            SkillPrefab.SetActive(true);
            currentCoolDown = coolDownTime;
            Invoke("TurnOffSkill", 5f);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (SkillPrefab.activeSelf)
            {
                Destroy(other.gameObject);
                other.gameObject.GetComponent<Obstacle>().BreakBigRock();
            }
        }
    }

    private void TurnOffSkill()
    {
        SkillPrefab.SetActive(false);
    }
}
