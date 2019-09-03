using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathologicalGames;
using TMPro;

namespace LibLabSystem
{
    public class LLPoolManager : MonoBehaviour
    {
        public static LLPoolManager instance;

        public GameObject enemyTouchParticlePrefab;
        public GameObject enemyAttackParticlePrefab;
        public GameObject enemyDeathParticlePrefab;
        
        void Awake()
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            transform.position = Vector3.zero;
            transform.eulerAngles = Vector3.zero;
        }

        void OnSceneLoaded()
        {
            transform.position = Vector3.zero;
            transform.eulerAngles = Vector3.zero;
        }

        public Transform SpawnEnemyTouchParticle(Vector3 position)
        {
            return PoolManager.Pools["POOL"].Spawn(enemyTouchParticlePrefab, position, Quaternion.identity);
        }

        public Transform SpawnEnemyAttackParticle(Transform parent)
        {
            return PoolManager.Pools["POOL"].Spawn(enemyAttackParticlePrefab, parent);
        }

        public Transform SpawnEnemyDeathParticle(Vector3 position)
        {
            return PoolManager.Pools["POOL"].Spawn(enemyDeathParticlePrefab, position, Quaternion.identity);
        }

        public void DespawnAll()
        {
            PoolManager.Pools["POOL"].DespawnAll();
        }

        public void Despawn(Transform t)
        {
            PoolManager.Pools["POOL"].Despawn(t, transform);
        }
    }
}