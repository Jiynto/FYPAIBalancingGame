using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Player : MonoBehaviour
{
    [SerializeField]
    private Movement movement;

    [SerializeField]
    private bool AIMode;

    [SerializeField] private GameManager gameManager;

    [SerializeField] private int minDistance;

    [SerializeField] private Shooting shoot;

    [SerializeField] private float fireRate;

    private float fireCoolDown;

    public List<MapTile> route;

    private MapTile nextTile;

    private MapTile currentTile;

    public Vector2 Direction { get { return direction; } }
    private Vector2 direction;



    /*
     * for each mob check distance, and if within distance calculate vector away from them
     * for each vector calculated, calculate the vector between them
     * if that vector points towards a wall, try to move in a similar direction that isnt blocke.
     * if that would result in hitting an enemy, stay still.
     */

    // Start is called before the first frame update
    void Start()
    {
        //movement = this.gameObject.GetComponent<Movement>();
        if(AIMode)
        {
            movement.SetDestinationMoveType(true);
        }
        route = new List<MapTile>();

    }

    // Update is called once per frame
    void Update()
    {
        if (AIMode)
        {
            if (fireCoolDown > 0) fireCoolDown -= Time.deltaTime;
            foreach (Mob mob in gameManager.mobs)
            {
                Vector3 mobDirection = (mob.gameObject.transform.position - this.transform.position).normalized;

                if (Vector3.Distance(this.transform.position, mob.gameObject.transform.position) <= minDistance && shoot.Bombs > 0 && fireCoolDown <= 0)
                {
                    shoot.AltShoot(mobDirection);
                    fireCoolDown = fireRate;
                }
                //Vector3 retreatDirection = new Vector3(-mobDirection.x, -mobDirection.y, -mobDirection.z);
                //direction += retreatDirection;


                if (route.Any())
                {
                    if (nextTile == null || Vector3.Distance(nextTile.worldPosition, this.transform.position) == 0)
                    {
                        currentTile = nextTile;
                        nextTile = route.Last();
                        this.movement.moveVector = nextTile.worldPosition;
                        route.Remove(nextTile);

                    }
                }




            }
        }
        else
        {
            direction.x = Input.GetAxisRaw("Horizontal");
            direction.y = Input.GetAxisRaw("Vertical");
            movement.moveVector = direction;
        }



    }

    public MapTile GetCurrentGoal()
    {
        return nextTile;
    }


    public void ClearMovement(MapTile location)
    {
        route = new List<MapTile>();
        direction = Vector2.zero;
        currentTile = location;
        nextTile = location;
        this.movement.moveVector = nextTile.worldPosition;
    }

}
