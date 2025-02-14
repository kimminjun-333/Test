using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpicUnitSkillA : MonoBehaviour, ISkill
{
    public float skillRange = 5f;//���ݹ���(����)
    public float skillDamage = 2f;//��ų������(���ݵ������� ���Ұ�)

    public GameObject atkobj;

    public float skillChance = 0.3f;
    public bool onTarget = true;

    public void ExecuteSkill(Unit unit, Monster target = null)
    {
        print("EpicUnitSkillA ����");

        StartCoroutine(SummonAndStrike(unit, target));

        Collider[] hitColliders = Physics.OverlapSphere(target.transform.position, skillRange);
        foreach (Collider collider in hitColliders)
        {
            if (collider.CompareTag("Monster"))
            {
                print($"Hit : {collider.gameObject.name}");
                collider.GetComponent<ITakeDamage>().TakeDamage(unit.attackDamage * skillDamage);
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

    // atkobj�� ��ȯ�ϰ� Ÿ�� ��ġ�� �̵���Ű�� �ڷ�ƾ
    private IEnumerator SummonAndStrike(Unit unit, Monster target)
    {
        // Ÿ���� ���ʿ� atkobj�� ����
        Vector3 targetPosition = target.transform.position;
        Vector3 spawnPosition = new Vector3(targetPosition.x, targetPosition.y + 5f, targetPosition.z);

        // atkobj�� spawnPosition ��ġ�� ����
        GameObject attackObject = Instantiate(atkobj, spawnPosition, Quaternion.identity);

        // 0.5�� �� Ÿ�� ��ġ�� �̵� ����
        float duration = 0.5f;
        float elapsedTime = 0f;

        // atkobj�� Ÿ�� ��ġ�� �������� ���
        while (elapsedTime < duration)
        {
            attackObject.transform.position = Vector3.Lerp(spawnPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ÿ�� ��ġ�� �����ϸ� ����
        attackObject.transform.position = targetPosition;
        Destroy(attackObject);
    }
}
