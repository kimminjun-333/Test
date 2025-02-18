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

    public float attackDamage = 10f; //���ݷ�
    public float attackRange = 5f;   //���� ����
    public float moveSpeed = 3f;     //�̵� �ӵ�

    public UnitState currentState = UnitState.Holding;
    public Monster targetMonster;

    public bool isCanAttack = false; //���� ������ �������� ����
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

    // ��ų�� Ȯ���� ���� �ߵ��ϴ� �޼���
    public void TryActivateSkill()
    {
        foreach (var skill in skills)
        {
            if (Random.value <= skill.GetActivationChance())
            {
                // ��ų �ߵ� �� Ÿ���� ���� ���� �ֱ� ������, ���ǿ� �°� ó��
                if (targetMonster != null)
                {
                    skill.ExecuteSkill(this, targetMonster);  // Ÿ���� ���� ���
                }
                else
                {
                    skill.ExecuteSkill(this);  // Ÿ���� ���� ���
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

        print($"���ݽ��� Ÿ�� : {targetMonster}");
        isAtk = true;
        yield return new WaitForSeconds(0.2f);

        Debug.Log($"{unitName} is attacking {targetMonster.name}");
        Atk();

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

    private void LookAtTarget()
    {
        if (targetMonster == null) return;

        // Ÿ���� y ���� �����ϰ�, Ÿ���� x, z ��ǥ�θ� ȸ���ϵ��� ���
        Vector3 targetPosition = new Vector3(targetMonster.transform.position.x, transform.position.y, targetMonster.transform.position.z);

        // ���� ��ġ���� Ÿ���� �ٶ󺸵��� ȸ��
        transform.LookAt(targetPosition);
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
