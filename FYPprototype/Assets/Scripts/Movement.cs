using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(Rigidbody2D))]

public class Movement : MonoBehaviour
{

    [SerializeField] private bool destinationBasedMovement;

    public float Movespeed = 8;
    public float speedMultiplier = 1;
    public new Rigidbody2D rigidbody { get; private set; }

    public Vector3 moveVector;


    public Vector3 startingPosition { get; private set; }

    private void Awake()
    {
        this.rigidbody = GetComponent<Rigidbody2D>();
        this.startingPosition = this.transform.position;
    }

    public void ResetState()
    {
        this.speedMultiplier = 1;
        this.transform.position = this.startingPosition;
        this.rigidbody.isKinematic = false;
        this.enabled = true;
    }

    private void FixedUpdate()
    {
        float speed = Movespeed;
        Vector2 direction = moveVector;

        if(destinationBasedMovement)
        {
            direction = (moveVector - this.transform.position).normalized;
            float distance = Vector3.Distance(moveVector, this.transform.position);
            if (distance < Movespeed)
            {
                speed = distance;
            }
        }

        Vector2 currentPosition = this.rigidbody.position;
        Vector2 translation = direction * speed * this.speedMultiplier * Time.fixedDeltaTime;
        this.rigidbody.MovePosition(currentPosition + translation);
        this.rigidbody.transform.up = direction;
    }

    private void Move(Vector2 direction)
    {
        //Vector2 move = speed * (transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal"));


    }




}
