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

    private void Start()
    {
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

        Vector2 currentPosition = this.rigidbody.position;
        Vector2 translation;
        if (destinationBasedMovement)
        {

            float distance = Vector3.Distance(moveVector, this.transform.position);
            direction = (moveVector - this.transform.position).normalized;
            translation = direction * speed * this.speedMultiplier * Time.fixedDeltaTime;
            if (translation.magnitude > distance)
            {
                translation = new Vector2(moveVector.x - this.transform.position.x, moveVector.y - this.transform.position.y);
            }
        }
        else
        {
            translation = direction * speed * this.speedMultiplier * Time.fixedDeltaTime;
        }

        this.rigidbody.MovePosition(currentPosition + translation);
        this.rigidbody.transform.up = direction;



    }

    public void SetDestinationMoveType(bool type)
    {
        destinationBasedMovement = type;
    }




}
