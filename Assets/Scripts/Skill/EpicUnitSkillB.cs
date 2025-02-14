using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpicUnitSkillB : MonoBehaviour, ISkill
{
    public float skillDamage = 10f;//��ų������(���ݵ������� ���Ұ�)

    public float skillChance = 0.5f;
    public bool onTarget = true;

    public void ExecuteSkill(Unit unit, Monster target = null)
    {
        print("EpicUnitSkillB ����");
        target.TakeDamage(unit.attackDamage * skillDamage); // ��: ���ݷ� 2�� ����
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
