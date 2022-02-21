using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Tilemaps;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private Tilemap ground;

    [SerializeField]
    private Tilemap walls;

    [SerializeField]
    private GameObject enemyPrefab;

    public UnityEvent GameOverFlag;

    public Player player;

    public List<Mob> mobs;

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


    private void Start()
    {
        score = 0;
        timeSinceLastPoint = 0;
        ground.CompressBounds();
        walls.CompressBounds();
        mapTiles = AStar.SetTiles(ground, walls);

        /*
        for (int i = 0; i < numMobs; i++)
        {
            AddMob();
        }
        */

    }




    public void AddMob()
    {
        MapTile Location = mapTiles[Random.Range(0, mapTiles.Count)];
        while(walls.HasTile(Location.cellPosition)) Location = mapTiles[Random.Range(0, mapTiles.Count)];
        Vector3 mobPosition = Location.worldPosition;
        //Vector3 mobPosition = new Vector3(5, 1, -1);
        GameObject mobInstance = Instantiate(enemyPrefab, mobPosition, Quaternion.identity);
        Mob newMob = mobInstance.GetComponent<Mob>();
        newMob.player = player;
        newMob.DeathFlag.AddListener(MobDied);
        newMob.DamageFlag.AddListener(PlayerHit);
        mobs.Add(newMob);
    }


    private void MobDied(Mob mob)
    {
        mobs.Remove(mob);
        mob.Die();
    }


    private void Update()
    {
        timeSinceLastPoint = timeSinceLastPoint + Time.deltaTime;
        if(timeSinceLastPoint >= scoreDelay)
        {
            score++;
            scoreText.text = "Score:" +score;
            timeSinceLastPoint -= scoreDelay;
        }
        //if (mobs.Count < 2) AddMob();
        foreach (Mob mob in mobs)
        {
            mob.route = AStar.Search(mob.transform.position, player.transform.position);
        }

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
        this.player.gameObject.SetActive(true);

    }

    private void GameOver()
    {
        player.gameObject.SetActive(false);
        score = 0;
        while(mobs.Any())
        {
            MobDied(mobs.First());
        }
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
