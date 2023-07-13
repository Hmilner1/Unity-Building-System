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

    public delegate void CamChanged(GameObject Cam);
    public static event CamChanged OnCamChanged;

    public delegate void Paused();
    public static event Paused OnPaused;

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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            m_State = CameraState.FirstPerson;
            OnStateChange();
            OnCamChanged?.Invoke(m_FirstPersonCamera);
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            m_State = CameraState.Overhead;
            OnStateChange();
            OnCamChanged?.Invoke(m_OverHeadCamera);
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OnPaused?.Invoke();
            if (m_State != CameraState.Pause)
            {
                m_State = CameraState.Pause;
                OnStateChange();
            }
            else
            {
                Time.timeScale = 1;
                m_State = CameraState.FirstPerson;
                OnStateChange();
                OnCamChanged?.Invoke(m_FirstPersonCamera);
            }
        }
    }

    private void OnStateChange()
    {
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
