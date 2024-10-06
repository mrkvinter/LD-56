using System;
using TMPro;
using UnityEngine;

namespace Code.MapEntities
{
    public class TowerEntity : MapEntity, IEnemyTarget
    {
        [SerializeField] private int health;
        [SerializeField] private TMP_Text healthText;

        public int Priority => 1;
        public Transform Transform => transform;

        protected override void Start()
        {
            base.Start();
            healthText.text = health.ToString();
        }

        public void Damage(int damage)
        {
            health -= damage;
            health = Math.Max(health, 0);
            healthText.text = health.ToString();
            if (health <= 0)
            {
                //Game Over
                Destroy(gameObject);
            }
        }
    }
}