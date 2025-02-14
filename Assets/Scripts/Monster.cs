using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour, ITakeDamage
{
    public float moveSpeed;
    public float curHp;
    private float maxHp;

    private void Awake()
    {
        maxHp = curHp;
    }

    private void Start()
    {
        UnitManager.instance.monsters.Add(this);
    }

    public void TakeDamage(float damage)
    {
        curHp -= Mathf.FloorToInt(damage);
    }
}
