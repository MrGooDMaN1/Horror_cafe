using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController characterController;

    [SerializeField]  float _speed;
    [SerializeField]  float _gravity;


    Vector3 velocity;

    void FixedUpdate()
    {

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        characterController.Move(move * _speed * Time.deltaTime);


        velocity.y += _gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);
    }

}
