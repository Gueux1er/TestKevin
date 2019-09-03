using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using LibLabSystem;
using UnityEngine;

namespace LibLabGames.JoysticksOnFire
{
    public class Crosswire : MonoBehaviour
    {
        public float speed;
        public bool onFire;
        public Color fireColor;

        public SpriteRenderer sp;
        public SphereCollider col;

        private Color baseColor;

        private void Start()
        {
            baseColor = sp.color;
        }

        private RaycastHit hit;
        private Enemy enemyTouched;
        public void Fire()
        {
            if (onFire) return;

            onFire = true;

            sp.DOColor(fireColor, 0.05f).SetLoops(6, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    onFire = false;
                    sp.color = baseColor;
                });

            // Layer Weapon
            if (Physics.Raycast(transform.position + Vector3.back * 100, Vector3.forward, out hit, Mathf.Infinity, 1 << 10))
            {
                enemyTouched = hit.collider.GetComponentInParent<Enemy>();
                enemyTouched.CancelAttack();
            }
            // Layer Enemy (main and weak)
            if (Physics.Raycast(transform.position + Vector3.back * 100, Vector3.forward, out hit, Mathf.Infinity, 1 << 11 | 1 << 12))
            {
                enemyTouched = hit.collider.GetComponentInParent<Enemy>();
                enemyTouched.Touched((hit.collider.gameObject.layer == 11) ? 1 : 3);

                LLPoolManager.instance.SpawnEnemyTouchParticle(transform.position);
            }
        }
    }
}