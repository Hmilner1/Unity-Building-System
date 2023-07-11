using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using Unity.VisualScripting;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    enum CameraState
    { 
        FirstPerson,
        Overhead,
        Pause
    }

    private GameObject m_FirstPersonCamera;
    private GameObject m_OverHeadCamera;
    private CameraState m_State;

    private void Awake()
    {
        m_FirstPersonCamera = GameObject.Find("FPS Cam Holder");
        m_OverHeadCamera = GameObject.Find("OH Cam Holder");

        m_State = CameraState.FirstPerson;
        OnStateChange();
    }

    private void Update()
    {
        UserInput();
    }

    private void UserInput()
    {
        if (Input.GetKeyDown("1"))
        {
            m_State = CameraState.FirstPerson;
            OnStateChange();
        }

        if (Input.GetKeyDown("2"))
        {
            m_State = CameraState.Overhead;
            OnStateChange();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            m_State = CameraState.Pause;
            OnStateChange();
        }
    }

    private void OnStateChange()
    {
        if (m_State == CameraState.FirstPerson)
        {
            m_OverHeadCamera.gameObject.SetActive(false);
            m_FirstPersonCamera.gameObject.SetActive(true);
        }

        switch (m_State)
        {
            case CameraState.FirstPerson:
                m_OverHeadCamera.gameObject.SetActive(false);
                m_FirstPersonCamera.gameObject.SetActive(true);
                break;

            case CameraState.Overhead:
                m_OverHeadCamera.gameObject.SetActive(true);
                m_FirstPersonCamera.gameObject.SetActive(false);
                break;

            case CameraState.Pause:
                GamePause();
                break;

            default:

                break;
        }
    }

    private void GamePause()
    {
        Time.timeScale = 0;
    }
}
