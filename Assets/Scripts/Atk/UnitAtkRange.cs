using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAtkRange : MonoBehaviour
{
    public Unit unit;

    private void OnTriggerEnter(Collider other)
    {
        if (unit.isCanAttack == false) return;
        // 공격 가능한 유닛이 사거리 내 적을 감지하면 공격 상태로 변경
        if (other.CompareTag("Monster"))
        {
            unit.SetClosestMonsterAsTarget();
            unit.isContactMonster = true;  //사거리내에적있음
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (unit.isCanAttack == false) return;
        // 적이 사거리에서 벗어나면 공격을 멈춤
        if (other.CompareTag("Monster"))
        {
            //unit.targetMonster = null;
            unit.isContactMonster = false;  //사거리내에적없음
        }
    }
}
