using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap ground;

    [SerializeField]
    private Tilemap walls;


    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private GameObject pointRep;

    [SerializeField]
    private GameObject pointRepVarient;

    public Vector2Int[] wallPositions;


    public UnityEvent GameOverFlag;

    public Player player;

    public List<Mob> mobs;

    public int challengeRating;

    public int maxChallenge;

    private List<MapTile> mapTiles;

    [SerializeField]
    private int lives;

    [SerializeField]
    private int numMobs;

    [SerializeField]
    public int maxMobs;

    private int score;
    private float timeSinceLastPoint;

    [SerializeField]
    private TextMeshProUGUI scoreText;

    private int scoreDelay = 1;

    private MapTile playerTile;

    [SerializeField]
    private float pathFindingTimer;

    private float pathFindingCounter;

    private void Start()
    {
        score = 0;
        timeSinceLastPoint = 0;
        ground.CompressBounds();
        walls.CompressBounds();
        mapTiles = AStar.SetTiles(ground, walls);
        wallPositions = AStar.GetWallPositions();
        
        /*
        foreach(Vector2Int position in wallPositions)
        {
            Vector3Int _location = new Vector3Int(position.x, position.y, 0);
            List<MapTile> tiles = mapTiles.Where(x => x.cellPosition == _location).ToList();
            if(tiles.Any())
            {
                MapTile tile = tiles[0];
                AddPointRep(tile.worldPosition, false);
            }

        }
        */
        playerTile = FindPositionAsTile(player.gameObject.transform.position);

        /*
        for (int i = 0; i < numMobs; i++)
        {
            AddMob();
        }
        */

        pathFindingCounter = 0;
    }

    public MapTile FindPositionAsTile(Vector3 position)
    {
        Vector3 currentPosition = new Vector3(position.x, position.y, 0);
        Vector3Int currentPositionInt = ground.WorldToCell(currentPosition);
        MapTile currrentTile = mapTiles.Where(x => x.cellPosition == currentPositionInt).First();
        return currrentTile;
    }


    public void AddPointRep(Vector3 position, bool varient)
    {
        if(varient)
        {
            Instantiate(pointRepVarient, position, Quaternion.identity);
        }
        else
        {
            Instantiate(pointRep, position, Quaternion.identity);
        }
        
    }



    public void AddMob()
    {
        MapTile Location = mapTiles[Random.Range(0, mapTiles.Count)];
        while(walls.HasTile(Location.cellPosition) || Vector3.Distance(Location.worldPosition, player.gameObject.transform.position) < 4 ) Location = mapTiles[Random.Range(0, mapTiles.Count)];
        Vector3 mobPosition = Location.worldPosition;
        GameObject mobInstance = Instantiate(enemyPrefab, mobPosition, Quaternion.identity);
        Mob newMob = mobInstance.GetComponent<Mob>();
        newMob.SetTiles(Location);
        newMob.player = player;
        newMob.DeathFlag.AddListener(MobDied);
        newMob.DamageFlag.AddListener(PlayerHit);
        mobs.Add(newMob);
        newMob.route = AStar.Search(newMob.transform.position, player.transform.position);

        //AddPointRep(newMob.route.Last().worldPosition, false);

    }


    public void AddMob(int xPosition, int yPosition)
    {
        Vector3Int _location = new Vector3Int(xPosition, yPosition, 0);
        if (!walls.HasTile(_location))
        {
            List<MapTile> tiles = mapTiles.Where(x => x.cellPosition == _location).ToList();
            if (tiles.Any())
            {
                MapTile Location = tiles[0];
                AddPointRep(Location.worldPosition, false);
                Vector3 mobPosition = Location.worldPosition;
                GameObject mobInstance = Instantiate(enemyPrefab, mobPosition, Quaternion.identity);
                Mob newMob = mobInstance.GetComponent<Mob>();
                newMob.SetTiles(Location);
                newMob.player = player;
                newMob.DeathFlag.AddListener(MobDied);
                newMob.DamageFlag.AddListener(PlayerHit);
                mobs.Add(newMob);
                newMob.route = AStar.Search(newMob.transform.position, player.transform.position);

            }
        }
       

       
        //AddPointRep(newMob.route.Last().worldPosition, false);

    }


    private void MobDied(Mob mob)
    {
        mobs.Remove(mob);
        mob.Die();
    }


    private void Update()
    {
        pathFindingCounter -= Time.deltaTime;
        timeSinceLastPoint += Time.deltaTime;
        if(timeSinceLastPoint >= scoreDelay)
        {
            score++;
            scoreText.text = "Score:" +score;
            timeSinceLastPoint -= scoreDelay;
        }

        /*
        if(mobs.Count < numMobs)
        {
            AddMob();
        }
        */

        List<Vector3> mobPositions = new List<Vector3>();

        MapTile currentPlayerTile = FindPositionAsTile(player.gameObject.transform.position);
        bool playerMoved = false;
        if (currentPlayerTile != playerTile )
        {
            playerMoved = true;
            playerTile = currentPlayerTile;
            
        }
        foreach (Mob mob in mobs)
        {
            if(playerMoved)
            {
                mob.route = AStar.Search(mob.GetCurrentGoal().worldPosition, player.transform.position);
                //AddPointRep(mob.route.First().worldPosition, true);
            }
            else if(pathFindingCounter <= 0)
            {
                mob.route = AStar.Search(mob.GetCurrentGoal().worldPosition, player.transform.position);
            }
            mobPositions.Add(mob.gameObject.transform.position);

        }

        if(pathFindingCounter <= 0)
        {
            player.route = AStar.AvoidanceSearch(player.GetCurrentGoal().worldPosition, mobPositions);
            pathFindingCounter = pathFindingTimer;
        }
        
        //AddPointRep(player.route.Last().worldPosition, false);


    }
    public void NewGame()
    {
        SetLives(3);
        
        /*
        for (int i = 0; i < numMobs; i++)
        {
            AddMob();
        }
        */
        
        this.player.gameObject.transform.position = new Vector3(0, 0, -1);
        List<MapTile> tiles = mapTiles.Where(x => x.cellPosition == Vector3.zero).ToList();
        this.player.ClearMovement(tiles.First());
        this.player.gameObject.SetActive(true);

    }

    private void GameOver()
    {
        player.gameObject.SetActive(false);
        score = 0;
        GameObject[] allObjects = GameObject.FindGameObjectsWithTag("PointRep");
        foreach (GameObject obj in allObjects)
        {
            Destroy(obj);
        }

        while (mobs.Any())
        {
            MobDied(mobs.First());
        }
        NewGame();
    }


    private void SetLives(int lives)
    {
        this.lives = lives;
    }

    public void PlayerHit()
    {
        SetLives(this.lives - 1);
        if(this.lives <= 0)
        {
            GameOver();
            GameOverFlag.Invoke();
        }
    }


}
