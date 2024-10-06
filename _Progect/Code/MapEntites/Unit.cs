using System.Collections.Generic;
using UnityEngine;

namespace Code.MapEntities
{
    public class Unit : MapEntity
    {
        [SerializeField] private float speed = 1f;
        [SerializeField] private float collectTime = 1f;
        [SerializeField] private float collectDistance = 0.1f;
        [SerializeField] private bool isEnemy;

        private State state = State.Search;
        private IResource targetResource;
        private IEnemyTarget targetEnemyTarget;
        private float lastCollectTime;
        private float idleTime;

        public override void Tick()
        {
            if (!gameObject.activeInHierarchy && !isEnemy)
                return;

            switch (state)
            {
                case State.Idle:
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
        }

        private void Idle()
        {
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
                    return;

                state = State.Move;
                targetEnemyTarget = nearestEnemyTarget.GetComponent<IEnemyTarget>();
            }
            else
            {
                var nearestResource = FindRandomNearestResource();
                if (nearestResource == null)
                    return;

                state = State.Move;
                targetResource = nearestResource.GetComponent<IResource>();
            }
        }

        private void Move()
        {
            var direction = (!isEnemy ? targetResource.Transform : targetEnemyTarget.Transform).position - transform.position;
            var distance = direction.magnitude;
            if (distance < collectDistance)
            {
                state = State.Collect;
                return;
            }

            transform.position += direction.normalized * (speed * Time.deltaTime);
        }

        private void Collect()
        {
            if (isEnemy)
            {
                if (Time.time - lastCollectTime < collectTime)
                {
                    return;
                }
                
                lastCollectTime = Time.time;
                targetEnemyTarget.Damage(1);
            }
            else
            {
                if (!targetResource.Transform.gameObject.activeSelf)
                {
                    state = State.Idle;
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
            var activeEnemyTargets = GameManager.Instance.EnemyTarget.FindAll(r => r.Transform.gameObject.activeSelf);
            if (activeEnemyTargets.Count == 0)
                return null;

            var highestPriorityTargets = new List<(GameObject, int)>();
            foreach (var activeEnemyTarget in activeEnemyTargets)
            {
                var priority = activeEnemyTarget.Priority;
                highestPriorityTargets.Add((activeEnemyTarget.Transform.gameObject, priority));
            }
            
            highestPriorityTargets.Sort((a, b) => b.Item2.CompareTo(a.Item2));
            var randomIndex = Random.Range(0, Mathf.Min(3, highestPriorityTargets.Count));
            return highestPriorityTargets[randomIndex].Item1;
        }

        public enum State
        {
            Idle,
            Search,
            Move,
            Collect
        }
    }
}