using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class BalancingAI : Agent
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameManager gameManager;

    public override void OnActionReceived(ActionBuffers actions)
    {
        bool spawn;
        if(actions.DiscreteActions[0] == 1)
        {
            spawn = true;
        }
        else
        {
            spawn = false;
        }

        if(spawn)
        {
            gameManager.AddMob();
        }

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        float[] directions = playerFreedom();
        sensor.AddObservation(directions);
    }

    


    private float CalculatePlayerEmpowerment()
    {
        float[] directions = playerFreedom();
        float degreesOfFreedom = 0;
        foreach(float i in directions)
        {
            degreesOfFreedom += i;
        }
        float empowerment = degreesOfFreedom / 4;
        return empowerment;
    }

    private float[] playerFreedom()
    {
        float[] directions = new float[4];
        directions[0] = Physics.Raycast(playerTransform.position, -transform.right, 1f) ? 1 : 0;
        directions[1] = Physics.Raycast(playerTransform.position, transform.right, 1f) ? 1 : 0;
        directions[2] = Physics.Raycast(playerTransform.position, transform.up, 1f) ? 1 : 0;
        directions[3] = Physics.Raycast(playerTransform.position, -transform.up, 1f) ? 1 : 0;
        return directions;
    }


}
