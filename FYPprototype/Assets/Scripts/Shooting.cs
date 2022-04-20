using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Shooting : MonoBehaviour
{
    [SerializeField]
    private float throwForce = 5;

    [SerializeField]
    private GameObject bombPrefab;

    [SerializeField]
    private Camera cam;

    [SerializeField]
    private int maxBombs;

    [SerializeField] private bool aIPlayer;

    [SerializeField]
    private int bombRegenTime;

    private float timePassed;


    public int Bombs { get { return bombs; } }

    private int bombs;


    private void Start()
    {
        bombs = maxBombs;
        timePassed = 0;
    }

    // Update is called once per frame
    void Update()
    {

        if(aIPlayer == false)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        

        if(bombs < maxBombs)
        {
            if(timePassed >= bombRegenTime)
            {
                timePassed = 0;
                bombs += 1;

            }
            else
            {
                timePassed += Time.deltaTime;
            }
        }
        
    }

    public void AltShoot(Vector3 direction)
    {
        if (bombs > 0)
        {
            bombs --;
            GameObject bomb = Instantiate(bombPrefab, this.transform.position, this.transform.rotation);
            Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
            rb.AddForce(direction * throwForce, ForceMode2D.Impulse);
        }
    }



    private void Shoot()
    {
        if(bombs > 0)
        {
            bombs --;
            GameObject bomb = Instantiate(bombPrefab, this.transform.position, this.transform.rotation);
            Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
            Vector3 direction = (cam.ScreenToWorldPoint(Input.mousePosition) - this.transform.position).normalized;
            rb.AddForce(direction * throwForce, ForceMode2D.Impulse);
        }

    }



}
