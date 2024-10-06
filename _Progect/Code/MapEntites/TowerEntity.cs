using System;
using KvinterGames;
using TMPro;
using UnityEngine;

namespace Code.MapEntities
{
    public class TowerEntity : MapEntity, IEnemyTarget
    {
        [SerializeField] private int health;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private TMP_Text healthTextUI;
        [SerializeField] private Transform damageMessageRoot;

        private float lastDamageTime = -100;

        public int Priority => 1;
        public Transform Transform => transform;
        public bool IsAlive => health > 0;

        private void Awake()
        {
            healthText.text = health.ToString();
            healthTextUI.text = health.ToString();
        }

        protected override void Start()
        {
            base.Start();
            healthText.text = health.ToString();
            healthTextUI.text = health.ToString();
        }

        public override void Tick()
        {
            SetMessageVisible(false);
            if (Time.time - lastDamageTime < 5) //15, 5
            {
                SetMessageVisible(true);
            }
        }

        private void SetMessageVisible(bool visible)
        {
            if (damageMessageRoot.gameObject.activeSelf == visible)
                return;

            damageMessageRoot.gameObject.SetActive(visible);
        }

        public void Damage(int damage)
        {
            if (health <= 0)
                return;

            lastDamageTime = Time.time;
            SoundController.Instance.PlaySound("hit", pitchRandomness: 0.1f);
            health -= damage;
            health = Math.Max(health, 0);
            healthText.text = health.ToString();
            healthTextUI.text = health.ToString();
            if (health <= 0)
            {
                //Game Over
                GameManager.Instance.GameOver();
                Destroy(gameObject);
            }
        }
    }
}