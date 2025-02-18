using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MonsterState
{
    Moving,  // �̵� ����
    Stunned  // ���� ����
}

public class Monster : MonoBehaviour, ITakeDamage
{
    public float moveSpeed;  // �⺻ �̵� �ӵ�
    public float curHp;  // ���� ü��
    private float maxHp;  // �ִ� ü��

    private Vector3[] waypoints = new Vector3[4];  // ��� ����Ʈ �迭
    private int currentWaypointIndex = 0;  // ���� ��ǥ ����Ʈ �ε���

    public MonsterState curState = MonsterState.Moving;  // ���� ���� (�̵� �Ǵ� ����)
    private float stunDuration = 0f;  // ���� ���� ���� �ð�

    // �̵� �ӵ� ���� ����
    private List<SpeedReduction> speedReductions = new List<SpeedReduction>();  // �̵� �ӵ� ���� ȿ����
    private float finalMoveSpeed;  // ���� �̵� �ӵ� (�̵� �ӵ� ���Ұ� ����� ��)

    private void Awake()
    {
        maxHp = curHp;  // ������ �ִ� ü���� �ʱ�ȭ
    }

    private void Start()
    {
        // UnitManager���� ���� ����Ʈ�� �ڽ��� �߰�
        UnitManager.instance.monsters.Add(this);
        finalMoveSpeed = moveSpeed;  // �⺻ �̵� �ӵ��� �ʱ�ȭ
    }

    private void Update()
    {
        // �̵� �ӵ� ���� ó��
        HandleSpeedReduction();

        switch (curState)
        {
            case MonsterState.Moving:
                MoveMonster();  // �̵� ������ �� �̵� ó��
                break;

            case MonsterState.Stunned:
                HandleStun();  // ���� ������ �� ���� ó��
                break;
        }
    }

    private void MoveMonster()
    {
        // ���� ��ǥ ����
        Vector3 targetPosition = waypoints[currentWaypointIndex];

        // ��ǥ ���� ��� (y�� ����)
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0; // y�� ȸ�� ����

        // ��ǥ ������ ���� �ε巴�� ȸ�� (ȸ�� �ӵ� ����)
        if (direction.magnitude > 0.1f)  // ����� ��������� �ʾ��� ���� ȸ��
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);  // ȸ�� �ӵ� 10f�� �� ������ ȸ��
        }

        // ��ǥ ������ �����ߴ��� Ȯ��
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            // ��ǥ ������ �����ϸ� ���� ��ǥ�� ����
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }

        // ��ǥ �������� �ε巴�� �̵�
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, finalMoveSpeed * Time.deltaTime);
    }

    private void HandleStun()
    {
        stunDuration -= Time.deltaTime;  // ���� �ð� ����

        // ���� �ð��� �� ������ �̵� ���·� ��ȯ
        if (stunDuration <= 0f)
        {
            curState = MonsterState.Moving;  // ���� ���¿��� �̵� ���·� ��ȯ
        }
    }

    private void HandleSpeedReduction()
    {
        // �̵� �ӵ� ���� ó��
        for (int i = speedReductions.Count - 1; i >= 0; i--)
        {
            SpeedReduction reduction = speedReductions[i];

            // �̵� �ӵ� ���� ȿ���� �����ٸ� ����
            if (reduction.duration <= 0f)
            {
                speedReductions.RemoveAt(i);  // ȿ�� ����
            }
            else
            {
                reduction.duration -= Time.deltaTime;  // ���� ȿ�� �ð� ����
                speedReductions[i] = reduction;  // ����Ʈ�� ���ŵ� ���� �ٽ� ����
            }
        }

        // ���� �̵� �ӵ� ��� (��� �̵� �ӵ� ���� ȿ���� �ݿ�)
        finalMoveSpeed = moveSpeed;  // �⺻ �̵� �ӵ����� ����
        foreach (var reduction in speedReductions)
        {
            finalMoveSpeed *= (1f - reduction.amount);  // �̵� �ӵ� ������ ����
        }
    }

    // �̵� �ӵ� ���Ҹ� �����ϴ� �޼ҵ�
    public void ApplySpeedReduction(float amount, float duration, int skillId)
    {
        bool foundExistingEffect = false;

        // �̹� ������ ��ų�� ����� ���, ���� �ð��� ����
        for (int i = 0; i < speedReductions.Count; i++)
        {
            SpeedReduction reduction = speedReductions[i];

            // ������ ��ųId�� ���� ȿ���� ������ ����
            if (reduction.skillId == skillId)
            {
                reduction.duration = Mathf.Max(reduction.duration, duration);  // �ð��� �� ��� ����
                foundExistingEffect = true;
                break;
            }
        }

        // ������ ��ų�� ������ ���ο� ȿ���� �߰�
        if (!foundExistingEffect)
        {
            speedReductions.Add(new SpeedReduction(amount, duration, skillId));
        }
    }

    // �̵� �ӵ� ���� ȿ���� ��Ÿ���� ����ü
    private struct SpeedReduction
    {
        public float amount;  // �̵� �ӵ� ���� ���� (��: 0.1f == 10% ����)
        public float duration;  // �̵� �ӵ� ���� ���� �ð�
        public int skillId;  // ���� ��ų ID (�̵� �ӵ� ���Ҹ� �߻���Ų ��ų�� �����ϱ� ���� ID)

        public SpeedReduction(float amount, float duration, int skillId)
        {
            this.amount = amount;
            this.duration = duration;
            this.skillId = skillId;
        }
    }

    public void TakeDamage(float damage)
    {
        // ���Ͱ� ���ظ� ���� �� ü�� ����
        curHp -= Mathf.FloorToInt(damage);
    }

    // �ܺο��� ��θ� ������ �� �ִ� �޼ҵ�
    public void SetWaypoints(Vector3[] newWaypoints)
    {
        // ����� �� ����Ʈ�� y ���� ���� ������ y������ ����
        for (int i = 0; i < newWaypoints.Length; i++)
        {
            newWaypoints[i].y = transform.position.y;  // y �� ����
        }

        waypoints = newWaypoints;  // ��� ����
    }

    // ���� ���·� ��ȯ�ϴ� �޼ҵ� (���� �ð��� ���Ͽ� �� ū ������ ������Ʈ)
    public void ApplyStun(float duration)
    {
        if (curState == MonsterState.Stunned)
        {
            // ���� ���� ������ ��, ���� ������ ���� �ð��� ������ ���� ���� �ð����� ��� ������Ʈ
            stunDuration = Mathf.Max(stunDuration, duration);
        }
        else
        {
            // ó�� ������ �޴� ���
            curState = MonsterState.Stunned;
            stunDuration = duration;
        }
    }
}
