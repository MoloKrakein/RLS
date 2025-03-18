using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MoveToPlayer : Agent
{
    [Header("Agent Properties")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotationSpeed;

    [Header("Player Reference")]
    [SerializeField] Transform playerPosition;
    [SerializeField] Transform[] spawnPoints;

    [Header("Tilemap Color Properties")]
    [SerializeField] Tilemap wall;
    [SerializeField] Color whenSuccessColor;
    [SerializeField] Color whenFailedColor;
    [SerializeField] Color defaultColor;

    bool playerDetected;
    bool playerCaught;
    
    Vector3 startPosition;
    Vector3 playerLastPosition;
    float lastDistanceToPlayer;

    RayPerceptionSensorComponent2D ray2d;

    public override void Initialize()
    {
        ray2d = GetComponent<RayPerceptionSensorComponent2D>();
        startPosition = transform.localPosition;
    }

    public override void OnEpisodeBegin()
    {
        if (!playerCaught)
        {
            AddReward(-0.5f); // Penalti jika gagal menangkap pemain
        }

        playerCaught = false;
        playerDetected = false;
        wall.color = defaultColor;
        transform.localPosition = startPosition;

        // Menentukan spawn pemain secara acak
        int randomIndex = Random.Range(0, spawnPoints.Length);
        Transform chosenSpawnPoint = spawnPoints[randomIndex];
        playerPosition.localPosition = chosenSpawnPoint.localPosition;

        lastDistanceToPlayer = Vector3.Distance(transform.localPosition, playerPosition.localPosition);
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
                playerLastPosition = ray.HitGameObject.transform.position;
                AddReward(0.005f); // Reward lebih kecil agar agen tidak hanya melihat
                break;
            }
        }

        sensor.AddObservation(playerDetected ? 1.0f : 0.0f);
        sensor.AddObservation(playerDetected ? playerLastPosition.x : 0.0f);
        sensor.AddObservation(playerDetected ? playerLastPosition.y : 0.0f);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        int walk = actions.DiscreteActions[0]; // 0 = diam, 1 = maju, 2 = mundur
        int rotation = actions.DiscreteActions[1]; // 0 = tidak berputar, 1 = kiri, 2 = kanan

        Vector3 moveDir = Vector3.zero;
        float rotationAmount = 0f;

        if (walk == 1) moveDir = transform.up; // Maju
        else if (walk == 2) moveDir = -transform.up; // Mundur (diberi penalti)

        if (rotation == 1) rotationAmount = rotationSpeed * Time.deltaTime;
        else if (rotation == 2) rotationAmount = -rotationSpeed * Time.deltaTime;

        transform.localPosition += moveDir * moveSpeed * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards(transform.rotation,
            Quaternion.Euler(0, 0, transform.rotation.eulerAngles.z + rotationAmount),
            rotationSpeed * Time.deltaTime);

        float currentDistance = Vector3.Distance(transform.localPosition, playerPosition.localPosition);
        float distanceChange = lastDistanceToPlayer - currentDistance;

        // **Reward jika agen mendekati pemain**
        if (distanceChange > 0)
        {
            AddReward(distanceChange * 0.1f);
        }
        else
        {
            AddReward(-0.02f); // Penalti jika menjauh
        }

        // **Penalti jika agen mundur saat melihat player**
        if (playerDetected && walk == 2)
        {
            AddReward(-0.05f); // Penalti lebih besar jika mundur ke pemain
        }

        // **Penalti jika agen hanya berputar tanpa bergerak maju**
        if (rotation != 0 && walk == 0)
        {
            AddReward(-0.02f);
        }

        lastDistanceToPlayer = currentDistance;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActions = actionsOut.DiscreteActions;
        discreteActions[0] = 0;
        discreteActions[1] = 0;

        if (Input.GetKey(KeyCode.W))
        {
            discreteActions[0] = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActions[0] = 2;
        }

        if (Input.GetKey(KeyCode.A))
        {
            discreteActions[1] = 1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            discreteActions[1] = 2;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            wall.color = whenSuccessColor;
            playerCaught = true;
            AddReward(2f);
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.1f);
        }
    }
}
