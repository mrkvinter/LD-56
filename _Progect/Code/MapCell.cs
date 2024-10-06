using System.Collections.Generic;
using System.Linq;
using Code.MapEntities;
using Code.UI;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code
{
    public class MapCell : MonoBehaviour
    {
        [SerializeField] private Vector2Int position;
        [SerializeField] private MapCellUI ui;
        [SerializeField] private List<MapEntity> entities;
        [SerializeField] private List<BuildingEntity> buildingsToBuild;

        public Vector2Int Position => position;
        public MapCellUI UI => ui;
        public bool IsActivated { get; private set; }

        private void Start()
        {
            CollectEntities();
            foreach (var entity in entities)
            {
                entity.ParentCell = this;
            }
        }

        [Button]
        public void CollectEntities()
        {
            entities = new List<MapEntity>(GetComponentsInChildren<MapEntity>(true));
        }

        public void Hide()
        {
            IsActivated = false;
            foreach (var entity in entities)
            {
                entity.HideEntity();
            }
            
            GameManager.Instance.BuildPanel.Hide();
        }
        
        public void HideInstantly()
        {
            IsActivated = false;
            foreach (var entity in entities)
            {
                entity.HideEntityInstantly();
            }
        }

        public void Show()
        {
            IsActivated = true;
            foreach (var entity in entities)
            {
                if (entity is IBuilding { IsBuilt: false })
                    continue;

                entity.ShowEntity();
            }
            
            var building = buildingsToBuild.FirstOrDefault(e => !e.IsBuilt);
            if (building != null)
            {
                GameManager.Instance.BuildPanel.SetBuilding(building, building.Build);
            }
        }
    }
}