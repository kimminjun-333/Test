using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonsterSpwaner : MonoBehaviour
{
    public GameObject monsterPrefab;

    // �ʵ�� �̵��� ����Ʈ���� ����
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
            print($"���� Wave : {wave} ����");
            for (int i = 0; i < maxWaveMonsterCount; ++i)
            {
                MonsterSpwan();
                yield return new WaitForSeconds(1f);
                curWaveTime++;
            }
            yield return new WaitForSeconds(waveTime - curWaveTime);
            print($"���� Wave : {wave} ����");
            wave++;
        }
        print($"���� Wave : {wave} ������ Wave ����");
    }

    public void MonsterSpwan()
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
