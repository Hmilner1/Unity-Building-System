using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    [SerializeField]
    private float m_SetDistance;

    private float m_ActualDistance;


    private GameObject m_currentActiveCam;

    public delegate void SpawnPreviewObject(float distance, GameObject currentCam);
    public static event SpawnPreviewObject OnSpawnPreviewOject;

    public delegate void PlaceObject();
    public static event PlaceObject OnPlaceObject;

    private void OnEnable()
    {
        CameraManager.OnCamChanged += UpdateCurrentCam;
    }

    private void OnDisable()
    {
        CameraManager.OnCamChanged -= UpdateCurrentCam;
    }

    private void Awake()
    {
        if (m_currentActiveCam == null)
        {
            m_currentActiveCam = GameObject.Find("FPS Cam Holder");
            m_ActualDistance = m_SetDistance;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            OnSpawnPreviewOject?.Invoke(DistanceChanger(), m_currentActiveCam);
        }

        //if (Input.GetKeyDown("Fire 1"))
        //{
        //    OnPlaceObject?.Invoke();
        //}
    }

    private void UpdateCurrentCam(GameObject Cam)
    { 
        m_currentActiveCam = Cam;
        DistanceChanger();
    }

    private float DistanceChanger()
    {
        if (m_currentActiveCam == GameObject.Find("FPS Cam Holder"))
        {
            m_ActualDistance = m_SetDistance;
        }
        else 
        {
            m_ActualDistance = 0;
        }

        return m_ActualDistance;
    }
}
