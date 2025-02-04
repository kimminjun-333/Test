using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState
{
    Idle,      // ���
    Moving,    // �̵�
    Attacking, // ����
    Holding    // Ȧ��
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
    public string unitName;    // ���� �̸�
    public UnitTier tier;      // ���� Ƽ��
    public GameObject unitPrefab;  // ���� �̹����� ���� ������
    public Sprite unitImage; //���� �̹���

    public float attackRange = 5f; // ���� ����
    public float moveSpeed = 3f;
    public UnitState currentState = UnitState.Idle;
    public Monster target;
    public Vector3 targetPos;

    public bool canAttack = false; // ���� ������ �������� ����
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
        // ���� ������ ������ ��Ÿ� �� ���� �����ϸ� ���� ���·� ����
        if (other.CompareTag("Monster") && canAttack)
        {
            isOnAtk = true;  //��Ÿ�����������
            SetClosestEnemyAsTarget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // ���� ��Ÿ����� ����� ������ ����
        if (other.CompareTag("Monster") && canAttack)
        {
            isOnAtk = false;  //��Ÿ�����������
            target = null;
        }
    }

    public void SetClosestEnemyAsTarget()
    {
        if (UnitManager.instance.monsters.Count == 0) return;  // ���Ͱ� ���ٸ� ����

        Monster closestEnemy = null;
        float closestDistance = Mathf.Infinity;  // ���� ����� ���� �Ÿ�

        // ������ �Ҽӵ� ���� ����Ʈ���� ���� ����� ���� ã�´�.
        foreach (Monster enemy in UnitManager.instance.monsters)
        {
            // ���� ��ġ�� ������ ��ġ ������ �Ÿ� ���
            float distanceToEnemy = Vector3.Distance(transform.position, enemy.transform.position);

            if (distanceToEnemy < closestDistance)
            {
                closestDistance = distanceToEnemy;
                closestEnemy = enemy;
            }
        }

        // ���� ����� ���� Ÿ������ ����
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
        // �̵��� ���� ���
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

        // ���� ���� ���� ���� �ִ��� Ȯ��
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);

        bool targetInRange = false;

        // OverlapSphere�� ���� ��� �ݶ��̴��� �߿��� Ÿ���� �ִ��� Ȯ��
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Enemy") && hitCollider.transform == target)
            {
                targetInRange = true;
                break; // Ÿ���� ���� ���� ���� ������ �ݺ��� ����
            }
        }

        // ���� ���� ���� ���� ������ ���� ����
        if (targetInRange)
        {
            Debug.Log($"{unitName} is attacking {target.name}");

            // ���� �ִϸ��̼� �Ǵ� ���� ó�� ������ ���⿡ �߰�
            // ���� ���, ü�� ���� ���� �߰� �۾�

        }
        else
        {
            // ���� ���� ���� ���� ������ ���� ����
            Debug.Log($"{unitName} failed to attack {target.name} - out of range");
        }
    }


    // Ȧ�� ���¿��� ������ �̵����� �ʰ�, ���� ������ ���ݸ� ��
    private void HoldPosition()
    {
        if (!isMove) isMove = false;
        if (isOnAtk)
        {
            AttackTarget();  // ���� ����
        }
    }

    public void Select()
    {
        // ���� ���ý� ���� ��ȭ �� ���־� ó���� �� �� ����
        GetComponent<Renderer>().material.color = Color.green;
    }

    public void Deselect()
    {
        // ���� ���� ������ ���� ��ȭ �� ���־� ó���� �� �� ����
        GetComponent<Renderer>().material.color = Color.white;
    }
}
