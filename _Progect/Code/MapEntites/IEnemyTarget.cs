using UnityEngine;

namespace Code.MapEntities
{
    public interface IEnemyTarget
    {
        int Priority { get; }
        Transform Transform { get; }
        bool IsAlive { get; }
        float Radius { get; }
        void Damage(int damage);
    }
}