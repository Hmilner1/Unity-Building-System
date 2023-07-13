using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.Rendering.LookDev;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Experimental.XR.Interaction;
using UnityEngine.UI;
using static CollisionDetection;

public class PrimativeShapeGeneration : MonoBehaviour
{
    enum BuildState
    { 
        Preview,
        Editing,
        Placed
    }
    private BuildState m_CurrentState = BuildState.Placed;
    [SerializeField]
    private float desiredHeight = 0.5f;
    [SerializeField]
    private Material OutineMat;
    [SerializeField]
    private Material Base;

    private float m_Distance;
    private GameObject m_CurrentCamera;
    private GameObject m_HoldObject;

    private float m_TerrainHeight = 0;
    private float m_RayHeight;
    private Material[] m_MaterialsPlace;
    private Material[] m_MaterialsEdit;
    private GameObject Highlight;
    private bool m_isHolding;
    private int m_MatNum;

    public delegate void Edit();
    public static event Edit OnEditMode;

    public delegate void ExitEdit();
    public static event ExitEdit OnExitEditMode;

    private void OnEnable()
    {
        ObjectManager.OnSpawnPreviewOject += SpawnHoldObject;
        ObjectManager.OnPlaceObject += PlaceObject;
        OnMoveObjectUp += IncreaseHight;
        OnMoveObjectDown += DecreaseHight;
    }

    private void OnDisable()
    {
        ObjectManager.OnSpawnPreviewOject -= SpawnHoldObject;
        ObjectManager.OnPlaceObject -= PlaceObject;
        OnMoveObjectUp -= IncreaseHight;
        OnMoveObjectDown -= DecreaseHight;
    }

    private void Awake()
    {
        if (m_CurrentCamera == null)
        {
            m_CurrentCamera = GameObject.Find("FPS Cam Holder");
        }

        m_MaterialsPlace = new Material[] { Base, Base };
        m_MaterialsEdit = new Material[] { Base, OutineMat };
        m_isHolding = false;
        m_MatNum = 0;
    }

    private void Update()
    {
        OnStateChange();
        ObjectSelection();
    }

