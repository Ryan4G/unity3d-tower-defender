using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public PathNode m_startNode;
    public int m_liveEnemy = 0;
    public List<WaveData> waves;
    int enemyIndex = 0;
    int waveIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(SpawnEnemies());    
    }

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForEndOfFrame();

        GameManager.Instance.SetWave(waveIndex + 1);

        WaveData wave = waves[waveIndex];
        yield return new WaitForSeconds(wave.interval);

        while (enemyIndex < wave.enemyPrefab.Count)
        {
            Vector3 dir = m_startNode.transform.position - this.transform.position;
            GameObject enemyObj = Instantiate(wave.enemyPrefab[enemyIndex],
                transform.position, Quaternion.LookRotation(dir));

            Enemy enemy = enemyObj.GetComponent<Enemy>();
            enemy.m_currentNode = m_startNode;

            // enemy's parameters

            enemy.m_life = wave.level * 3;
            enemy.m_maxlife = enemy.m_life;

            m_liveEnemy++;
            enemy.onDeath = (Enemy e) =>
            {
                m_liveEnemy--;
            };

            enemyIndex++;

            yield return new WaitForSeconds(wave.interval);
        }

        while(m_liveEnemy > 0)
        {
            yield return 0;
        }

        enemyIndex = 0;
        waveIndex++;

        if(waveIndex < waves.Count)
        {
            StartCoroutine(SpawnEnemies());
        }
        else
        {
            // win the game
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawIcon(this.transform.position, "spawner.tif");
    }
}
