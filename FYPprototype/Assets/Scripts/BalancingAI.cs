using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class BalancingAI : Agent
{
    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameManager gameManager;

    private float ImbalanceValue;

    private List<float> imbalanceList;

    private void Start()
    {
        gameManager.GameOverFlag.AddListener(GameOver);
        imbalanceList = new List<float>();
        File.CreateText("C:/Users/owenc/Documents/GitHub/FYPAIBalancingGame/FYPprototype/TrainingDataLogs/CumulativeImbalanceLog.txt");
    }

    private void GameOver()
    {
        float totalImbalance = 0;
        foreach(float i in imbalanceList)
        {
            totalImbalance += i;
        }
        float cumulativeImbalance = totalImbalance / imbalanceList.Count;
        using (StreamWriter sw = File.AppendText("C:/Users/owenc/Documents/GitHub/FYPAIBalancingGame/FYPprototype/TrainingDataLogs/CumulativeImbalanceLog.txt"))
        {
            sw.WriteLine(cumulativeImbalance);
        }
        imbalanceList = new List<float>();
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
            /*
            int x = actions.DiscreteActions[1] - 9;
            int y = actions.DiscreteActions[2] - 8;
            gameManager.AddMob(x, y);
            */
            gameManager.AddMob();
        }
        

        // get current empowerment for this step.
        float currentEmpowerment = CalculatePlayerEmpowerment();
        // get current imbalance in the game for this step.
        ImbalanceValue = CalculateImbalance(currentEmpowerment);
        imbalanceList.Add(ImbalanceValue);
        Debug.Log("Imbalance: " + ImbalanceValue);
        // calculate reward as 1 - absolute imbalance, where the lowest imbalance is 0.
        AddReward(1 - Mathf.Abs(ImbalanceValue));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        float directions = playerFreedom();
        sensor.AddObservation(directions);
        //for(int i = 0; i < gameManager.wallPositions.Length; i++)
        //{ 
        //sensor.AddObservation(gameManager.wallPositions[i]);
        //}
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
    /// <returns> The players empowerment </returns>;
    private float CalculatePlayerEmpowerment()
    {
        float directions = playerFreedom();
        /*
        float degreesOfFreedom = 0;
        foreach(float i in directions)
        {
            degreesOfFreedom += i;
        }
        float empowerment = degreesOfFreedom / 4;
        Debug.Log("empowerment: " + empowerment);
        */
        float empowerment = directions / 8;
        Debug.Log("empowerment: " + empowerment);
        return empowerment;
    }

    /// <summary>
    /// Calculates the degree of freedom the player currently has in their movement.
    /// </summary>
    /// <returns> An array of values between 0 and 1 representing whether the player can move in a given direction </returns>
    private float playerFreedom()
    {
        /*
        float radius = gameManager.player.gameObject.GetComponent<CircleCollider2D>().radius;
        float[] freedom = new float[4];
        Vector3 position = new Vector3(playerTransform.position.x, playerTransform.position.y, 0);
        freedom[0] = System.Convert.ToInt32(!Physics.Raycast(position, -transform.right, radius, wallsMask));
        //Debug.DrawRay(position, -transform.right, Color.green);
        freedom[1] = System.Convert.ToInt32(!Physics.Raycast(position, transform.right, radius, wallsMask));
        //Debug.DrawRay(position, transform.right, Color.green);
        freedom[2] = System.Convert.ToInt32(!Physics.Raycast(position, transform.up, radius, wallsMask));
        //Debug.DrawRay(position, transform.up, Color.green);
        freedom[3] = System.Convert.ToInt32(! Physics.Raycast(position, -transform.up, radius, wallsMask));
        //Debug.DrawRay(position, -transform.up, Color.green);
        */
        MapTile playerTile = gameManager.FindPositionAsTile(playerTransform.position);



        return playerTile.GetNeighbours().Count;
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
        float top = value - min;
        float bottom = max - min;
        float divided = top / bottom;
        float timesTwo = divided * 2;
        float result = timesTwo - 1;

        
        return result;
    }

}
