using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState
{
    Moving,    // 이동
    Attacking, // 공격
    Holding    // 홀드
}


public enum UnitTier
{
    Epic = 4,
    Unique = 3,
    Rare = 2,
    Common = 1
}

public class Unit : MonoBehaviour
{
    public string unitName;        //유닛 이름
    public UnitTier tier;          //유닛 티어
    public Sprite unitImage;       //유닛 이미지

    public float attackDamage = 10f; //공격력
    public float attackRange = 5f;   //공격 범위
    public float moveSpeed = 3f;     //이동 속도

    public UnitState currentState = UnitState.Holding;
    public Monster targetMonster;

    public bool isCanAttack = false; //공격 가능한 유닛인지 여부
    public bool isAtk = false;
    public bool ishold = false;
    public bool isContactMonster = false;

    public UnitMove unitMove;

    private Color basicColor;
    private Collider col;

    private List<ISkill> skills = new List<ISkill>();

    private void Awake()
    {
        col = GetComponent<Collider>();
        unitMove = GetComponent<UnitMove>();
        basicColor = GetComponent<Renderer>().material.color;
        skills.AddRange(GetComponents<ISkill>());
    }

    private void Update()
    {
        switch (currentState)
        {
            case UnitState.Moving:
                MoveToTargetPos();
                break;

            case UnitState.Attacking:
                if (isAtk == false)
                {
                    StartCoroutine(AttackTarget());
                }
                break;

            case UnitState.Holding:
                HoldPosition();
                break;
        }
    }

    public void SetClosestMonsterAsTarget()
    {
        if (UnitManager.instance.monsters.Count == 0) return;  // 몬스터가 없다면 종료

        Monster closestMonster = null;
        float closestDistance = Mathf.Infinity;  // 가장 가까운 적의 거리

        // 유닛이 소속된 몬스터 리스트에서 가장 가까운 적을 찾는다.
        foreach (Monster monsters in UnitManager.instance.monsters)
        {
            // 적의 위치와 유닛의 위치 사이의 거리 계산
            float distanceToMonster = Vector3.Distance(transform.position, monsters.transform.position);

            if (distanceToMonster < closestDistance)
            {
                closestDistance = distanceToMonster;
                closestMonster = monsters;
            }
        }
        // 가장 가까운 적을 타겟으로 설정
        if (closestMonster != null)
        {
            targetMonster = closestMonster;
        }
    }

    public void MoveToTargetPos()
    {
        if (isCanAttack && isContactMonster)
        {
            currentState = UnitState.Attacking;
        }
        else
        {
            unitMove.Move();
        }
    }

    // 스킬을 확률에 따라 발동하는 메서드
    public void TryActivateSkill()
    {
        foreach (var skill in skills)
        {
            if (Random.value <= skill.GetActivationChance())
            {
                // 스킬 발동 시 타겟이 없을 수도 있기 때문에, 조건에 맞게 처리
                if (targetMonster != null)
                {
                    skill.ExecuteSkill(this, targetMonster);  // 타겟이 있을 경우
                }
                else
                {
                    skill.ExecuteSkill(this);  // 타겟이 없을 경우
                }
            }
        }
    }

    public void Atk()
    {
        targetMonster.TakeDamage(attackDamage);
        TryActivateSkill();
    }

    public IEnumerator AttackTarget()
    {
        SetClosestMonsterAsTarget();

        if (targetMonster == null) yield break;

        LookAtTarget();

        print($"공격시작 타겟 : {targetMonster}");
        isAtk = true;
        yield return new WaitForSeconds(0.2f);

        Debug.Log($"{unitName} is attacking {targetMonster.name}");
        Atk();

        yield return new WaitForSeconds(1f);
        isAtk = false;
    }


    // 홀딩 상태에서 유닛이 이동하지 않고, 적이 있으면 공격만 함
    private void HoldPosition()
    {
        if (isContactMonster)
        {
            AttackTarget();  // 공격 시작
        }
    }

    private void LookAtTarget()
    {
        if (targetMonster == null) return;

        // 타겟의 y 값을 유지하고, 타겟의 x, z 좌표로만 회전하도록 계산
        Vector3 targetPosition = new Vector3(targetMonster.transform.position.x, transform.position.y, targetMonster.transform.position.z);

        // 현재 위치에서 타겟을 바라보도록 회전
        transform.LookAt(targetPosition);
    }


    public void Select()
    {
        // 유닛 선택시 색상 변화 등 비주얼 처리를 할 수 있음
        GetComponent<Renderer>().material.color = Color.green;
    }

    public void Deselect()
    {
        // 유닛 선택 해제시 색상 변화 등 비주얼 처리를 할 수 있음
        GetComponent<Renderer>().material.color = basicColor;
    }
}
