using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField] private GameObject _enemyContainer;
    [SerializeField] private GameObject _enemyPrefab;
    [SerializeField] private GameObject[] _powerupsPrefab;
    private bool _stopSpawning;

    private IEnumerator SpawnEnemy()
    {
        float duration = 2f;
        yield return new WaitForSeconds(duration);
        while (!_stopSpawning)
        {
            float xPos = 9;
            float yPos = 7;
            float randomX = Random.Range(-xPos, xPos);
            Vector3 spawnPos = new Vector3(randomX, yPos, 0);
            GameObject newEnemy = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity);
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(5f);
        }
    }

    private IEnumerator SpawnPowerup()
    {
        float duration = 2f;
        yield return new WaitForSeconds(duration);
        while (!_stopSpawning)
        {
            float xPos = 9;
            float yPos = 7;
            float randomX = Random.Range(-xPos, xPos);
            Vector3 spawnPos = new Vector3(randomX, yPos, 0);
            int randomPowerup = Random.Range(0, _powerupsPrefab.Length);
            Instantiate(_powerupsPrefab[randomPowerup], spawnPos, Quaternion.identity);

            float minInclusive = 3f;
            float maxExclusive = 8f;
            float randomTime = Random.Range(minInclusive, maxExclusive);
            yield return new WaitForSeconds(randomTime);
        }
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    public void StartSpawning()
    {
        _stopSpawning = false;
        StartCoroutine(SpawnEnemy());
        StartCoroutine(SpawnPowerup());
    }
}
