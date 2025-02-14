using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    float GetActivationChance(); //스킬 발동 확률
    bool RequiresTarget();//타겟이 필요한지

    void ExecuteSkill(Unit unit, Monster target = null);
}
