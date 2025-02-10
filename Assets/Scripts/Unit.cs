using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum UnitState
{
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
    public string unitName;        //���� �̸�
    public UnitTier tier;          //���� Ƽ��
    public Sprite unitImage;       //���� �̹���
    public Color basicColor;

    public float attackDamage = 10f; //���ݷ�
    public float attackRange = 5f;   //���� ����
    public float moveSpeed = 3f;     //�̵� �ӵ�

    public UnitState currentState = UnitState.Holding;
    public Monster targetMonster;

    public bool isCanAttack = false; //���� ������ �������� ����
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
        // ���� ������ ������ ��Ÿ� �� ���� �����ϸ� ���� ���·� ����
        if (other.CompareTag("Monster") && isCanAttack)
        {
            isContactMonster = true;  //��Ÿ�����������
            SetClosestMonsterAsTarget();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // ���� ��Ÿ����� ����� ������ ����
        if (other.CompareTag("Monster") && isCanAttack)
        {
            isContactMonster = false;  //��Ÿ�����������
            targetMonster = null;
        }
    }

    public void SetClosestMonsterAsTarget()
    {
        if (UnitManager.instance.monsters.Count == 0) return;  // ���Ͱ� ���ٸ� ����

        Monster closestMonster = null;
        float closestDistance = Mathf.Infinity;  // ���� ����� ���� �Ÿ�

        // ������ �Ҽӵ� ���� ����Ʈ���� ���� ����� ���� ã�´�.
        foreach (Monster monsters in UnitManager.instance.monsters)
        {
            // ���� ��ġ�� ������ ��ġ ������ �Ÿ� ���
            float distanceToMonster = Vector3.Distance(transform.position, monsters.transform.position);

            if (distanceToMonster < closestDistance)
            {
                closestDistance = distanceToMonster;
                closestMonster = monsters;
            }
        }

        // ���� ����� ���� Ÿ������ ����
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
        // ���� ���� ���� ���� �ִ��� Ȯ��
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange);

        bool targetInRange = false;

        // OverlapSphere�� ���� ��� �ݶ��̴��� �߿��� Ÿ���� �ִ��� Ȯ��
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Monster") && hitCollider.transform == targetMonster)
            {
                targetInRange = true;
                break; // Ÿ���� ���� ���� ���� ������ �ݺ��� ����
            }
        }

        // ���� ���� ���� ���� ������ ���� ����
        if (targetInRange)
        {
            Debug.Log($"{unitName} is attacking {targetMonster.name}");
            atk.Atk();

        }
        else
        {
            // ���� ���� ���� ���� ������ ���� ����
            Debug.Log($"{unitName} failed to attack {targetMonster.name} - out of range");
        }
        yield return new WaitForSeconds(1f);
        isAtk = false;
    }


    // Ȧ�� ���¿��� ������ �̵����� �ʰ�, ���� ������ ���ݸ� ��
    private void HoldPosition()
    {
        if (isContactMonster)
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
        GetComponent<Renderer>().material.color = basicColor;
    }
}
