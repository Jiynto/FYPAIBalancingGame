using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using System.Linq;

[System.Serializable]
public class MobEvent : UnityEvent<Mob>
{
}

public class Mob : MonoBehaviour
{
    public List<MapTile> route;

    public MobEvent DeathFlag;

    public UnityEvent DamageFlag;

    public Player player;

    [SerializeField]
    private Movement movement;

    private MapTile nextTile;

    private MapTile currentTile;


    private void Start()
    {
        route = new List<MapTile>();
    }


    private void Update()
    {
        if (route.Any())
        {
            if (nextTile == null || Vector3.Distance(nextTile.worldPosition, this.transform.position) == 0 )
            {

                currentTile = nextTile;
                nextTile = route.Last();
                this.movement.moveVector = nextTile.worldPosition;
                route.Remove(nextTile);
                //float xDiff = Mathf.Abs(currentTile.worldPosition.x - nextTile.worldPosition.x);
                //float yDiff = Mathf.Abs(currentTile.worldPosition.y - nextTile.worldPosition.y);
                /*
                if (!currentTile.GetNeighbours().Contains(nextTile))//xDiff > 1 || yDiff > 1)
                {
                    route = new List<MapTile>();
                    nextTile = null;
                }
                else
                {
                    this.movement.moveVector = nextTile.worldPosition;
                    route.Remove(nextTile);
                }
                */
                
            }
        }
       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform == player.transform)
        {
            DamageFlag.Invoke();
            DeathFlag.Invoke(this);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Bomb"))
        {
            Destroy(collision.gameObject);
            DeathFlag.Invoke(this);
        }

    }


    public void SetTiles(MapTile tile)
    {
        currentTile = tile;
        nextTile = tile;
        this.movement.moveVector = nextTile.worldPosition;
    }


    public MapTile GetCurrentGoal()
    {
        return nextTile;
    }


    public void Die()
    {
        Destroy(this.gameObject);
    }

    /**
    private bool CanMove(Vector2 direction)
    {
        Vector3Int gridPosition = groundTileMap.WorldToCell(transform.position + (Vector3)direction);
        if (!groundTileMap.HasTile(gridPosition) || wallTileMap.HasTile(gridPosition))
            return false;
        else return true;

    }
    */

    






}
