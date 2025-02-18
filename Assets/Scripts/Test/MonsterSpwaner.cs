using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterSpwaner : MonoBehaviour
{
    public GameObject monsterPrefab;

    // 필드로 이동할 포인트들을 설정
    private Vector3[] waypoints = new Vector3[]
    {
        new Vector3(-20, 0, 20),  //0
        new Vector3(-20, 0, -20), //1
        new Vector3(20, 0, -20),  //2
        new Vector3(20, 0, 20)    //3
    };

    private void Start()
    {
        StartCoroutine(WaveStart());
    }

    public IEnumerator WaveStart()
    {
        int wave = 1;
        int maxWave = 60;
        int maxWaveMonsterCount = 40;
        float waveTime = 60f;
        float curWaveTime = 0f;
        while (wave < maxWave)
        {
            print($"현재 Wave : {wave} 시작");
            for (int i = 0; i < maxWaveMonsterCount; ++i)
            {
                MonsterSpwan();
                yield return new WaitForSeconds(1f);
                curWaveTime++;
            }
            yield return new WaitForSeconds(waveTime - curWaveTime);
            print($"현재 Wave : {wave} 종료");
            wave++;
        }
        print($"현재 Wave : {wave} 마지막 Wave 종료");
    }

    public void MonsterSpwan()
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
