using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testscript : MonoBehaviour
{
    public atktest atktest;

    // Start is called before the first frame update
    void Start()
    {
        atktest = gameObject.GetComponent<atktest>();
        Atk();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Atk()
    {
        atktest.Atk();
    }
}
