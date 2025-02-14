using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EpicUnitSkillA : MonoBehaviour, ISkill
{
    public float skillRange = 5f;//공격범위(원형)
    public float skillDamage = 2f;//스킬데미지(공격데미지에 곱할값)

    public GameObject atkobj;

    public float skillChance = 0.3f;
    public bool onTarget = true;

    public void ExecuteSkill(Unit unit, Monster target = null)
    {
        print("EpicUnitSkillA 실행");

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

    // atkobj를 소환하고 타겟 위치로 이동시키는 코루틴
    private IEnumerator SummonAndStrike(Unit unit, Monster target)
    {
        // 타겟의 위쪽에 atkobj를 생성
        Vector3 targetPosition = target.transform.position;
        Vector3 spawnPosition = new Vector3(targetPosition.x, targetPosition.y + 5f, targetPosition.z);

        // atkobj를 spawnPosition 위치에 생성
        GameObject attackObject = Instantiate(atkobj, spawnPosition, Quaternion.identity);

        // 0.5초 후 타겟 위치로 이동 시작
        float duration = 0.5f;
        float elapsedTime = 0f;

        // atkobj가 타겟 위치로 내려가는 모션
        while (elapsedTime < duration)
        {
            attackObject.transform.position = Vector3.Lerp(spawnPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // 타겟 위치에 도달하면 삭제
        attackObject.transform.position = targetPosition;
        Destroy(attackObject);
    }
}
