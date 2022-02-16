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

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Fire1"))
        {
            Shoot();
        }
        
    }

    private void Shoot()
    {
        GameObject bomb = Instantiate(bombPrefab, this.transform.position, this.transform.rotation);
        Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
        Vector3 direction = (cam.ScreenToWorldPoint(Input.mousePosition) - this.transform.position).normalized;
        rb.AddForce(direction * throwForce, ForceMode2D.Impulse);
    }
}
