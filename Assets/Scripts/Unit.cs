using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState
{
    Idle,      // 대기
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
    public string unitName;    // 유닛 이름
    public UnitTier tier;      // 유닛 티어
    public GameObject unitPrefab;  // 유닛 이미지를 위한 프리팹
    public Sprite unitImage; //유닛 이미지

    public float attackRange = 5f; // 공격 범위
    public float moveSpeed = 3f;
    public UnitState currentState = UnitState.Idle;
    public Monster target;
    public Vector3 targetPos;

    public bool canAttack = false; // 공격 가능한 유닛인지 여부
    public bool isAtk = false;
    public bool ishold = false;
    public bool isMove = false;
    private bool isOnAtk = false;

    private Collider col;

    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    private void Update()
    {
        switch (currentState)
        {
            case UnitState.Moving:
                MoveToTarget();
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
        if (other.CompareTag("Monster") && canAttack)
        {
            isOnAtk = true;  //사거리내에적있음
            SetClosestEnemyAsTarget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 적이 사거리에서 벗어나면 공격을 멈춤
        if (other.CompareTag("Monster") && canAttack)
        {
            isOnAtk = false;  //사거리내에적없음
            target = null;
        }
    }

    public void SetClosestEnemyAsTarget()
    {
        if (UnitManager.instance.monsters.Count == 0) return;  // 몬스터가 없다면 종료

        Monster closestEnemy = null;
        float closestDistance = Mathf.Infinity;  // 가장 가까운 적의 거리

        // 유닛이 소속된 몬스터 리스트에서 가장 가까운 적을 찾는다.
        foreach (Monster enemy in UnitManager.instance.monsters)
        {
            // 적의 위치와 유닛의 위치 사이의 거리 계산
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }

        // 가장 가까운 적을 타겟으로 설정
        if (closestEnemy != null)
        {
            target = closestEnemy;
        }
    }

    public void SetTarget(Vector3 newTarget)
    {
        targetPos = newTarget;
    }

    public void MoveToTarget()
    {
        // 이동할 방향 계산
        Vector3 direction = targetPos - transform.position;
        transform.position += direction.normalized * moveSpeed * Time.deltaTime;
        if (isAtk == true && isOnAtk == true && isMove == false)
        {
            currentState = UnitState.Attacking;
        }
        else if (Vector3.Distance(transform.position, targetPos) <= 0.1f)
        {
            currentState = UnitState.Holding;
        }
    }

    public void AttackTarget()
    {
        if (target == null) return;

        // 공격 범위 내에 적이 있는지 확인
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);

        bool targetInRange = false;

        // OverlapSphere로 얻은 모든 콜라이더들 중에서 타겟이 있는지 확인
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy") && hitCollider.transform == target)
            {
                targetInRange = true;
                break; // 타겟이 공격 범위 내에 있으면 반복문 종료
            }
        }

        // 공격 범위 내에 적이 있으면 공격 실행
        if (targetInRange)
        {
            Debug.Log($"{unitName} is attacking {target.name}");

            // 공격 애니메이션 또는 공격 처리 로직을 여기에 추가
            // 예를 들어, 체력 감소 등의 추가 작업

        }
        else
        {
            // 공격 범위 내에 적이 없으면 공격 실패
            Debug.Log($"{unitName} failed to attack {target.name} - out of range");
        }
    }


    // 홀딩 상태에서 유닛이 이동하지 않고, 적이 있으면 공격만 함
    private void HoldPosition()
    {
        if (!isMove) isMove = false;
        if (isOnAtk)
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
        GetComponent<Renderer>().material.color = Color.white;
    }
}
