using System.Collections;
using System.Collections.Generic;
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
    private List<Unit> selectedUnits = new List<Unit>();  // ���õ� ���� ����Ʈ

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

        if (Input.GetKeyDown(KeyCode.M) && selectedUnits.Count > 0)
        {
            HandleMoveMode();
        }

        // ���콺 ��ư�� ������ �� ���� ����
        if (Input.GetMouseButtonDown(0))
        {
            isSelecting = true;
            startMousePos = Input.mousePosition;
            StartGrid();  // �׸��� ����
        }

        // ���콺 ��ư�� ���� ���� ����
        if (Input.GetMouseButtonUp(0))
        {
            isSelecting = false;
            SelectUnitsInArea();  // �׸��� ������ ���Ե� ���� ����
            ClearSelectionGrid(); // �׸��� �����
        }

        // ����Ʈ + ��Ŭ�� �� ���� �߰�/����
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {
            ToggleUnitSelectionOnClick();
        }

        // ��Ŭ������ ������ Ŭ���ϸ� �� ���ָ� ����
        if (!Input.GetKey(KeyCode.LeftShift) && Input.GetMouseButtonDown(0))
        {
            SelectSingleUnitOnClick();
        }

        // ���� �ڽ� �׸���
        if (isSelecting)
        {
            UpdateSelectionRect();
        }
    }

    // 'A' Ű�� ������ �� �̵� ��� ó��
    private void HandleAtkMoveMode()
    {
        // ���콺 Ŭ�� ��ġ�� �̵��� ��ǥ ���� ����
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;

            foreach (Unit unit in selectedUnits)
            {
                if (unit != null)
                {
                    unit.isAtk = false; // �̵� ���� ������ ���� ���°� �ƴϹǷ� false ����

                    // �̵� ó��
                    if (UnitManager.instance.monsters.Count == 0) unit.SetTarget(targetPosition);
                    unit.isMove = false;
                    unit.currentState = UnitState.Moving;
                }
            }
        }
    }

    // 'A' Ű�� ������ �� �̵� ��� ó��
    private void HandleMoveMode()
    {
        // ���콺 Ŭ�� ��ġ�� �̵��� ��ǥ ���� ����
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;

            foreach (Unit unit in selectedUnits)
            {
                if (unit != null)
                {
                    unit.isAtk = false; // �̵� ���� ������ ���� ���°� �ƴϹǷ� false ����

                    // �̵� ó��
                    if (UnitManager.instance.monsters.Count == 0) unit.SetTarget(targetPosition);
                    unit.isMove = true;
                    unit.currentState = UnitState.Moving;
                }
            }
        }
    }

    // ���� �ڽ� ������Ʈ
    private void UpdateSelectionRect()
    {
        Vector3 currentMousePos = Input.mousePosition;
        float width = currentMousePos.x - startMousePos.x;
        float height = currentMousePos.y - startMousePos.y;

        selectionRect = new Rect(startMousePos.x, Screen.height - startMousePos.y, width, -height);
        // �׸��� UI ��ġ�� ũ�� ������Ʈ
        selectionGrid.GetComponent<RectTransform>().sizeDelta = new Vector2(width, -height);
        selectionGrid.transform.position = new Vector3(startMousePos.x + width / 2, startMousePos.y - height / 2, 0);
    }

    // �׸��� ����: �׸��� UI ����
    private void StartGrid()
    {
        selectionGrid = Instantiate(gridPrefab, Vector3.zero, Quaternion.identity);
        selectionGrid.transform.SetParent(transform, false);  // SelectionManager�� �ڽ����� ����
    }

    // ���õ� �׸��� ���� ���ֵ� ����
    private void SelectUnitsInArea()
    {
        selectedUnits.Clear();  // ���� ���õ� ������ ����� ���� ����

        foreach (Unit unit in FindObjectsOfType<Unit>())
        {
            // ������ ȭ�� �� ��ġ�� ȭ�� ��ǥ�� ��ȯ�Ͽ� �׸��� ���� ���ԵǴ��� Ȯ��
            Vector3 screenPos = mainCamera.WorldToScreenPoint(unit.transform.position);
            if (selectionRect.Contains(screenPos))
            {
                selectedUnits.Add(unit);
                unit.Select();  // ���õ� ���ֿ� ���� �ð������� ǥ��
            }
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
        if (Physics.Raycast(ray, out hit))
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
        if (Physics.Raycast(ray, out hit))
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
