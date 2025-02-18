using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpicUnitSkillB : MonoBehaviour, ISkill
{
    public float skillRange = 5f;//���ݹ���(����)
    public float skillDamage = 1.5f;//��ų������(���ݵ������� ���Ұ�)

    public float speedReductionAmount = 0.3f;//�̵��ӵ����Һ���
    public float speedReductionDuration = 3f;//�̵��ӵ����ҽð�

    public float skillChance = 0.8f;
    public bool onTarget = true;

    public void ExecuteSkill(Unit unit, Monster target = null)
    {
        print("EpicUnitSkillB ����");
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
