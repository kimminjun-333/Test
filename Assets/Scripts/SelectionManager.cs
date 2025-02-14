using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public List<Unit> selectedUnits = new List<Unit>();  // 선택된 유닛 리스트
    private int maxSelectedUnits = 24;

    private float dragThreshold = 5f;
    private bool isDragging = false;

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

        if (Input.GetKeyDown(KeyCode.M) && selectedUnits.Count > 0 || Input.GetMouseButtonDown(1) && selectedUnits.Count > 0)
        {
            HandleMoveMode();
        }

        if (Input.GetKeyDown(KeyCode.H) && selectedUnits.Count > 0)
        {
            HandleHold();
        }

        // 마우스 버튼을 눌렀을 때 선택 시작
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                startMousePos = Input.mousePosition;
                ToggleUnitSelectionOnClick();
            }
            else
            {
                startMousePos = Input.mousePosition;

                // 드래그 임계값을 넘지 않으면 단일 선택
                isDragging = false; // 드래그 초기화

                isSelecting = false;
                SelectSingleUnitOnClick();  // 단일 유닛 선택
            }
        }

        // 마우스 이동 중
        if (Input.GetMouseButton(0))
        {
            // 드래그 임계값을 체크하여 드래그 모드로 전환
            if (!isDragging && Vector2.Distance(startMousePos, Input.mousePosition) > dragThreshold)
            {
                isDragging = true;  // 드래그 시작
                isSelecting = true;  // 그리드 선택 모드로 전환
                StartGrid(startMousePos);  // 그리드 시작
            }

            if (isDragging)
            {
                UpdateSelectionRect();  // 드래그 중 그리드 업데이트
            }
        }

        // 마우스 버튼을 떼면 선택 종료
        if (Input.GetMouseButtonUp(0))
        {
            if (isSelecting)
            {
                // 그리드 영역에 포함된 유닛 선택
                SelectUnitsInArea();
                ClearSelectionGrid();  // 그리드 지우기
            }
            isSelecting = false;  // 선택 종료
            isDragging = false;  // 드래그 상태 초기화
        }

        // 좌클릭으로 유닛을 클릭하면 그 유닛만 선택
        if (!Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {
            SelectSingleUnitOnClick();
        }
    }

    // 'A' 키를 눌렀을 때 이동 모드 처리
    private void HandleAtkMoveMode()
    {
        // 마우스 클릭 위치로 이동할 목표 지점 설정
        int groundLayerMask = LayerMask.GetMask("Ground");
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            Vector3 targetPosition = hit.point;

            foreach (Unit unit in selectedUnits)
            {
                if (unit != null)
                {
                    // 이동 처리
                    if (UnitManager.instance.monsters.Count == 0) unit.unitMove.SetTargetPos(targetPosition);
                    unit.isCanAttack = true;
                    unit.currentState = UnitState.Moving;
                }
            }
        }
    }

    // 'M' 키를 눌렀을 때 이동 모드 처리
    private void HandleMoveMode()
    {
        // 마우스 클릭 위치로 이동할 목표 지점 설정
        int groundLayerMask = LayerMask.GetMask("Ground");
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayerMask))
        {
            Vector3 targetPosition = hit.point;

            foreach (Unit unit in selectedUnits)
            {
                if (unit != null)
                {
                    // 이동 처리
                    unit.unitMove.SetTargetPos(targetPosition);
                    unit.isCanAttack = false;
                    unit.currentState = UnitState.Moving;
                }
            }
        }
    }

    private void HandleHold()
    {
        foreach (Unit unit in selectedUnits)
        {
            if (unit != null)
            {
                unit.currentState = UnitState.Holding;
            }
        }
    }

    // 그리드 시작: 그리드 UI 생성
    private void StartGrid(Vector3 mouseStartPos)
    {
        selectionGrid = Instantiate(gridPrefab, mouseStartPos, Quaternion.identity);
        selectionGrid.transform.SetParent(transform, false);  // SelectionManager의 자식으로 설정
    }

    // 선택 박스 업데이트
    private void UpdateSelectionRect()
    {
        Vector3 currentMousePos = Input.mousePosition;
        float width = currentMousePos.x - startMousePos.x;
        float height = currentMousePos.y - startMousePos.y;

        // Rect의 크기와 위치를 시작점과 현재 마우스 위치에 맞게 설정
        float xMin = Mathf.Min(startMousePos.x, currentMousePos.x);
        float yMin = Mathf.Min(startMousePos.y, currentMousePos.y);
        float xMax = Mathf.Max(startMousePos.x, currentMousePos.x);
        float yMax = Mathf.Max(startMousePos.y, currentMousePos.y);

        selectionRect = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);

        // 그리드 UI 위치와 크기 업데이트
        selectionGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(xMax - xMin, yMax - yMin);
        selectionGrid.transform.position = new Vector3((xMin + xMax) / 2, (yMin + yMax) / 2, 0);
    }

    private void SelectUnitsInArea()
    {
        if (selectedUnits.Count != 0)
        {
            foreach (Unit selectedUnit in selectedUnits)
            {
                selectedUnit.Deselect();
            }
        }
        selectedUnits.Clear();  // 기존 선택된 유닛을 지우고 새로 선택

        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            // 유닛의 화면 상 위치를 화면 좌표로 변환하여 그리드 내에 포함되는지 확인
            Vector3 screenPos = mainCamera.WorldToScreenPoint(unit.transform.position);

            // 유닛이 그리드 영역에 조금이라도 걸치면 선택
            if (selectionRect.Overlaps(new Rect(screenPos.x, screenPos.y, 1, 1))) // 1x1 크기의 점으로 확인
            {
                // 최대 24개까지만 선택하도록 제한
                if (selectedUnits.Count < maxSelectedUnits)
                {
                    selectedUnits.Add(unit);
                    unit.Select();  // 선택된 유닛에 대해 시각적으로 표시
                }
            }
        }

        if (selectedUnits.Count == 0)
        {
            return;  // 선택된 유닛이 없으면 메서드 종료
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

        int layerMask = LayerMask.GetMask("Unit");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
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

        int layerMask = LayerMask.GetMask("Unit");

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
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
