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
                Obstacle rock = other.gameObject.GetComponent<Obstacle>();
                if (rock != null)
                {
                    rock.BreakBigRock();
                    Destroy(other.gameObject);
                    return;
                }

                DroneEnemy drone = other.gameObject.GetComponent<DroneEnemy>();
                if (drone != null)
                {
                    drone.Die();
                    return;
                }
            }
        }
    }

    private void TurnOffSkill()
    {
        SkillPrefab.SetActive(false);
    }
}
