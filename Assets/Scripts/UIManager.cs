using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Transform unitUIContainer;  // �ϴ� UI �����̳�
    public GameObject unitUIItemPrefab; // ���� UI �׸� ������

    private void Awake()
    {
        instance = this;
    }

    // ���õ� ���ֵ��� �ϴ� UI�� ǥ��
    public void UpdateUnitUI(List<Unit> selectedUnits)
    {
        // ���� UI �׸� �����
        foreach (Transform child in unitUIContainer)
        {
            Destroy(child.gameObject);
        }

        // Ƽ��� �������� ���� ��, �� �ȿ��� �̸������� ����
        var sortedUnits = selectedUnits
            .OrderByDescending(unit => unit.tier)  // Ƽ�� ��������
            .ThenBy(unit => unit.unitName)         // �̸������� ����
            .ToList();

        // ���ĵ� ���ֵ��� UI�� ǥ��
        foreach (Unit unit in sortedUnits)
        {
            // ���� UI ������ ����
            GameObject unitUIItem = Instantiate(unitUIItemPrefab, unitUIContainer);

            // ���� �̸� �ؽ�Ʈ ǥ��
            unitUIItem.GetComponentInChildren<Text>().text = unit.unitName;

            // ���� �̹��� ǥ�� (���ֿ� ������ �̹���)
            unitUIItem.GetComponentInChildren<Image>().sprite = unit.unitImage;
        }
    }
}
