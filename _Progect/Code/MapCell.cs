using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Code.MapEntities;
using Code.UI;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Code
{
    public class MapCell : MonoBehaviour
    {
        [SerializeField] private Vector2Int position;
        [SerializeField] private MapCellUI ui;
        [SerializeField] private List<MapEntity> entities;
        [SerializeField] private List<BuildingEntity> buildingsToBuild;
        
        [SerializeField] private Transform priceRoot;
        [SerializeField] private TMP_Text treeCountText;
        [SerializeField] private TMP_Text rockCountText;

        private List<IMapCellExtension> extensions;
        public Vector2Int Position => position;
        public MapCellUI UI => ui;
        public bool IsActivated { get; private set; }

        private void Awake()
        {
            CollectEntities();
            extensions = GetComponents<IMapCellExtension>().ToList();
        }

        private void Start()
        {
            foreach (var entity in entities)
            {
                entity.ParentCell = this;
            }
            
            UpdateBuildUI();
        }

        [Button]
        public void CollectEntities()
        {
            entities = new List<MapEntity>(GetComponentsInChildren<MapEntity>(true)
                .Where(e => e is not Unit));
        }

        public void Hide()
        {
            IsActivated = false;
            foreach (var entity in entities)
            {
                entity.HideEntity();
            }

            foreach (var extension in extensions)
            {
                extension.IsActivated = false;
                extension.OnHide();
            }

            UpdateBuildUI();
        }

        public void HideInstantly()
        {
            IsActivated = false;
            foreach (var entity in entities)
            {
                entity.HideEntityInstantly();
            }

            foreach (var extension in extensions)
            {
                extension.IsActivated = false;
                extension.OnHide();
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
            
            foreach (var extension in extensions)
            {
                extension.IsActivated = true;
                extension.OnShow();
            }

            UpdateBuildUI();
        }

        private void UpdateBuildUI()
        {
            if (IsActivated)
            {
                var building = buildingsToBuild.FirstOrDefault(e => !e.IsBuilt);
                if (building != null)
                {
                    GameManager.Instance.BuildPanel.Set(building.Price, $"Build {building.Name}", false, () =>
                    {
                        building.Build();
                        UpdateBuildUI();
                    });
                    
                    if (treeCountText != null)
                    {
                        treeCountText.text = building.Price.Tree.ToString();
                    }
                    
                    if (rockCountText != null)
                    {
                        rockCountText.text = building.Price.Rock.ToString();
                    }
                }
                else
                {
                    if (priceRoot != null)
                    {
                        priceRoot.gameObject.SetActive(false);
                    }
                    GameManager.Instance.BuildPanel.Hide();
                }
            }
            else
            {
                GameManager.Instance.BuildPanel.Hide();
            }
        }
    }
}