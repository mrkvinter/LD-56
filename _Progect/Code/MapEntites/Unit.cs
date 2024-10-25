using System;
using System.Collections.Generic;
using System.Linq;
using Animancer;
using DG.Tweening;
using KvinterGames;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Code.MapEntities
{
    public class Unit : MapEntity, IEnemyTarget
    {
        [SerializeField] private float speed = 1f;
        [SerializeField] private float collectTime = 1f;
        [SerializeField] private float collectDistance = 0.1f;
        [SerializeField] private bool isEnemy;
        [SerializeField] private int damage = 1;
        [SerializeField] private int health = 1;
        [SerializeField] private TMP_Text healthText;
        [SerializeField] private Image hpPanel;
        [SerializeField] private Color damageColor;
        
        [Space]
        [SerializeField] private AnimancerComponent animancer;
        [SerializeField] private ClipTransition idle;
        [SerializeField] private ClipTransition move;
        [SerializeField] private ClipTransition collect;
        [SerializeField] private ClipTransition attack;

        [ShowInInspector] private State state = State.Search;
        private IResource targetResource;
        private IEnemyTarget targetEnemyTarget;
        private float lastCollectTime;
        private float idleTime;
        private Vector3? walkingTarget;
        private Color originalColor;
        private float radius;
        // private Vector3 position;
        private bool isFirstActivation = true;

        public bool IsEnemy => isEnemy;

        private void Awake()
        {
            // position = transform.position;
            originalColor = hpPanel.color;
            UpdateHealthText();
        }

        public override void Tick()
        {
            if (!IsActivated || !IsAlive)
                return;

            if (!gameObject.activeInHierarchy && !isEnemy)
                return;

            if (!gameObject.activeInHierarchy && isEnemy && state == State.Move)
            {
                Collect();
                return;
            }

            if (isFirstActivation)
            {
                isFirstActivation = false;
                if (isEnemy)
                {
                    state = State.Walking;
                    idleTime = Random.Range(-5f, -3f);
                }
            }

            switch (state)
            {
                case State.Walking:
                    Idle();
                    break;
                case State.Search:
                    Search();
                    break;
                case State.Move:
                    Move();
                    break;
                case State.Collect:
                    Collect();
                    break;
            }
            
            // transform.position = position;
        }

        private void Idle()
        {
            if (!walkingTarget.HasValue)
            {
                var bounds = GameManager.Instance.MapBounds;
                walkingTarget = new Vector3(Random.Range(bounds.min.x, bounds.max.x), transform.position.y, Random.Range(bounds.min.z, bounds.max.z));
            }
            
            var direction = walkingTarget.Value - transform.position;
            var distance = direction.magnitude;
            if (distance < 0.1f)
            {
                walkingTarget = null;
            }
            transform.position += direction.normalized * (speed * Time.deltaTime * 0.25f);

            idleTime += Time.deltaTime;
            if (idleTime >= 1f)
            {
                state = State.Search;
                idleTime = 0f;
            }
        }

        private void Search()
        {
            if (isEnemy)
            {
                var nearestEnemyTarget = FindHighestPriorityRandomEnemyTarget();
                if (nearestEnemyTarget == null)
                {
                    state = State.Walking;
                    animancer.Play(move);
                    return;
                }

                state = State.Move;
                animancer.Play(move);
                targetEnemyTarget = nearestEnemyTarget.GetComponent<IEnemyTarget>();
            }
            else
            {
                var nearestEnemyTarget = FindHighestPriorityRandomEnemyTarget();
                if (nearestEnemyTarget != null)
                {
                    state = State.Move;
                    animancer.Play(move);
                    targetEnemyTarget = nearestEnemyTarget.GetComponent<IEnemyTarget>();
                    return;
                }
                var nearestResource = FindRandomNearestResource();
                if (nearestResource == null)
                {
                    state = State.Walking;
                    animancer.Play(move);
                    return;
                }

                state = State.Move;
                animancer.Play(move);
                targetResource = nearestResource.GetComponent<IResource>();
            }
        }

        private void Move()
        {
            Vector3 direction;
            float radius;
            if (isEnemy)
            {
                if (targetEnemyTarget == null || !targetEnemyTarget.IsAlive)
                {
                    state = State.Walking;
                    animancer.Play(move);
                    return;
                }
                radius = targetEnemyTarget.Radius;
                direction = targetEnemyTarget.Transform.position - transform.position;
            }
            else
            {
                if (targetEnemyTarget != null)
                {
                    if (!targetEnemyTarget.IsAlive)
                    {
                        targetEnemyTarget = null;
                        state = State.Walking;
                        return;
                    }
                    radius = targetEnemyTarget.Radius;
                    direction = targetEnemyTarget.Transform.position - transform.position;
                }
                else if (targetResource == null)
                {
                    state = State.Walking;
                    animancer.Play(move);
                    return;
                }
                else
                {
                    radius = targetResource.Radius;
                    direction = targetResource.Transform.position - transform.position;
                }
            }
            var distance = direction.magnitude - radius;
            if (distance < collectDistance)
            {
                state = State.Collect;
                animancer.Play(collect);
                return;
            }

            transform.position += direction.normalized * (speed * Time.deltaTime);
        }

        private void Collect()
        {
            if (isEnemy)
            {
                if (targetEnemyTarget == null || !targetEnemyTarget.IsAlive)
                {
                    targetEnemyTarget = null;
                    state = State.Walking;
                    animancer.Play(move);
                    return;
                }

                if (Time.time - lastCollectTime < collectTime)
                {
                    return;
                }
                
                var newTarget = FindHighestPriorityRandomEnemyTarget();
                if (newTarget != null && newTarget.GetComponent<IEnemyTarget>().Priority > targetEnemyTarget.Priority)
                {
                    state = State.Walking;
                    animancer.Play(move);
                    return;
                }
                lastCollectTime = Time.time;
                targetEnemyTarget.Damage(damage);
            }
            else
            {
                if (targetEnemyTarget is { IsAlive: true } && targetEnemyTarget.Transform.gameObject.activeSelf)
                {
                    if (Time.time - lastCollectTime < collectTime)
                    {
                        return;
                    }

                    lastCollectTime = Time.time;
                    targetEnemyTarget.Damage(damage);
                    return;
                }

                if (targetResource == null || !targetResource.Transform.gameObject.activeSelf)
                {
                    state = State.Walking;
                    animancer.Play(move);
                    return;
                }

                if (Time.time - lastCollectTime < collectTime)
                {
                    return;
                }

                lastCollectTime = Time.time;
                targetResource.Collect(1);
            }
        }

        private GameObject FindNearestResource()
        {
            var nearestResource = default(GameObject);
            var nearestDistance = float.MaxValue;
            foreach (var resource in GameManager.Instance.Resources)
            {
                if (!resource.Transform.gameObject.activeSelf)
                    continue;

                var distance = Vector3.Distance(transform.position, resource.Transform.position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestResource = resource.Transform.gameObject;
                }
            }

            return nearestResource;
        }
        
        private GameObject FindRandomResource()
        {
            var activeResources = GameManager.Instance.Resources.FindAll(r => r.Transform.gameObject.activeSelf);
            if (activeResources.Count == 0)
                return null;

            return activeResources[Random.Range(0, activeResources.Count)].Transform.gameObject;
        }

        private GameObject FindRandomNearestResource()
        {
            var activeResources = GameManager.Instance.Resources.FindAll(r => r.Transform.gameObject.activeSelf);
            if (activeResources.Count == 0)
                return null;

            var nearestResources = new List<(GameObject, float)>();
            foreach (var activeResource in activeResources)
            {
                var distance = Vector3.Distance(transform.position, activeResource.Transform.position);
                nearestResources.Add((activeResource.Transform.gameObject, distance));
            }
            
            nearestResources.Sort((a, b) => a.Item2.CompareTo(b.Item2));
            var randomIndex = Random.Range(0, Mathf.Min(3, nearestResources.Count));
            return nearestResources[randomIndex].Item1;
        }
        
        private GameObject FindHighestPriorityRandomEnemyTarget()
        {
            var activeEnemyTargets = GameManager.Instance.EnemyTarget.Where(e =>
            {
                if (e is Unit unit)
                    return unit.IsEnemy && unit.gameObject.activeInHierarchy && unit.IsEnemy != isEnemy ||
                           !unit.IsEnemy && gameObject.activeInHierarchy && unit.IsEnemy != isEnemy;

                return isEnemy;
            }).ToList();
            if (activeEnemyTargets.Count == 0)
                return null;

            var highestPriorityTargets = new List<(GameObject, int)>();
            foreach (var activeEnemyTarget in activeEnemyTargets)
            {
                var priority = activeEnemyTarget.Priority;
                highestPriorityTargets.Add((activeEnemyTarget.Transform.gameObject, priority));
            }
            
            highestPriorityTargets.Sort((a, b) => b.Item2.CompareTo(a.Item2));
            var highestPriority = highestPriorityTargets[0].Item2;
            highestPriorityTargets = highestPriorityTargets.FindAll(e => e.Item2 == highestPriority);
            var randomIndex = Random.Range(0, Mathf.Min(3, highestPriorityTargets.Count));
            return highestPriorityTargets[randomIndex].Item1;
        }

        public enum State
        {
            Walking,
            Search,
            Move,
            Collect
        }

        public int Priority => 5;
        public Transform Transform => transform;
        public bool IsAlive => health > 0;
        public bool IsActivated { get; set; }

        public void Damage(int damage)
        {
            if (!IsAlive)
                return;

            hpPanel.DOKill();
            hpPanel.color = damageColor;
            hpPanel.DOColor(originalColor, 0.5f).SetEase(Ease.InSine);
            SoundController.Instance.PlaySound("hit", pitchRandomness: 0.1f);
            health -= damage;
            health = Math.Max(health, 0);
            UpdateHealthText();
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }

        public override void HideEntity()
        {
            if (!IsAlive)
                return;

            base.HideEntity();
        }

        public override void ShowEntity()
        {
            if (!IsAlive)
                return;

            base.ShowEntity();
        }

        private void UpdateHealthText()
        {
            healthText.text = health.ToString();
        }
    }
}