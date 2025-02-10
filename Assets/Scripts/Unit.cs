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
    public Color basicColor;

    public float attackDamage = 10f; //공격력
    public float attackRange = 5f;   //공격 범위
    public float moveSpeed = 3f;     //이동 속도

    public UnitState currentState = UnitState.Holding;
    public Monster targetMonster;

    public bool isCanAttack = false; //공격 가능한 유닛인지 여부
    public bool isAtk = false;
    public bool ishold = false;
    private bool isContactMonster = false;

    private Collider col;
    public UnitMove unitMove;
    public IAtk atk;

    private void Awake()
    {
        col = GetComponent<Collider>();
        unitMove = GetComponent<UnitMove>();
        atk = GetComponent<IAtk>();
        basicColor = GetComponent<Renderer>().material.color;
    }

    private void Update()
    {
        switch (currentState)
        {
            case UnitState.Moving:
                MoveToTargetPos();
                break;

            case UnitState.Attacking:
                AttackTarget();
                break;

            case UnitState.Holding:
                HoldPosition();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 공격 가능한 유닛이 사거리 내 적을 감지하면 공격 상태로 변경
        if (other.CompareTag("Monster") && isCanAttack)
        {
            isContactMonster = true;  //사거리내에적있음
            SetClosestMonsterAsTarget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 적이 사거리에서 벗어나면 공격을 멈춤
        if (other.CompareTag("Monster") && isCanAttack)
        {
            isContactMonster = false;  //사거리내에적없음
            targetMonster = null;
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

    public IEnumerator AttackTarget()
    {
        if (targetMonster == null) yield break;
        isAtk = true;
        yield return new WaitForSeconds(0.5f);
        // 공격 범위 내에 적이 있는지 확인
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);

        bool targetInRange = false;

        // OverlapSphere로 얻은 모든 콜라이더들 중에서 타겟이 있는지 확인
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Monster") && hitCollider.transform == targetMonster)
            {
                targetInRange = true;
                break; // 타겟이 공격 범위 내에 있으면 반복문 종료
            }
        }

        // 공격 범위 내에 적이 있으면 공격 실행
        if (targetInRange)
        {
            Debug.Log($"{unitName} is attacking {targetMonster.name}");
            atk.Atk();

        }
        else
        {
            // 공격 범위 내에 적이 없으면 공격 실패
            Debug.Log($"{unitName} failed to attack {targetMonster.name} - out of range");
        }
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
