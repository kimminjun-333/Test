using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAtkRange : MonoBehaviour
{
    public Unit unit;

    private void OnTriggerEnter(Collider other)
    {
        if (unit.isCanAttack == false) return;
        // ���� ������ ������ ��Ÿ� �� ���� �����ϸ� ���� ���·� ����
        if (other.CompareTag("Monster"))
        {
            unit.SetClosestMonsterAsTarget();
            unit.isContactMonster = true;  //��Ÿ�����������
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (unit.isCanAttack == false) return;
        // ���� ��Ÿ����� ����� ������ ����
        if (other.CompareTag("Monster"))
        {
            //unit.targetMonster = null;
            unit.isContactMonster = false;  //��Ÿ�����������
        }
    }
}
