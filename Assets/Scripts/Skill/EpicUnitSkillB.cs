using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpicUnitSkillB : MonoBehaviour, ISkill
{
    public float skillDamage = 10f;//스킬데미지(공격데미지에 곱할값)

    public float skillChance = 0.5f;
    public bool onTarget = true;

    public void ExecuteSkill(Unit unit, Monster target = null)
    {
        print("EpicUnitSkillB 실행");
        target.TakeDamage(unit.attackDamage * skillDamage); // 예: 공격력 2배 피해
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
