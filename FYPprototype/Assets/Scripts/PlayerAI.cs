using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerAI : MonoBehaviour
{

    [SerializeField] private GameManager gameManager;

    [SerializeField] private int minDistance;

    [SerializeField] private Shooting shoot;

    /*
     * for each mob check distance, and if within distance calculate vector away from them
     * for each vector calculated, calculate the vector between them
     * if that vector points towards a wall, try to move in a similar direction that isnt blocke.
     * if that would result in hitting an enemy, stay still.
     */

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        List<Vector3> directions = new List<Vector3>();
        Vector3 direction = Vector3.zero;
        foreach(Mob mob in gameManager.mobs)
        {
            Vector3 mobDirection = (this.transform.position - mob.gameObject.transform.position).normalized;
            if (Vector3.Distance(this.transform.position, mob.gameObject.transform.position) <= minDistance && shoot.Bombs > 0)
            {
                shoot.AltShoot(mobDirection);
            }
            Vector3 retreatDirection = new Vector3(-mobDirection.x, -mobDirection.y, -mobDirection.z);
            direction += retreatDirection;
        }
        Vector3 target = new Vector3();

        //cast in direction
        //some distance
        //if hit go further?
        //min distance half player radius
        //max like 6 players or wall of the arena 
        //set walls on a different layer?


        Ray ray = new Ray(this.gameObject.transform.position, direction);
        if (Physics.Raycast(this.gameObject.transform.position, direction))
        {

        }

        /*
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            target = hit.point;
        }
        else
        {
            target = ray.GetPoint(range);
        }
        */


    }
}
