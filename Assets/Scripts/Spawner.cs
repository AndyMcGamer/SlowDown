using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private GameObject rock;
    [SerializeField] private GameObject torch;
    [SerializeField] private GameObject platform;
    [SerializeField] private GameObject heart;
    [SerializeField] private int numEnemies;
    private int currentEnemies = 0;
    [SerializeField] private float spawnDelay;
    private float spawnTime;
    private int extraEnemies = 0;

    [SerializeField] private float minHeartTime, maxHeartTime;
    private float heartTimer = 0;

    private void Awake()
    {
        Vector2 prev = -1 * Vector3.up;
        for(int i = 0; i < 6; ++i)
        {
            float randomOffset = Random.Range(3f, 6.5f);
            Vector2 pos = new Vector2(0, prev.y + randomOffset);
            Instantiate(platform, pos, Quaternion.identity);
            prev = pos;
        }
        for(int i = 0; i < 7; ++i)
        {
            float pos = Random.Range(-5f, 5f);
            Instantiate(torch, new Vector2(pos, i * 5 + Random.Range(-1.5f, 1.5f)), Quaternion.identity);
        }
        spawnTime = spawnDelay;
        heartTimer = maxHeartTime * 2f;
    }

    private void Update()
    {
        extraEnemies = (int)GameLogic.instance.score / 200;
        if(heartTimer <= 0)
        {
            float pos = Random.Range(-5.5f, 5.5f);
            Instantiate(heart, new Vector2(pos, transform.position.y), Quaternion.identity);
            heartTimer = Random.Range(minHeartTime, maxHeartTime);
        }
        else
        {
            heartTimer -= Time.deltaTime * GameLogic.instance.speedMultiplier;
        }
        if(currentEnemies >= numEnemies + extraEnemies)
        {
            return;
        }
        if(spawnTime > 0)
        {
            spawnTime -= Time.deltaTime;
        }
        else
        {
            currentEnemies++;
            Instantiate(rock, new Vector2(Random.Range(-5f, 5f), transform.position.y), Quaternion.identity);
            spawnTime = spawnDelay;
        }
        
    }
}
