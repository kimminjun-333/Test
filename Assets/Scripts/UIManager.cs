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
            // 유닛 UI 아이템 생성
            GameObject unitUIItem = Instantiate(unitUIItemPrefab, unitUIContainer);

            // 유닛 이름 텍스트 표시
            unitUIItem.GetComponentInChildren<Text>().text = unit.unitName;

            // 유닛 이미지 표시 (유닛에 설정된 이미지)
            unitUIItem.GetComponentInChildren<Image>().sprite = unit.unitImage;
        }
    }
}