    private void OnStateChange()
    {
        switch (m_CurrentState)
        {
            case BuildState.Preview:
                PreviewEffect();
                MovePreview(m_HoldObject);
                break;
            case BuildState.Editing:
                EditingMode();
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
            if (m_CurrentState == BuildState.Editing)
            {
                return;
            }
            else
            {
                m_CurrentState = BuildState.Preview;
                m_Distance = distance;
                m_CurrentCamera = cam;

                m_HoldObject = GameObject.CreatePrimitive(type);

                if (type != PrimitiveType.Cube)
                {
                    Destroy(m_HoldObject.GetComponent<Collider>());
                }
                else if (type == PrimitiveType.Cube)
                {
                    m_HoldObject.GetComponent<BoxCollider>().size = new Vector3(0.9f, 0.9f, 0.9f);
                }

                m_HoldObject.AddComponent<CollisionDetection>();
                m_HoldObject.transform.position = calcSpawnGrid();
                m_HoldObject.transform.SetParent(cam.transform);
            }
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

    private void MovePreview(GameObject moveingObject)
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveingObject.transform.position = calcSpawnFree();
            
        }
        else
        {
            moveingObject.transform.position = calcSpawnGrid();
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
        m_HoldObject.GetComponent<Renderer>().materials = m_MaterialsPlace;
    }

    private Vector3 calcSpawnFree()
    {
        Terrain terrain = FindObjectOfType<Terrain>();
        Vector3 camDirection = m_CurrentCamera.transform.forward;

        float spawnX = m_CurrentCamera.transform.position.x + camDirection.x * m_Distance;
        float spawnZ = m_CurrentCamera.transform.position.z + camDirection.z * m_Distance;
        float spawnY = terrain.SampleHeight(new Vector3(spawnX, 0, spawnZ)) + desiredHeight + m_TerrainHeight;

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
        float spawnY = terrain.SampleHeight(new Vector3(spawnX, 0, spawnZ)) + desiredHeight + m_TerrainHeight;

        Vector3 camPos = new Vector3(spawnX, spawnY, spawnZ);

        Grid grid = terrain.GetComponent<Grid>();
        Vector3Int gridPosition = grid.WorldToCell(camPos);
        return grid.CellToWorld(gridPosition);
    }

    private void IncreaseHight()
    {
        m_TerrainHeight = m_RayHeight + 0.5f;
    }
    private void DecreaseHight()
    {
        m_TerrainHeight = 0;
    }

    private void ObjectSelection()
    {
        Camera cam = m_CurrentCamera.GetComponentInChildren<Camera>();
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, 100f))
        {
            Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            if (hit.collider.gameObject.tag == "PrimShape")
            {
                m_RayHeight = hit.collider.transform.position.y;
                if (m_CurrentState == BuildState.Placed)
                {
                    Highlight = hit.transform.gameObject;
                    HighlightedObjects(Highlight);
                    if (Input.GetButtonDown("Fire2"))
                    { 
                        m_CurrentState = BuildState.Editing;
                        OnEditMode?.Invoke();
                    }
                }
            }
            else
            {
                if (Highlight != null)
                {
                    if (hit.collider.gameObject != Highlight)
                    {
                        ResetMaterial(Highlight);
                    }
                }
            }
        } 
    }

    private void HighlightedObjects(GameObject highlightedObject)
    {
        highlightedObject.GetComponent<Renderer>().materials = m_MaterialsEdit;
    }

    private void ResetMaterial(GameObject Object)
    {
        Object.GetComponent<Renderer>().materials = m_MaterialsPlace;
    }

    private GameObject ObjectToEdit()
    {

        Camera cam = m_CurrentCamera.GetComponentInChildren<Camera>();
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.TransformDirection(Vector3.forward), out hit, 100f))
        {
            Debug.DrawRay(cam.transform.position, cam.transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            if (hit.collider.gameObject.tag == "PrimShape")
            {
                return hit.collider.gameObject;
            }
        }
        else
        {
            return null;
        }
        return null;
    }

    private void EditingMode()
    {
        if (ObjectToEdit() != null)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                if (ObjectToEdit() != null)
                {
                    Destroy(ObjectToEdit());
                    m_CurrentState = BuildState.Placed;
                    OnExitEditMode?.Invoke();
                }
            }

            if (Input.GetButtonDown("Fire2") && !m_isHolding)
            {
                ObjectToEdit().transform.SetParent(m_CurrentCamera.transform);
                m_isHolding = true;
            }
            else if (Input.GetButtonDown("Fire2") && m_isHolding)
            {
                ObjectToEdit().transform.SetParent(null, true);
                m_isHolding = false;
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ObjectToEdit().transform.Rotate(new Vector3(0, 1, 0), 45.0f, Space.World);
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                ObjectToEdit().transform.Rotate(new Vector3(0, 0, 1), 45.0f, Space.World);
            }

            if (Input.GetKey(KeyCode.Y))
            {
                float scale = 1;

                scale += (Input.GetAxis("Mouse ScrollWheel"));
                scale = Mathf.Clamp(scale, -10, 10);

                ObjectToEdit().transform.localScale = new Vector3(ObjectToEdit().transform.localScale.x * scale, ObjectToEdit().transform.localScale.y * scale, ObjectToEdit().transform.localScale.z * scale);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Color[] colours = { Color.white, Color.red, Color.gray, Color.black, Color.blue, Color.yellow };
                if (m_MatNum <= colours.Length - 2 )
                {
                    m_MatNum++;
                }
                else
                {
                    m_MatNum = 0;
                }
                Base.color = colours[m_MatNum];
            }
        }
        else if (Input.GetButtonDown("Fire1"))
        {
            m_CurrentState = BuildState.Placed;
            OnExitEditMode?.Invoke();
        }
    }
}
