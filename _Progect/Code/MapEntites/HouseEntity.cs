using UnityEngine;

namespace Code.MapEntities
{
    public class HouseEntity : BuildingEntity
    {
        [SerializeField] private int creatureCapacity;

        public override int CreatureCapacity => creatureCapacity;
    }
}