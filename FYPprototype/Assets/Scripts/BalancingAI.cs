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

    private void Start()
    {
        gameManager.GameOverFlag.AddListener(GameOver);
    }

    private void GameOver()
    {
        EndEpisode();
    }

    public override void OnEpisodeBegin()
    {
        gameManager.NewGame();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<int> discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = Input.GetButtonDown("enter") ? 1 : 0;
    }



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
        AddReward(1 - CalculatePlayerEmpowerment());
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
