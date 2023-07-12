using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private Canvas m_MainCanvas;
    [SerializeField]
    private Canvas m_EditCanvas;
    [SerializeField]
    private Canvas m_PauseCanvas;
    [SerializeField]
    private Canvas m_PlacementCanvas;

    private bool m_IsPaused;

    private void OnEnable()
    {
        CameraManager.OnPaused += OnPause;
        PrimativeShapeGeneration.OnEditMode += OnEdit;
        PrimativeShapeGeneration.OnExitEditMode += OnExitEdit;
    }

    private void OnDisable()
    {
        CameraManager.OnPaused -= OnPause;
        PrimativeShapeGeneration.OnEditMode -= OnEdit;
        PrimativeShapeGeneration.OnExitEditMode -= OnExitEdit;
    }

    private void Start()
    {
        m_EditCanvas.gameObject.SetActive(false);
        m_PauseCanvas.gameObject.SetActive(false);

        m_IsPaused = false;
    }

    private void Update()
    {
        OnPlace();
    }

    private void OnPause()
    {
        if (!m_IsPaused)
        {
            m_PauseCanvas.gameObject.SetActive(true);
            m_IsPaused = true;
        }
        else if (m_IsPaused)
        {
            m_PauseCanvas.gameObject.SetActive(false);
            m_IsPaused = false;
        }
    }

    private void OnEdit()
    {
        m_EditCanvas.gameObject.SetActive(true);
        m_PlacementCanvas.gameObject.SetActive(false);
    }

    private void OnExitEdit()
    {
        m_EditCanvas.gameObject.SetActive(false);
    }

    private void OnPlace()
    {
        if (!m_EditCanvas.isActiveAndEnabled && !m_PauseCanvas.isActiveAndEnabled)
        {
            m_PlacementCanvas.gameObject.SetActive(true);
        }
        else
        {
            m_PlacementCanvas.gameObject.SetActive(false);
        }
    }
}
