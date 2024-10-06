using System;
using UnityEngine;

namespace Code.MapEntities
{
    public abstract class BuildingEntity : MapEntity, IBuilding
    {
        [SerializeField] private Price price;

        public Transform Transform => transform;
        public Price Price => price;
        public bool IsBuilt { get; private set; }
        public virtual int TreeCount => 0;
        public virtual int RockCount => 0;
        public virtual int WheatCount => 0;

        public void Build()
        {
            if (GameManager.Instance.TreeCount < price.Tree || 
                GameManager.Instance.RockCount < price.Rock || 
                GameManager.Instance.WheatCount < price.Wheat)
            {
                return;
            }
            
            GameManager.Instance.AddTree(-price.Tree);
            GameManager.Instance.AddRock(-price.Rock);
            GameManager.Instance.AddWheat(-price.Wheat);

            IsBuilt = true;
            if (ParentCell.IsActivated)
            {
                ShowEntity();
            }
        }
    }
    
    [Serializable]
    public class Price
    {
        public int Tree;
        public int Rock;
        public int Wheat;
    }
}