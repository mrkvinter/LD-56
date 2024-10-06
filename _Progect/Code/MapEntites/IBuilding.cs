using UnityEngine;

namespace Code.MapEntities
{
    public interface IBuilding
    {
        Transform Transform { get; }
        Price Price { get; }
        bool IsBuilt { get; }
        int TreeCount { get; }
        int RockCount { get; }
        int WheatCount { get; }
        
        void Build();
    }
}