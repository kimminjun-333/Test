using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIcon : MonoBehaviour
{
    public Unit unit;

    public Image background;  // ��� �̹���
    public Image icon;        // ���� �̹��� (������)
    public Text unitNameText; // ���� �̸� �ؽ�Ʈ

    // ���� �����Ϳ� ���� UI �������� ������Ʈ
    public void UpdateUI()
    {
        // ������ ���� �������� �ʾҴٸ� ��ȯ
        if (unit == null)
            return;

        // ������ Ƽ� ���� ��� ���� ����
        Color color = Color.black;
        switch (unit.tier)
        {
            case UnitTier.Common: color = Color.white; break;
            case UnitTier.Rare: color = Color.grey; break;
            case UnitTier.Unique: color = Color.red; break;
            case UnitTier.Epic: color = Color.yellow; break;
            default: color = Color.black; break;
        }
        background.color = color;

        // ������ ������ �̹��� ����
        icon.sprite = unit.unitImage;

        // ���� �̸� �ؽ�Ʈ ����
        unitNameText.text = unit.unitName;
    }
}
