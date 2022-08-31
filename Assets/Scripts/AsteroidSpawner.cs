using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidSpawner : MonoBehaviour
{
    public GameObject[] asteroidObjects;

    public int amountAsteroidsToSpawn = 10;

    public float minRandomSpawn = -500;
    public float maxRandomSpawn = 500;

    private void Start()
    {
        SpawnAsteroid();
    }

    void SpawnAsteroid()
    {
        //repeatedly execute spawning asteroids while i count is less then 10
        for (int i = 0; i < amountAsteroidsToSpawn; i++)
        {
            //random positions for x,y,z values
            float randomX = Random.Range(minRandomSpawn, maxRandomSpawn);
            float randomY = Random.Range(minRandomSpawn, maxRandomSpawn);
            float randomZ = Random.Range(minRandomSpawn, maxRandomSpawn);

            //spawn all to random point positions
            Vector3 randomSpawnPoint = new Vector3(transform.position.x + randomX,
                transform.position.y + randomY, transform.position.z + randomZ);

            GameObject tempObj = Instantiate(asteroidObjects[0], randomSpawnPoint, Quaternion.identity);
            tempObj.transform.parent = this.transform;
        }
    }

    void OnDrawGizmos()
    {
           Gizmos.DrawWireCube(transform.position, new Vector3(maxRandomSpawn * 2, maxRandomSpawn * 2, maxRandomSpawn * 2));
    }
}
