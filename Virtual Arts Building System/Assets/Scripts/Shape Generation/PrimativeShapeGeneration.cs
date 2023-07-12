using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PrimativeShapeGeneration : MonoBehaviour
{
    enum BuildState
    { 
        Preview,
        Placed
    }
    private BuildState m_CurrentState = BuildState.Placed;
    [SerializeField]
    private float desiredHeight = 0.5f;

    private float m_Distance;
    private GameObject m_CurrentCamera;
    private GameObject m_HoldObject;

    private float m_TerrainHeight = 0;

    private void OnEnable()
    {
        ObjectManager.OnSpawnPreviewOject += SpawnHoldObject;
        ObjectManager.OnPlaceObject += PlaceObject;
        CollisionDetection.OnMoveObject += IncreaseHight;
    }

    private void OnDisable()
    {
        ObjectManager.OnSpawnPreviewOject -= SpawnHoldObject;
        ObjectManager.OnPlaceObject -= PlaceObject;
        CollisionDetection.OnMoveObject -= IncreaseHight;
    }

    private void Update()
    {
        OnStateChange();
    }

    private void OnStateChange()
    {
        switch (m_CurrentState)
        {
            case BuildState.Preview:
                PreviewEffect();
                MovePreview();
                break;
            case BuildState.Placed:

                break;
            default:
                break;
        }
    }

    private void SpawnHoldObject(float distance, GameObject cam, PrimitiveType type)
    {
        if (m_CurrentState != BuildState.Preview)
        {
            m_CurrentState = BuildState.Preview;
            m_Distance = distance;
            m_CurrentCamera = cam;

            m_HoldObject = GameObject.CreatePrimitive(type);
            m_HoldObject.AddComponent<CollisionDetection>();
            m_HoldObject.transform.position = calcSpawnGrid();
            m_HoldObject.transform.SetParent(cam.transform);
        }
    }

    private void PlaceObject()
    {
        if (m_CurrentState == BuildState.Preview)
        {
            m_CurrentState = BuildState.Placed;
            PlaceColour();
            m_HoldObject.GetComponent<CollisionDetection>().enabled = false;

            m_HoldObject.transform.SetParent(null, true);
            m_HoldObject = null;
        }
    }

    private void MovePreview()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            m_HoldObject.transform.position = calcSpawnFree();
            
        }
        else
        {
            m_HoldObject.transform.position = calcSpawnGrid();
        }
      
        float lockPos = 0;
        m_HoldObject.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, lockPos, lockPos);
    }

    private void PreviewEffect()
    { 
        Material matertial = m_HoldObject.GetComponent<Renderer>().material;
        matertial.color = new Color(0, 1, 0, 0);
    
    }

    private void PlaceColour()
    {
        Material matertial = m_HoldObject.GetComponent<Renderer>().material;
        matertial.color = new Color(0.5f, 0.5f, 0.5f, 1);
    }

    private Vector3 calcSpawnFree()
    {
        Terrain terrain = FindObjectOfType<Terrain>();
        Vector3 camDirection = m_CurrentCamera.transform.forward;

        float spawnX = m_CurrentCamera.transform.position.x + camDirection.x * m_Distance;
        float spawnZ = m_CurrentCamera.transform.position.z + camDirection.z * m_Distance;
        float spawnY = terrain.SampleHeight(new Vector3(spawnX, m_TerrainHeight, spawnZ)) + desiredHeight;

        Vector3 camPos = new Vector3(spawnX, spawnY, spawnZ);
        Vector3 spawnPos = camPos;

        return spawnPos;
    }

    private Vector3 calcSpawnGrid()
    {
        Terrain terrain = FindObjectOfType<Terrain>();
        Vector3 camDirection = m_CurrentCamera.transform.forward;

        float spawnX = m_CurrentCamera.transform.position.x + camDirection.x * m_Distance;
        float spawnZ = m_CurrentCamera.transform.position.z + camDirection.z * m_Distance;
        float spawnY = terrain.SampleHeight(new Vector3(spawnX, m_TerrainHeight, spawnZ)) + desiredHeight;

        Vector3 camPos = new Vector3(spawnX, spawnY, spawnZ);

        Grid grid = terrain.GetComponent<Grid>();
        Vector3Int gridPosition = grid.WorldToCell(camPos);
        return grid.CellToWorld(gridPosition);
    }

    private void IncreaseHight()
    {
        m_TerrainHeight = m_TerrainHeight + 100;
    }
}
