using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public delegate void MoveObjectUp();
    public static event MoveObjectUp OnMoveObjectUp;

    public delegate void MoveObjectDown();
    public static event MoveObjectDown OnMoveObjectDown;

    private void Awake()
    {
        gameObject.tag = "PrimShape";
        gameObject.AddComponent<BoxCollider>();
        gameObject.AddComponent<Rigidbody>();
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        gameObject.GetComponent<BoxCollider>().isTrigger = true;
        gameObject.GetComponent<BoxCollider>().size = new Vector3(0.9f, 0.9f, 0.9f);
    }

    private void OnTriggerEnter(Collider other)
    {
        //move object if there is a collision
        if (other.gameObject.tag != "Ground")
        {
            OnMoveObjectUp?.Invoke();
        }
    }

    private void Update()
    {
        //Moves object to the floor if there is no other objects below them 
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 100f))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            if (hit.collider.tag == "Ground")
            {
                OnMoveObjectDown?.Invoke();
            }
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 100f, Color.white);
        }
    }
}
