﻿using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LibLabSystem;
using PathologicalGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LibLabGames.JoysticksOnFire
{
    public class GameManager : IGameManager
    {
        public static GameManager instance;

        public int maxPlayerLife;
        public int currentPlayerLife;

        public Crosswire crosswireLeft;
        public Crosswire crosswireRight;

        public float[] countdownSpawnEnemy;

        public GameObject[] enemiesPrefabs;

        public Enemy[] currentEnemies;

        public Transform[] enemiesTransformsParents;
        public Transform[] enemiesInterfaces;
        public Image[] enemiesLifesFills;
        public TextMeshProUGUI[] enemiesNamesText;

        public Transform heartsParent;
        public GameObject heartPrefab;
        public List<Heart> hearts;

        public Image playerHurtedImage;

        public GameObject gameOverDisplay;

        public override void GetSettingGameValues()
        {
            // Example :
            // int value = settingValues.GetIntValue("exampleThree");
        }

        private void Awake()
        {
            if (!DOAwake()) return;

            instance = this;
        }

        private void Start()
        {
            base.DOStart();

            crosswireLeft.speed = settingValues.GetFloatValue("Speed");
            crosswireRight.speed = settingValues.GetFloatValue("Speed");
            maxPlayerLife = (int) settingValues.GetFloatValue("PlayerLife");
            countdownSpawnEnemy = new float[2];
            countdownSpawnEnemy[0] = settingValues.GetFloatValue("EnemySpawnCountdownMin");
            countdownSpawnEnemy[1] = settingValues.GetFloatValue("EnemySpawnCountdownMax");

            currentEnemies = new Enemy[enemiesInterfaces.Length];

            for (int i = 0; i < enemiesInterfaces.Length; ++i)
            {
                Destroy(enemiesTransformsParents[i].GetChild(0).gameObject);
                enemiesInterfaces[i].gameObject.SetActive(false);
            }

            currentPlayerLife = maxPlayerLife;
            for (int i = 0; i < heartsParent.childCount; ++i)
            {
                Destroy(heartsParent.GetChild(i).gameObject);
            }
            hearts = new List<Heart>();
            for (int i = 0; i < maxPlayerLife; ++i)
            {
                hearts.Add(Instantiate(heartPrefab, heartsParent).GetComponent<Heart>());
            }

            gameOverDisplay.SetActive(false);

            SpawnEnemy(0);

            StartCoroutine(SpawnEnemyCoco());
        }

        private IEnumerator SpawnEnemyCoco()
        {
            if (gameHasStarted)
                yield return null;

            while (gameHasStarted)
            {
                while (fullEnemy)
                    yield return null;

                yield return new WaitForSeconds(Random.Range(countdownSpawnEnemy[0], countdownSpawnEnemy[1]));

                for (int i = 0; i < currentEnemies.Length; ++i)
                {
                    if (currentEnemies[i] == null)
                    {
                        SpawnEnemy(i);
                        break;
                    }
                }
            }
        }

        public bool fullEnemy;
        private void SpawnEnemy(int index)
        {
            currentEnemies[index] = Instantiate(enemiesPrefabs[Random.Range(0, enemiesPrefabs.Length)], enemiesTransformsParents[index]).GetComponent<Enemy>();
            currentEnemies[index].index = index;

            fullEnemy = true;
            for (int i = 0; i < currentEnemies.Length; ++i)
            {
                if (currentEnemies[i] == null)
                {
                    fullEnemy = false;
                    break;
                }
            }
        }

        public void PlayerHurted()
        {
            currentPlayerLife--;
            hearts[currentPlayerLife].Disable();

            playerHurtedImage.gameObject.SetActive(true);
            playerHurtedImage.color = new Color(1,0,0,0);
            playerHurtedImage.DOColor(new Color(1,0,0,0.5f), 0.05f).SetLoops(4, LoopType.Yoyo)
                .OnComplete(() => playerHurtedImage.gameObject.SetActive(false));

            if (currentPlayerLife <= 0)
                GameOver();
        }

        private bool mouseControl;
        private Vector3 mousePos;
        private void Update()
        {
            if (!gameHasStarted) return;

            crosswireLeft.transform.Translate(new Vector3(Input.GetAxis("HorizontalLeft"), Input.GetAxis("VerticalLeft"), 0) * Time.deltaTime * crosswireLeft.speed);
            crosswireRight.transform.Translate(new Vector3(Input.GetAxis("HorizontalRight"), Input.GetAxis("VerticalRight"), 0) * Time.deltaTime * crosswireLeft.speed);

            if (Input.GetButtonDown("FireLeft"))
            {
                crosswireLeft.Fire();
            }
            if (Input.GetButtonDown("FireRight"))
            {
                crosswireRight.Fire();
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                mouseControl = !mouseControl;
                LLLog.Log("GameManager", string.Format("Mouse Control : {0}", mouseControl));
            }
            if (mouseControl)
            {
                mousePos = Input.mousePosition;
                mousePos.z = 10;
                crosswireLeft.transform.position = Camera.main.ScreenToWorldPoint(mousePos);
                
                if (Input.GetMouseButton(0))
                {
                    crosswireLeft.Fire();
                }
            }
        }

        public bool onGameOver;
        public void GameOver()
        {
            onGameOver = true;

            for (int i = 0; i < currentEnemies.Length; ++i)
            {
                if (currentEnemies[i] != null)
                    currentEnemies[i].CancelAttack();
            }

            gameOverDisplay.SetActive(true);
        }
    }
}