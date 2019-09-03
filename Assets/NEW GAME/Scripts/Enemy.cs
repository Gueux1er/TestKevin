using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LibLabSystem;
using UnityEngine;

namespace LibLabGames.JoysticksOnFire
{
    public class Enemy : MonoBehaviour
    {
        public string enemyName;

        public float maxLife;
        public float currentLife;
        public float countdownAttack;

        public Color touchedColor;

        public int index;

        public Transform mainTransform;

        public Collider[] colliders;
        public SpriteRenderer enemySprite;

        [System.Serializable]
        public struct Weapon
        {
            public Collider[] colliders;
            public SpriteRenderer[] spriteRenderers;
        }
        public Weapon[] weapons;

        public Color weaponOnChargeColor;

        private Color baseEnemyColor;
        private Color baseWeaponColor;

        private void Start()
        {
            baseEnemyColor = enemySprite.color;
            baseWeaponColor = Color.white;

            Init();

            countdownAttackCoco = CountdownAttackCoco();
            StartCoroutine(countdownAttackCoco);
        }

        private void Init()
        {
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;

            for (int i = 0; i < weapons.Length; ++i)
            {
                for (int j = 0; j < weapons[i].colliders.Length; ++j)
                {
                    weapons[i].colliders[j].enabled = false;
                }
            }

            maxLife = GameManager.instance.settingValues.GetFloatValue(string.Format("EnemyLife_{0}", enemyName));
            countdownAttack = GameManager.instance.settingValues.GetFloatValue(string.Format("EnemyAttack_{0}", enemyName));
            currentLife = maxLife;

            GameManager.instance.enemiesInterfaces[index].gameObject.SetActive(true);
            GameManager.instance.enemiesNamesText[index].text = enemyName;
            GameManager.instance.enemiesLifesSlider[index].value = currentLife / maxLife;
        }

        public bool onChargeAttack;
        public int attackWeaponIndex;
        IEnumerator countdownAttackCoco;
        IEnumerator CountdownAttackCoco()
        {
            if (GameManager.instance.onGameOver || isDead)
                yield break;

            onChargeAttack = false;
            for (int i = 0; i < weapons[attackWeaponIndex].spriteRenderers.Length; ++i)
            {
                weapons[attackWeaponIndex].spriteRenderers[i].DOKill();
                weapons[attackWeaponIndex].spriteRenderers[i].color = baseWeaponColor;
            }

            for (int i = 0; i < weapons[attackWeaponIndex].colliders.Length; ++i)
            {
                weapons[attackWeaponIndex].colliders[i].enabled = false;
            }

            attackWeaponIndex = Random.Range(0, weapons.Length);

            for (int i = 0; i < weapons[attackWeaponIndex].colliders.Length; ++i)
            {
                weapons[attackWeaponIndex].colliders[i].enabled = false;
            }

            yield return new WaitForSeconds(countdownAttack);

            if (GameManager.instance.onGameOver || isDead)
                yield break;

            onChargeAttack = true;

            for (int i = 0; i < weapons[attackWeaponIndex].colliders.Length; ++i)
            {
                weapons[attackWeaponIndex].colliders[i].enabled = true;
            }

            for (int i = 0; i < weapons[attackWeaponIndex].spriteRenderers.Length; ++i)
            {
                weapons[attackWeaponIndex].spriteRenderers[i].DOColor(weaponOnChargeColor, 0.1f).SetLoops(30, LoopType.Yoyo)
                    .OnComplete(() =>
                    {
                        Attack();
                    });
            }
        }

        public void Touched(float strenght)
        {
            if (isDead) return;

            currentLife -= strenght;
            GameManager.instance.enemiesLifesSlider[index].value = currentLife / maxLife;

            enemySprite.DOKill();
            enemySprite.color = baseEnemyColor;

            if (currentLife <= 0)
            {
                Death();
            }
            else
            {
                enemySprite.DOColor(touchedColor, 0.05f).SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() =>
                    {
                        enemySprite.color = baseEnemyColor;
                    });
            }
        }

        public void CancelAttack()
        {
            if (!onChargeAttack) return;

            StopCoroutine(countdownAttackCoco);
            if (!GameManager.instance.onGameOver)
            {
                countdownAttackCoco = CountdownAttackCoco();
                StartCoroutine(countdownAttackCoco);
            }
        }

        private void Attack()
        {
            if (!onChargeAttack || isDead) return;

            onChargeAttack = false;

            GameManager.instance.PlayerHitted();

            var t = LLPoolManager.instance.SpawnEnemyAttackParticle(weapons[attackWeaponIndex].spriteRenderers[0].transform);
            t.localPosition = Vector3.zero;

            StopCoroutine(countdownAttackCoco);
            if (!GameManager.instance.onGameOver)
            {
                countdownAttackCoco = CountdownAttackCoco();
                StartCoroutine(countdownAttackCoco);
            }
        }

        private bool isDead;
        public void Death()
        {
            isDead = true;

            CancelAttack();

            GameManager.instance.fullEnemy = false;

            mainTransform.gameObject.SetActive(false);
            GameManager.instance.enemiesInterfaces[index].gameObject.SetActive(false);

            var t = LLPoolManager.instance.SpawnEnemyDeathParticle(transform.position);

            DOVirtual.DelayedCall(4f, () =>
            {
                Destroy(gameObject);
            });
        }
    }
}