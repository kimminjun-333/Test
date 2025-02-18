using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSpwan : MonoBehaviour
{
    public GameObject monsterPrefab;
    private Button button;

    // �ʵ�� �̵��� ����Ʈ���� ����
    private Vector3[] waypoints = new Vector3[]
    {
        new Vector3(-20, 0, 20),  //0
        new Vector3(-20, 0, -20), //1
        new Vector3(20, 0, -20),  //2
        new Vector3(20, 0, 20)    //3
    };

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => TestMonsterSpwan());
    }

    public void TestMonsterSpwan()
    {
        // ������ Y ��ġ�� ��ȯ ��ġ�� ����
        float yPos = monsterPrefab.transform.localScale.y / 2;

        // ���� ������ waypoints[0]���� ����
        Vector3 spawnPos = new Vector3(waypoints[0].x, yPos, waypoints[0].z);

        // ���� ��ȯ
        GameObject monster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);

        // ��ȯ�� ���Ϳ� ��� ����
        Monster monsterScript = monster.GetComponent<Monster>();
        if (monsterScript != null)
        {
            // �ʵ�� ���ǵ� waypoints�� ���Ϳ� ����
            monsterScript.SetWaypoints(waypoints);
        }
    }
}
