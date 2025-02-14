using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSpwan : MonoBehaviour
{
    public GameObject monsterPrefab;
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => TestMonsterSpwan());
    }

    public void TestMonsterSpwan()
    {
        float yPos = monsterPrefab.transform.localScale.y / 2;
        Vector3 pos = new Vector3(0, yPos, 0);
        GameObject monster = Instantiate(monsterPrefab, pos, Quaternion.identity);
    }
}
