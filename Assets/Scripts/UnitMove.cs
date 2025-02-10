using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMove : MonoBehaviour
{
    public Unit unit;
    private float moveSpeed;
    private Vector3 targetPos;

    private void Awake()
    {
        unit = GetComponent<Unit>();
        moveSpeed = unit.moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetTargetPos(Vector3 pos)
    {
        targetPos = pos;
    }

    public void Move()
    {
        Vector3 targetPosition = new Vector3(this.targetPos.x, transform.position.y, this.targetPos.z);
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            transform.position = targetPosition;
            unit.currentState = UnitState.Attacking;
        }
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0;
        if (direction.magnitude > 0.1f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
