using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    float GetActivationChance(); //��ų �ߵ� Ȯ��
    bool RequiresTarget();//Ÿ���� �ʿ�����

    void ExecuteSkill(Unit unit, Monster target = null);
}
