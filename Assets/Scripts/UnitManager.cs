using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager instance;

    public List<Monster> monsters;

    private void Awake()
    {
        instance = this;
    }
}
