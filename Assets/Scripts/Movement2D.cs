using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement2D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private Vector3 moveDirection = Vector3.zero;

    public Vector3 MoveDirection { set => moveDirection = value; }

    private void Update()
    {
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }
}