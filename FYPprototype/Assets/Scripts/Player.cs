using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Movement movement;


    public Vector2 Direction { get { return direction; }  }
    private Vector2 direction;


    private void Start()
    {


    }
    private void Update()
    {
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");
        movement.direction = direction;

    }

}
