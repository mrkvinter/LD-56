using UnityEngine;

namespace Code.MapEntities
{
    public interface IEnemyTarget
    {
        int Priority { get; }
        Transform Transform { get; }
        bool IsAlive { get; }
        void Damage(int damage);
    }
}