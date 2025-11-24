using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Take : MonoBehaviour
{
    [SerializeField] float distance;
    public Transform pos;
    private Rigidbody rb;
    private TakeOneItem takeOneItem;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        takeOneItem = GameObject.Find("Player").GetComponent<TakeOneItem>();
    }

    private void OnMouseDown()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, distance) && takeOneItem.take == false)
        {
            rb.isKinematic = true;
            takeOneItem.take = true;
            rb.MovePosition(pos.position);
        }
    }

    private void FixedUpdate()
    {
        if (rb.isKinematic == true)
        {
            this.gameObject.transform.position = pos.position;
            if (Input.GetKeyDown(KeyCode.G))
            {
                takeOneItem.take = false;
                rb.useGravity = true;
                rb.isKinematic = false;
                rb.AddForce(Camera.main.transform.forward * 500);
            }
        }    
    }
}
