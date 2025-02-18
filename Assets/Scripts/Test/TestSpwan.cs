using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestSpwan : MonoBehaviour
{
    public GameObject monsterPrefab;
    private Button button;

    // 필드로 이동할 포인트들을 설정
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
        // 몬스터의 Y 위치를 소환 위치로 지정
        float yPos = monsterPrefab.transform.localScale.y / 2;

        // 시작 지점은 waypoints[0]으로 설정
        Vector3 spawnPos = new Vector3(waypoints[0].x, yPos, waypoints[0].z);

        // 몬스터 소환
        GameObject monster = Instantiate(monsterPrefab, spawnPos, Quaternion.identity);

        // 소환한 몬스터에 경로 설정
        Monster monsterScript = monster.GetComponent<Monster>();
        if (monsterScript != null)
        {
            // 필드로 정의된 waypoints를 몬스터에 전달
            monsterScript.SetWaypoints(waypoints);
        }
    }
}
