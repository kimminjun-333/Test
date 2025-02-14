using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public static SelectionManager instance;

    public Camera mainCamera; // ī�޶�
    public GameObject gridPrefab;  // �׸��� UI ������
    private GameObject selectionGrid;  // �׸��� ������Ʈ

    private Vector3 startMousePos;   // �巡�� ���� ��ġ
    private Rect selectionRect;      // ���õ� �簢�� ����
    private bool isSelecting = false; // ���� ����
    public List<Unit> selectedUnits = new List<Unit>();  // ���õ� ���� ����Ʈ
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
        // 'A' Ű�� ������ �� ���ֵ� �߿��� �̵� ó��
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

        // ���콺 ��ư�� ������ �� ���� ����
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

                // �巡�� �Ӱ谪�� ���� ������ ���� ����
                isDragging = false; // �巡�� �ʱ�ȭ

                isSelecting = false;
                SelectSingleUnitOnClick();  // ���� ���� ����
            }
        }

        // ���콺 �̵� ��
        if (Input.GetMouseButton(0))
        {
            // �巡�� �Ӱ谪�� üũ�Ͽ� �巡�� ���� ��ȯ
            if (!isDragging && Vector2.Distance(startMousePos, Input.mousePosition) > dragThreshold)
            {
                isDragging = true;  // �巡�� ����
                isSelecting = true;  // �׸��� ���� ���� ��ȯ
                StartGrid(startMousePos);  // �׸��� ����
            }

            if (isDragging)
            {
                UpdateSelectionRect();  // �巡�� �� �׸��� ������Ʈ
            }
        }

        // ���콺 ��ư�� ���� ���� ����
        if (Input.GetMouseButtonUp(0))
        {
            if (isSelecting)
            {
                // �׸��� ������ ���Ե� ���� ����
                SelectUnitsInArea();
                ClearSelectionGrid();  // �׸��� �����
            }
            isSelecting = false;  // ���� ����
            isDragging = false;  // �巡�� ���� �ʱ�ȭ
        }

        // ��Ŭ������ ������ Ŭ���ϸ� �� ���ָ� ����
        if (!Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {
            SelectSingleUnitOnClick();
        }
    }

    // 'A' Ű�� ������ �� �̵� ��� ó��
    private void HandleAtkMoveMode()
    {
        // ���콺 Ŭ�� ��ġ�� �̵��� ��ǥ ���� ����
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
                    // �̵� ó��
                    if (UnitManager.instance.monsters.Count == 0) unit.unitMove.SetTargetPos(targetPosition);
                    unit.isCanAttack = true;
                    unit.currentState = UnitState.Moving;
                }
            }
        }
    }

    // 'M' Ű�� ������ �� �̵� ��� ó��
    private void HandleMoveMode()
    {
        // ���콺 Ŭ�� ��ġ�� �̵��� ��ǥ ���� ����
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
                    // �̵� ó��
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

    // �׸��� ����: �׸��� UI ����
    private void StartGrid(Vector3 mouseStartPos)
    {
        selectionGrid = Instantiate(gridPrefab, mouseStartPos, Quaternion.identity);
        selectionGrid.transform.SetParent(transform, false);  // SelectionManager�� �ڽ����� ����
    }

    // ���� �ڽ� ������Ʈ
    private void UpdateSelectionRect()
    {
        Vector3 currentMousePos = Input.mousePosition;
        float width = currentMousePos.x - startMousePos.x;
        float height = currentMousePos.y - startMousePos.y;

        // Rect�� ũ��� ��ġ�� �������� ���� ���콺 ��ġ�� �°� ����
        float xMin = Mathf.Min(startMousePos.x, currentMousePos.x);
        float yMin = Mathf.Min(startMousePos.y, currentMousePos.y);
        float xMax = Mathf.Max(startMousePos.x, currentMousePos.x);
        float yMax = Mathf.Max(startMousePos.y, currentMousePos.y);

        selectionRect = new Rect(xMin, yMin, xMax - xMin, yMax - yMin);

        // �׸��� UI ��ġ�� ũ�� ������Ʈ
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
        selectedUnits.Clear();  // ���� ���õ� ������ ����� ���� ����

        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            // ������ ȭ�� �� ��ġ�� ȭ�� ��ǥ�� ��ȯ�Ͽ� �׸��� ���� ���ԵǴ��� Ȯ��
            Vector3 screenPos = mainCamera.WorldToScreenPoint(unit.transform.position);

            // ������ �׸��� ������ �����̶� ��ġ�� ����
            if (selectionRect.Overlaps(new Rect(screenPos.x, screenPos.y, 1, 1))) // 1x1 ũ���� ������ Ȯ��
            {
                // �ִ� 24�������� �����ϵ��� ����
                if (selectedUnits.Count < maxSelectedUnits)
                {
                    selectedUnits.Add(unit);
                    unit.Select();  // ���õ� ���ֿ� ���� �ð������� ǥ��
                }
            }
        }

        if (selectedUnits.Count == 0)
        {
            return;  // ���õ� ������ ������ �޼��� ����
        }

        // UI ������Ʈ
        UIManager.instance.UpdateUnitUI(selectedUnits);
    }

    // ���õ� ���� ��ȯ
    public List<Unit> GetSelectedUnits()
    {
        return selectedUnits;
    }

    // �׸��� �����
    private void ClearSelectionGrid()
    {
        Destroy(selectionGrid);  // �׸��� ����
    }

    // ����Ʈ + ��Ŭ�� �� ���� ���� �߰�/����
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
                    // �̹� ���õ� �����̶�� ��Ͽ��� ����
                    selectedUnits.Remove(clickedUnit);
                    clickedUnit.Deselect(); // ���� ���� ����
                }
                else
                {
                    // ���õ��� ���� �����̶�� ��Ͽ� �߰�
                    selectedUnits.Add(clickedUnit);
                    clickedUnit.Select(); // ���� ����
                }
                // UI ������Ʈ
                UIManager.instance.UpdateUnitUI(selectedUnits);
            }
        }
    }

    // ��Ŭ������ ������ Ŭ���ϸ� �� ���ָ� ����
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
                // ������ ���õ� ���ֵ��� ��� ����
                foreach (Unit unit in selectedUnits)
                {
                    unit.Deselect(); // ���� ����
                }

                // ���ο� ������ ����
                selectedUnits.Clear();
                selectedUnits.Add(clickedUnit);
                clickedUnit.Select(); // ���� ����

                // UI ������Ʈ
                UIManager.instance.UpdateUnitUI(selectedUnits);
            }
        }
    }
}
