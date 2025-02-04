using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance;

    public Camera mainCamera; // 카메라
    public GameObject gridPrefab;  // 그리드 UI 프리팹
    private GameObject selectionGrid;  // 그리드 오브젝트

    private Vector3 startMousePos;   // 드래그 시작 위치
    private Rect selectionRect;      // 선택된 사각형 영역
    private bool isSelecting = false; // 선택 상태
    private List<Unit> selectedUnits = new List<Unit>();  // 선택된 유닛 리스트

    private void Awake()
    {
        instance = this;
        mainCamera = Camera.main;
    }

    private void Update()
    {
        // 'A' 키를 눌렀을 때 유닛들 중에서 이동 처리
        if (Input.GetKeyDown(KeyCode.A) && selectedUnits.Count > 0)
        {
            HandleAtkMoveMode();
        }

        if (Input.GetKeyDown(KeyCode.M) && selectedUnits.Count > 0)
        {
            HandleMoveMode();
        }

        // 마우스 버튼을 눌렀을 때 선택 시작
        if (Input.GetMouseButtonDown(0))
        {
            isSelecting = true;
            startMousePos = Input.mousePosition;
            StartGrid();  // 그리드 시작
        }

        // 마우스 버튼을 떼면 선택 종료
        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false;
            SelectUnitsInArea();  // 그리드 영역에 포함된 유닛 선택
            ClearSelectionGrid(); // 그리드 지우기
        }

        // 쉬프트 + 좌클릭 시 유닛 추가/제거
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {
            ToggleUnitSelectionOnClick();
        }

        // 좌클릭으로 유닛을 클릭하면 그 유닛만 선택
        if (!Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {
            SelectSingleUnitOnClick();
        }

        // 선택 박스 그리기
        if (isSelecting)
        {
            UpdateSelectionRect();
        }
    }

    // 'A' 키를 눌렀을 때 이동 모드 처리
    private void HandleAtkMoveMode()
    {
        // 마우스 클릭 위치로 이동할 목표 지점 설정
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;

            foreach (Unit unit in selectedUnits)
            {
                if (unit != null)
                {
                    unit.isAtk = false; // 이동 중인 유닛은 공격 상태가 아니므로 false 설정

                    // 이동 처리
                    if (UnitManager.instance.monsters.Count == 0) unit.SetTarget(targetPosition);
                    unit.isMove = false;
                    unit.currentState = UnitState.Moving;
                }
            }
        }
    }

    // 'A' 키를 눌렀을 때 이동 모드 처리
    private void HandleMoveMode()
    {
        // 마우스 클릭 위치로 이동할 목표 지점 설정
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;

            foreach (Unit unit in selectedUnits)
            {
                if (unit != null)
                {
                    unit.isAtk = false; // 이동 중인 유닛은 공격 상태가 아니므로 false 설정

                    // 이동 처리
                    if (UnitManager.instance.monsters.Count == 0) unit.SetTarget(targetPosition);
                    unit.isMove = true;
                    unit.currentState = UnitState.Moving;
                }
            }
        }
    }

    // 선택 박스 업데이트
    private void UpdateSelectionRect()
    {
        Vector3 currentMousePos = Input.mousePosition;
        float width = currentMousePos.x - startMousePos.x;
        float height = currentMousePos.y - startMousePos.y;

        selectionRect = new Rect(startMousePos.x, Screen.height - startMousePos.y, width, -height);
        // 그리드 UI 위치와 크기 업데이트
        selectionGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(width, -height);
        selectionGrid.transform.position = new Vector3(startMousePos.x + width / 2, startMousePos.y - height / 2, 0);
    }

    // 그리드 시작: 그리드 UI 생성
    private void StartGrid()
    {
        selectionGrid = Instantiate(gridPrefab, Vector3.zero, Quaternion.identity);
        selectionGrid.transform.SetParent(transform, false);  // SelectionManager의 자식으로 설정
    }

    // 선택된 그리드 안의 유닛들 선택
    private void SelectUnitsInArea()
    {
        selectedUnits.Clear();  // 기존 선택된 유닛을 지우고 새로 선택

        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            // 유닛의 화면 상 위치를 화면 좌표로 변환하여 그리드 내에 포함되는지 확인
            Vector3 screenPos = mainCamera.WorldToScreenPoint(unit.transform.position);
            if (selectionRect.Contains(screenPos))
            {
                selectedUnits.Add(unit);
                unit.Select();  // 선택된 유닛에 대해 시각적으로 표시
            }
        }

        // UI 업데이트
        UIManager.instance.UpdateUnitUI(selectedUnits);
    }

    // 선택된 유닛 반환
    public List<Unit> GetSelectedUnits()
    {
        return selectedUnits;
    }

    // 그리드 지우기
    private void ClearSelectionGrid()
    {
        Destroy(selectionGrid);  // 그리드 삭제
    }

    // 쉬프트 + 좌클릭 시 유닛 선택 추가/제거
    private void ToggleUnitSelectionOnClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Unit clickedUnit = hit.collider.GetComponent<Unit>();
            if (clickedUnit != null)
            {
                if (selectedUnits.Contains(clickedUnit))
                {
                    // 이미 선택된 유닛이라면 목록에서 제거
                    selectedUnits.Remove(clickedUnit);
                    clickedUnit.Deselect(); // 유닛 선택 해제
                }
                else
                {
                    // 선택되지 않은 유닛이라면 목록에 추가
                    selectedUnits.Add(clickedUnit);
                    clickedUnit.Select(); // 유닛 선택
                }
                // UI 업데이트
                UIManager.instance.UpdateUnitUI(selectedUnits);
            }
        }
    }

    // 좌클릭으로 유닛을 클릭하면 그 유닛만 선택
    private void SelectSingleUnitOnClick()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Unit clickedUnit = hit.collider.GetComponent<Unit>();
            if (clickedUnit != null)
            {
                // 기존에 선택된 유닛들을 모두 제거
                foreach (Unit unit in selectedUnits)
                {
                    unit.Deselect(); // 선택 해제
                }

                // 새로운 유닛을 선택
                selectedUnits.Clear();
                selectedUnits.Add(clickedUnit);
                clickedUnit.Select(); // 유닛 선택

                // UI 업데이트
                UIManager.instance.UpdateUnitUI(selectedUnits);
            }
        }
    }
}
