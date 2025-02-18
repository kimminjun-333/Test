using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterState
{
    Moving,  // 이동 상태
    Stunned  // 스턴 상태
}

public class Monster : MonoBehaviour, ITakeDamage
{
    public float moveSpeed;  // 기본 이동 속도
    public float curHp;  // 현재 체력
    private float maxHp;  // 최대 체력

    private Vector3[] waypoints = new Vector3[4];  // 경로 포인트 배열
    private int currentWaypointIndex = 0;  // 현재 목표 포인트 인덱스

    public MonsterState curState = MonsterState.Moving;  // 현재 상태 (이동 또는 스턴)
    private float stunDuration = 0f;  // 스턴 상태 지속 시간

    // 이동 속도 감소 관련
    private List<SpeedReduction> speedReductions = new List<SpeedReduction>();  // 이동 속도 감소 효과들
    private float finalMoveSpeed;  // 실제 이동 속도 (이동 속도 감소가 적용된 값)

    private void Awake()
    {
        maxHp = curHp;  // 몬스터의 최대 체력을 초기화
    }

    private void Start()
    {
        // UnitManager에서 몬스터 리스트에 자신을 추가
        UnitManager.instance.monsters.Add(this);
        finalMoveSpeed = moveSpeed;  // 기본 이동 속도로 초기화
    }

    private void Update()
    {
        // 이동 속도 감소 처리
        HandleSpeedReduction();

        switch (curState)
        {
            case MonsterState.Moving:
                MoveMonster();  // 이동 상태일 때 이동 처리
                break;

            case MonsterState.Stunned:
                HandleStun();  // 스턴 상태일 때 스턴 처리
                break;
        }
    }

    private void MoveMonster()
    {
        // 현재 목표 지점
        Vector3 targetPosition = waypoints[currentWaypointIndex];

        // 목표 방향 계산 (y축 제외)
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // y축 회전 방지

        // 목표 방향을 향해 부드럽게 회전
        if (direction.magnitude > 0.1f)  // 충분히 가까워지지 않았을 때만 회전
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5f);  // 부드럽게 회전
        }

        // 목표 지점에 도달했는지 확인
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // 목표 지점에 도달하면 다음 목표로 변경
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        // 목표 지점으로 부드럽게 이동
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, finalMoveSpeed * Time.deltaTime);
    }

    private void HandleStun()
    {
        stunDuration -= Time.deltaTime;  // 스턴 시간 감소

        // 스턴 시간이 다 지나면 이동 상태로 전환
        if (stunDuration <= 0f)
        {
            curState = MonsterState.Moving;  // 스턴 상태에서 이동 상태로 전환
        }
    }

    private void HandleSpeedReduction()
    {
        // 이동 속도 감소 처리
        for (int i = speedReductions.Count - 1; i >= 0; i--)
        {
            SpeedReduction reduction = speedReductions[i];

            // 이동 속도 감소 효과가 끝났다면 제거
            if (reduction.duration <= 0f)
            {
                speedReductions.RemoveAt(i);
            }
            else
            {
                reduction.duration -= Time.deltaTime;  // 감소 효과 시간 차감
            }
        }

        // 최종 이동 속도 계산 (모든 이동 속도 감소 효과를 반영)
        finalMoveSpeed = moveSpeed;  // 기본 이동 속도에서 시작
        foreach (var reduction in speedReductions)
        {
            finalMoveSpeed *= (1f - reduction.amount);  // 이동 속도 감소율 적용
        }
    }

    public void TakeDamage(float damage)
    {
        // 몬스터가 피해를 입을 때 체력 감소
        curHp -= Mathf.FloorToInt(damage);
    }

    // 외부에서 경로를 설정할 수 있는 메소드
    public void SetWaypoints(Vector3[] newWaypoints)
    {
        // 경로의 각 포인트의 y 값을 현재 몬스터의 y값으로 고정
        for (int i = 0; i < newWaypoints.Length; i++)
        {
            newWaypoints[i].y = transform.position.y;  // y 값 고정
        }

        waypoints = newWaypoints;  // 경로 설정
    }

    // 스턴 상태로 전환하는 메소드 (스턴 시간을 비교하여 더 큰 값으로 업데이트)
    public void ApplyStun(float duration)
    {
        if (curState == MonsterState.Stunned)
        {
            // 현재 스턴 상태일 때, 새로 들어오는 스턴 시간이 기존의 남은 스턴 시간보다 길면 업데이트
            stunDuration = Mathf.Max(stunDuration, duration);
        }
        else
        {
            // 처음 스턴을 받는 경우
            curState = MonsterState.Stunned;
            stunDuration = duration;
        }
    }

    // 이동 속도 감소를 적용하는 메소드
    public void ApplySpeedReduction(float amount, float duration)
    {
        speedReductions.Add(new SpeedReduction(amount, duration));
    }

    // 이동 속도 감소 효과를 나타내는 구조체
    private struct SpeedReduction
    {
        public float amount;  // 이동 속도 감소 비율 (예: 0.1f == 10% 감소)
        public float duration;  // 이동 속도 감소 지속 시간

        public SpeedReduction(float amount, float duration)
        {
            this.amount = amount;
            this.duration = duration;
        }
    }
}
