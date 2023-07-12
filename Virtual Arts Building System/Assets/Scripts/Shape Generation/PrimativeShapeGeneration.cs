using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class PrimativeShapeGeneration : MonoBehaviour
{
    [SerializeField]
    private float desiredHeight = 0.5f;

    private float m_Distance;
    private GameObject m_CurrentCamera;
    private GameObject m_HoldObject;

    private void OnEnable()
    {
        ObjectManager.OnSpawnPreviewOject += SpawnHoldObject;
        ObjectManager.OnPlaceObject += PlaceObject;
    }

    private void OnDisable()
    {
        ObjectManager.OnSpawnPreviewOject -= SpawnHoldObject;
        ObjectManager.OnPlaceObject -= PlaceObject;
    }

    private void Update()
    {
        if (m_HoldObject != null)
        {
            m_HoldObject.transform.position = calcSpawn();
        }
    }

    private void SpawnHoldObject(float distance, GameObject cam)
    {
        m_Distance = distance;
        m_CurrentCamera = cam;

        m_HoldObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        m_HoldObject.transform.position = calcSpawn();
        m_HoldObject.transform.SetParent(cam.transform);
    }

    private void PlaceObject()
    {
        m_HoldObject.transform.SetParent(null, true);
    }

    private Vector3 calcSpawn()
    {
        Terrain terrain = FindObjectOfType<Terrain>();
        Vector3 camDirection = m_CurrentCamera.transform.forward;

        float spawnX = m_CurrentCamera.transform.position.x + camDirection.x * m_Distance;
        float spawnZ = m_CurrentCamera.transform.position.z + camDirection.z * m_Distance;
        float spawnY = terrain.SampleHeight(new Vector3(spawnX, 0, spawnZ)) + desiredHeight;

        Vector3 camPos = new Vector3(spawnX, spawnY, spawnZ);
        Vector3 spawnPos = camPos;

        return spawnPos;
    }
}
