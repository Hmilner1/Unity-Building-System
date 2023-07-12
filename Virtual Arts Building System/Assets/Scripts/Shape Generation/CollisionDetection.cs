using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetection : MonoBehaviour
{
    public delegate void MoveObject();
    public static event MoveObject OnMoveObject;

    private void Update()
    {
        //    ForwardCast();
        //    BackwardsCast();
        //    LeftCast();
        //    RightCast();
        UpCast();   
        DownCast();
    }

    //private void ForwardCast()
    //{
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 0.5f))
    //    {
    //        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
            
    //    }
    //    else
    //    {
    //        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * 0.5f, Color.white);
            
    //    }
    //}

    //private void BackwardsCast()
    //{
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out hit, 0.5f))
    //    {
    //        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.back) * hit.distance, Color.yellow);
            
    //    }
    //    else
    //    {
    //        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.back) * 0.5f, Color.white);
            
    //    }
    //}

    //private void LeftCast()
    //{
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 0.5f))
    //    {
    //        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * hit.distance, Color.yellow);

    //    }
    //    else
    //    {
    //        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left) * 0.5f, Color.white);

    //    }
    //}

    //private void RightCast()
    //{
    //    RaycastHit hit;
    //    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 0.5f))
    //    {
    //        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * hit.distance, Color.yellow);

    //    }
    //    else
    //    {
    //        Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right) * 0.5f, Color.white);

    //    }
    //}

    private void UpCast()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.up), out hit, 0.4f))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * hit.distance, Color.yellow);
            OnMoveObject?.Invoke();
            Debug.Log("hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.up) * 0.4f, Color.white);

        }
    }

    private void DownCast()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, 0.4f))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * hit.distance, Color.yellow);
            OnMoveObject?.Invoke();
            Debug.Log("hit");
        }
        else
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.down) * 0.4f, Color.white);

        }
    }

}
