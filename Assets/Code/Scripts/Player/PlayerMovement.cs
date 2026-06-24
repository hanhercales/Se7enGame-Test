using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 3f;
    public InputAction moveAction;
    
    public PlayerStateMachine stateMachine;
    private Rigidbody rb;
    private Vector3 direction;
    
    private void OnEnable() =>  moveAction.Enable();
    private void OnDisable() =>  moveAction.Disable();

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        rb = GetComponent<Rigidbody>();
        
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    private void Update()
    {
        Vector2 moveInput = moveAction.ReadValue<Vector2>();
        direction = stateMachine.RunState(moveInput);

        if (direction != Vector3.zero)
        {
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 5f);
        }
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = new Vector3(direction.x * speed, rb.linearVelocity.y, direction.z * speed);
    }
}
