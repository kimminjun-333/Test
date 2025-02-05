using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIIcon : MonoBehaviour
{
    public Unit unit;

    public Image background;  // 배경 이미지
    public Image icon;        // 유닛 이미지 (아이콘)
    public Text unitNameText; // 유닛 이름 텍스트

    // 유닛 데이터에 맞춰 UI 아이콘을 업데이트
    public void UpdateUI()
    {
        // 유닛이 아직 설정되지 않았다면 반환
        if (unit == null)
            return;

        // 유닛의 티어에 따라 배경 색상 설정
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

        // 유닛의 아이콘 이미지 설정
        icon.sprite = unit.unitImage;

        // 유닛 이름 텍스트 설정
        unitNameText.text = unit.unitName;
    }
}
