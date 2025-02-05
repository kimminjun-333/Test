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
            // ���� UI ������ ���� (������ �ν��Ͻ�ȭ)
            GameObject unitUIItem = Instantiate(unitUIItemPrefab, unitUIContainer);

            // UIIcon ������Ʈ ����
            UIIcon uiIcon = unitUIItem.GetComponent<UIIcon>();

            // ���� �����͸� UIIcon�� ����
            uiIcon.unit = unit; // ���� ������ ����

            // UIIcon���� ���� ���� �ݿ� (����, �̹���, �̸� ��)
            uiIcon.UpdateUI();
        }
    }
}
