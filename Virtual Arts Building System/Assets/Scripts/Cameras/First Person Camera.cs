using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField]
    private float Speed;
    [SerializeField]
    private float mouseSensitivity;

    private Transform m_FirstPersonCamera;
    private CharacterController m_Controller;
    private float m_CameraCap;
    private Vector2 m_CurrentMouse;
    private Vector2 m_CurrentMouseVelocity;
    private Vector2 m_CurrentDirection;
    private Vector2 m_CurrentVelocity;

    void Awake()
    {
        m_Controller = GetComponent<CharacterController>();
        m_FirstPersonCamera = GameObject.Find("First Person Camera").GetComponent<Transform>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    void Update()
    {
        MouseMovement();
        KeybaordMovement();
        IsGrounded();
    }

    private bool IsGrounded()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 1.5f))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            return true;
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 1.5f, Color.white);
            return false;
        }
    }

    void MouseMovement()
    {
        Vector2 mouseTarget = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        m_CurrentMouse = Vector2.SmoothDamp(m_CurrentMouse, mouseTarget, ref m_CurrentMouseVelocity, 0.1f);

        m_CameraCap -= m_CurrentMouse.y * mouseSensitivity;
        m_CameraCap = Mathf.Clamp(m_CameraCap, -90.0f, 90.0f);

        m_FirstPersonCamera.localEulerAngles = Vector3.right * m_CameraCap;
        transform.Rotate(Vector3.up * m_CurrentMouse.x * mouseSensitivity);
    }

    void KeybaordMovement()
    {
        float velocityY =0;
        if (IsGrounded())
        {
            velocityY += 30f * 2f * Time.deltaTime;
        }
        else
        {
            velocityY = -8f;
        }

        Vector2 targetDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDirection.Normalize();

        m_CurrentDirection = Vector2.SmoothDamp(m_CurrentDirection, targetDirection, ref m_CurrentVelocity, 0.3f);

        Vector3 velocity = (transform.forward * m_CurrentDirection.y + transform.right * m_CurrentDirection.x) * Speed + Vector3.up * velocityY;
        m_Controller.Move(velocity * Time.deltaTime);
    }
}