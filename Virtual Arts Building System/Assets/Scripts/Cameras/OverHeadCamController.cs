using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OverHeadCamController : MonoBehaviour
{
    [SerializeField]
    private float Speed;

    private Camera m_camera;
    private CharacterController m_Controller;
    private Vector2 m_CurrentDirection;
    private Vector2 m_CurrentVelocity;


    private void Awake()
    {
        m_Controller = GetComponent<CharacterController>();
        m_camera = GetComponentInChildren<Camera>();
    }

    private void Update()
    {
        KeybaordMovement();
        ChangeFOV();
    }

    void KeybaordMovement()
    {
        float velocityY = 0;

        Vector2 targetDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        targetDirection.Normalize();

        m_CurrentDirection = Vector2.SmoothDamp(m_CurrentDirection, targetDirection, ref m_CurrentVelocity, 0.3f);

        Vector3 velocity = (transform.forward * m_CurrentDirection.y + transform.right * m_CurrentDirection.x) * Speed + Vector3.up * velocityY;
        m_Controller.Move(velocity * Time.deltaTime);
    }

    void ChangeFOV()
    {
        float fov;

        fov = m_camera.fieldOfView;
        fov += (Input.GetAxis("Mouse ScrollWheel") * 10);
        fov = Mathf.Clamp(fov, 30f, 120f);

        m_camera.fieldOfView = fov;
    }
}
