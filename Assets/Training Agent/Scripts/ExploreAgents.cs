using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine.Tilemaps;

public class ExploreAgents : Agent
{
    [Header("Agent Properties")]
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] Transform playerPosition;
    [SerializeField] Tilemap tilemap;

    [Header("Exploration Settings")]
    [SerializeField] Transform[] spawnPoints;

    private HashSet<Vector2Int> visitedTiles = new HashSet<Vector2Int>();
    private Dictionary<Vector2Int, float> lastVisitTime = new Dictionary<Vector2Int, float>();

    public override void OnEpisodeBegin()
    {
        int randomIndex = Random.Range(0, spawnPoints.Length);
        transform.localPosition = spawnPoints[randomIndex].localPosition;
        transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));

        int randomPlayerIndex = Random.Range(0, spawnPoints.Length);
        playerPosition.localPosition = spawnPoints[randomPlayerIndex].localPosition;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(transform.rotation.eulerAngles.z);
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        Debug.Log(GetTotalTileCount());

        int rotateAction = actions.DiscreteActions[0];
        int moveAction = actions.DiscreteActions[1];

        Rotate(rotateAction);
        Move(moveAction);

        Vector2Int currentTile = GetAgentTilePosition();
        float currentTime = Time.time;

        float reward = 0f;

        // Reward untuk tile baru
        if (!visitedTiles.Contains(currentTile))
        {
            visitedTiles.Add(currentTile);
            reward += 0.1f;  // Reward eksplorasi awal
        }
        else
        {
            // Beri penalti kecil jika terlalu sering kembali ke tile yang sama
            if (lastVisitTime.ContainsKey(currentTile) && currentTime - lastVisitTime[currentTile] < 5f)
            {
                reward -= 0.05f;
            }
        }

        // Update waktu kunjungan
        lastVisitTime[currentTile] = currentTime;

        // Bonus jika mencapai milestone eksplorasi
        float exploredRatio = (float)visitedTiles.Count / GetTotalTileCount();
        if (exploredRatio >= 0.25f) reward += 1.5f;
        if (exploredRatio >= 0.50f) reward += 3f;
        if (exploredRatio >= 0.75f) reward += 5f;

        AddReward(reward);
    }

    private void Rotate(int action)
    {
        if (action == 1) transform.Rotate(Vector3.forward * rotateSpeed * Time.deltaTime);
        else if (action == 2) transform.Rotate(Vector3.back * rotateSpeed * Time.deltaTime);
    }

    private void Move(int action)
    {
        if (action == 1) transform.position += transform.up * moveSpeed * Time.deltaTime;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            AddReward(-0.5f);
            EndEpisode();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player")){
            AddReward(10f);
            EndEpisode();
        }
    }

    public Vector2Int GetAgentTilePosition()
    {
        Vector3 localPos = transform.localPosition;  // Posisi agen dalam world space
        Vector3Int tilePos = tilemap.WorldToCell(localPos); // Konversi ke grid

        return new Vector2Int(tilePos.x, tilePos.y);
    }

    public int GetTotalTileCount()
    {
        int count = 0;
        BoundsInt bounds = tilemap.cellBounds;

        foreach (var pos in bounds.allPositionsWithin)
        {
            if (tilemap.HasTile((Vector3Int)pos)) // Cek apakah ada tile di posisi ini
            {
                count++;
            }
        }
        return count;
    }
}
