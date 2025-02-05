using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Transform unitUIContainer;  // 하단 UI 컨테이너
    public GameObject unitUIItemPrefab; // 유닛 UI 항목 프리팹

    private void Awake()
    {
        instance = this;
    }

    // 선택된 유닛들을 하단 UI에 표시
    public void UpdateUnitUI(List<Unit> selectedUnits)
    {
        // 기존 UI 항목 지우기
        foreach (Transform child in unitUIContainer)
        {
            Destroy(child.gameObject);
        }

        // 티어별로 내림차순 정렬 후, 그 안에서 이름순으로 정렬
        var sortedUnits = selectedUnits
            .OrderByDescending(unit => unit.tier)  // 티어 내림차순
            .ThenBy(unit => unit.unitName)         // 이름순으로 정렬
            .ToList();

        // 정렬된 유닛들을 UI에 표시
        foreach (Unit unit in sortedUnits)
        {
            // 유닛 UI 아이템 생성 (프리팹 인스턴스화)
            GameObject unitUIItem = Instantiate(unitUIItemPrefab, unitUIContainer);

            // UIIcon 컴포넌트 참조
            UIIcon uiIcon = unitUIItem.GetComponent<UIIcon>();

            // 유닛 데이터를 UIIcon에 전달
            uiIcon.unit = unit; // 유닛 데이터 연결

            // UIIcon에서 유닛 정보 반영 (색상, 이미지, 이름 등)
            uiIcon.UpdateUI();
        }
    }
}
