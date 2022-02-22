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

    private float ImbalanceValue;

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
        discreteActions[0] = Input.GetKeyDown(KeyCode.Return) ? 1 : 0;
    }



    public override void OnActionReceived(ActionBuffers actions)
    {
        bool spawn;
        if(actions.DiscreteActions[0] == 1)
        {
            spawn = true;
            Debug.Log(spawn);
        }
        else
        {
            spawn = false;
            Debug.Log(spawn);
        }

        if(spawn && gameManager.mobs.Count < gameManager.maxMobs)
        {
            gameManager.AddMob();
        }

        // get current empowerment for this step.
        float currentEmpowerment = CalculatePlayerEmpowerment();
        // get current imbalance in the game for this step.
        ImbalanceValue = CalculateImbalance(currentEmpowerment);

        // calculate reward as 1 - absolute imbalance, where the lowest imbalance is 0.
        AddReward(1 - Mathf.Abs(ImbalanceValue));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        float[] directions = playerFreedom();
        sensor.AddObservation(directions);
        sensor.AddObservation(ImbalanceValue);
    }


    /// <summary>
    /// Calculates the current imbalance in the game for this step
    /// </summary>
    /// <param name="currentEmpowerment"> The empowerment of the player in this step </param>
    /// <returns> a value between -1 and 1 representing the imbalance in the game, where 0 is a perfectly balanced game </returns>
    private float CalculateImbalance(float currentEmpowerment)
    {
        float scoreEmpowProduct = gameManager.challengeRating * currentEmpowerment;
        float imbalance = NormaliseBetweenMinusOneAndOne(scoreEmpowProduct, 0, gameManager.maxChallenge);
        return imbalance;
    }

    

    /// <summary>
    /// Calculates the current empowerment of the player in this step.
    /// </summary>
    /// <returns> The players empowerment </returns>
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

    /// <summary>
    /// Calculates the degree of freedom the player currently has in their movement.
    /// </summary>
    /// <returns> An array of values between 0 and 1 representing whether the player can move in a given direction </returns>
    private float[] playerFreedom()
    {
        float radius = gameManager.player.gameObject.GetComponent<CircleCollider2D>().radius;

        float[] directions = new float[4];
        directions[0] = Physics.Raycast(playerTransform.position, -transform.right, radius) ? 0 : 1;
        directions[1] = Physics.Raycast(playerTransform.position, transform.right, radius) ? 0 : 1;
        directions[2] = Physics.Raycast(playerTransform.position, transform.up, radius) ? 0 : 1;
        directions[3] = Physics.Raycast(playerTransform.position, -transform.up, radius) ? 0 : 1;
        return directions;
    }




    /// <summary>
    /// Normalizes a value from a data set to between -1 and 1
    /// </summary>
    /// <param name="value"> The value to be normalised </param>
    /// <param name="min"> The min range of the original data set </param>
    /// <param name="max"> The max value of the data set </param>
    /// <returns> Normalised value </returns>
    private float NormaliseBetweenMinusOneAndOne(float value, float min, float max)
    {
        return (2 * (value - min / max - min) - 1);
    }

}
