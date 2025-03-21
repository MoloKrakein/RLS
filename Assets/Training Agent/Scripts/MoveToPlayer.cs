using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;

public class MoveToPlayer : Agent
{
    [Header("Agent Properties")]
    [SerializeField] float rotationSpeed;
    [SerializeField] float moveSpeed;

    [Header("Player Reference")]
    [SerializeField] Transform playerPosition;
    [SerializeField] Transform[] spawnPoints;

    bool playerDetected;
    bool playerCaught;
    
    Vector3 startPosition;
    Vector3 playerLastSeenPosition;

    RayPerceptionSensorComponent2D ray2d;

    public override void Initialize()
    {
        ray2d = GetComponent<RayPerceptionSensorComponent2D>();
        startPosition = transform.localPosition;
    }

    public override void OnEpisodeBegin()
    {
        if (!playerCaught){
            AddReward(-1f);
        }

        playerDetected = false;
        playerCaught = false;
        transform.localPosition = startPosition;

        // Menentukan spawn pemain secara acak
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform chosenSpawnPoint = spawnPoints[randomIndex];
        playerPosition.localPosition = chosenSpawnPoint.localPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition.x);
        sensor.AddObservation(transform.localPosition.y);
        sensor.AddObservation(transform.localRotation.eulerAngles.z);

        RayPerceptionInput rayInput = ray2d.GetRayPerceptionInput();
        RayPerceptionOutput rayOutput = RayPerceptionSensor.Perceive(rayInput);

        playerDetected = false;

        foreach (var ray in rayOutput.RayOutputs)
        {
            if (ray.HasHit && ray.HitGameObject.CompareTag("Player"))
            {
                playerDetected = true;
                playerLastSeenPosition = ray.HitGameObject.transform.localPosition;
                break;
            }
        }

        sensor.AddObservation(playerDetected ? 1.0f : 0.0f);
        sensor.AddObservation(playerDetected ? playerLastSeenPosition.x : 0.0f);
        sensor.AddObservation(playerDetected ? playerLastSeenPosition.y : 0.0f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int rotation = actions.DiscreteActions[0]; // 0 = tidak berputar, 1 = kiri, 2 = kanan
        int move = actions.DiscreteActions[1]; // 0 = diam, 1 = maju, 2 = mundur

        Vector3 moveDir = Vector3.zero;
        float rotationAmount = 0f;

        if (rotation == 1) rotationAmount = rotationSpeed * Time.deltaTime;
        else if (rotation == 2) rotationAmount = -rotationSpeed * Time.deltaTime;

        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + rotationAmount),
            rotationSpeed * Time.deltaTime);

        if (move == 1) moveDir = transform.up;
        else if (move == 2) moveDir = -transform.up;

        transform.localPosition += moveDir * moveSpeed * Time.deltaTime;

        if (playerDetected)
        {
            float distance = Vector3.Distance(transform.position, playerLastSeenPosition);
            float reward = 0.002f * (1 / (distance + 1));
            AddReward(reward);
        }
        else
        {
            AddReward(-0.01f);
        }

        if(move == 1){
            AddReward(0.003f);
        }

        if(move == 2){
            AddReward(-0.01f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = 0;
        discreteActions[1] = 0;

        if (Input.GetKey(KeyCode.A))
        {
            discreteActions[0] = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            discreteActions[0] = 2;
        }

        if(Input.GetKey(KeyCode.W)){
            discreteActions[1] = 1;
        }
        else if(Input.GetKey(KeyCode.S)){
            discreteActions[1] = 2;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Player")){
            playerCaught = true;
            AddReward(5f);
            EndEpisode();
        }

        if(collision.gameObject.CompareTag("Wall")){
            AddReward(-0.1f);
        }
    }
}
