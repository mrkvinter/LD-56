using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

namespace Code.MapEntities
{
    public class ArcheryTowerEntity : BuildingEntity
    {
        [SerializeField] private float arrowTime;
        [SerializeField] private float arrowSpeed;
        [SerializeField] private Transform arrowSpawnPoint;
        [SerializeField] private int arrowDamage;
        [SerializeField] private GameObject arrowPrefab;

        private float timer;
        private Unit target;

        public override void Tick()
        {
            if (!IsBuilt)
                return;

            if (target == null || !target.IsAlive)
            {
                target = FindClosestEnemy();
            }
            timer += Time.deltaTime;
            if (timer >= 2)
            {
                timer = 0;
                if (target != null)
                {
                    Shoot(target).Forget();
                    target = null;
                }
            }
        }
        
        private async UniTask Shoot(Unit unit)
        {
            var arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, Quaternion.identity);
            arrow.transform.parent = arrowSpawnPoint;
            while (unit.IsAlive && Vector3.Distance(arrow.transform.position, unit.Transform.position) > 0.1f)
            {
                arrow.transform.position = Vector3.MoveTowards(arrow.transform.position, unit.Transform.position, arrowSpeed * Time.deltaTime);
                await UniTask.Yield();
            }
            if (unit != null && unit.IsAlive)
                unit.Damage(arrowDamage);

            Destroy(arrow);
        }
        
        private Unit FindClosestEnemy()
        {
            foreach (var entity in GameManager.Instance.Entities)
            {
                if (entity is Unit unit && unit.IsEnemy && unit.IsActivated && unit.IsAlive)
                {
                    return unit;
                }
            }
            
            return null;
        }
    }
}