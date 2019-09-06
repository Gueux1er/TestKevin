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
        public Color noTouchedColor;

        public int index;

        public Transform mainTransform;

        public Collider[] colliders;
        public SpriteRenderer enemySprite;
        public SpriteRenderer enemyTouchSprite;

        [System.Serializable]
        public struct Weapon
        {
            public Collider[] colliders;
            public SpriteRenderer[] spriteRenderers;
        }
        public Weapon[] weapons;

        public Color weaponOnChargeColor;

        private Color baseWeaponColor;

        private void Start()
        {
            baseWeaponColor = weapons[0].spriteRenderers[0].color;

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
            GameManager.instance.enemiesLifesFills[index].fillAmount = currentLife / maxLife;
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
            GameManager.instance.enemiesLifesFills[index].fillAmount = currentLife / maxLife;

            enemyTouchSprite.DOKill();
            enemyTouchSprite.color = noTouchedColor;

            if (currentLife <= 0)
            {
                Death();
            }
            else
            {
                enemyTouchSprite.DOColor(touchedColor, 0.05f).SetLoops(2, LoopType.Yoyo)
                    .OnComplete(() =>
                    {
                        enemyTouchSprite.color = noTouchedColor;
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

            GameManager.instance.PlayerHurted();

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