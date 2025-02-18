using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpicUnitSkillB : MonoBehaviour, ISkill
{
    public float skillRange = 5f;//공격범위(원형)
    public float skillDamage = 1.5f;//스킬데미지(공격데미지에 곱할값)

    public float speedReductionAmount = 0.3f;//이동속도감소비율
    public float speedReductionDuration = 3f;//이동속도감소시간

    public float skillChance = 0.8f;
    public bool onTarget = true;

    public void ExecuteSkill(Unit unit, Monster target = null)
    {
        print("EpicUnitSkillB 실행");
        Collider[] hitColliders = Physics.OverlapSphere(target.transform.position, skillRange);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Monster"))
            {
                print($"Hit : {collider.gameObject.name}");
                collider.GetComponent<ITakeDamage>().TakeDamage(unit.attackDamage * skillDamage);
                collider.GetComponent<Monster>().ApplySpeedReduction(speedReductionAmount, speedReductionDuration, 401);
            }
        }
    }

    public bool RequiresTarget()
    {
        return onTarget;
    }

    public float GetActivationChance()
    {
        return skillChance;
    }
}
