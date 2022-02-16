using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField]
    private float timeLimit;
    private float timeAppeared;
    // Start is called before the first frame update
    void Start()
    {
        timeAppeared = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time - timeAppeared >= timeLimit)
        {
            Destroy(this.gameObject);
        }
    }
}
