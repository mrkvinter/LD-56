using System;
using System.Collections.Generic;
using System.Linq;
using Code.MapEntities;
using Code.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton

        private static GameManager instance;

        public static GameManager Instance => instance ??= CreateInstance();

        private static GameManager CreateInstance()
        {
            var gameObject = new GameObject("GameManager");
            return gameObject.AddComponent<GameManager>();
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        [SerializeField] private List<Unit> units;
        [SerializeField] private MapController mapController;

        [SerializeField] private BuildPanel buildPanel;
        [Space] [SerializeField] private MapCell treeCell;
        [SerializeField] private MapCell rockCell;

        private bool isTreeCell;

        public event System.Action<int> OnTreeCountChanged;
        public event System.Action<int> OnRockCountChanged;
        public event System.Action<int> OnWheatCountChanged;

        public BuildPanel BuildPanel => buildPanel;
        public int TreeCount { get; private set; } = 0;
        public int RockCount { get; private set; } = 0;
        public int WheatCount { get; private set; } = 0;

        public int AdditionalTreeCount => Buildings.Where(e => e.IsBuilt).Sum(e => e.TreeCount);
        public int AdditionalRockCount => Buildings.Where(e => e.IsBuilt).Sum(e => e.RockCount);
        public int AdditionalWheatCount => Buildings.Where(e => e.IsBuilt).Sum(e => e.WheatCount);

        public MapController MapController => mapController;

        public List<MapEntity> Entities { get; } = new();
        public List<IResource> Resources { get; } = new();
        public List<IEnemyTarget> EnemyTarget { get; } = new();
        public List<IBuilding> Buildings { get; } = new();

        private void Update()
        {
            foreach (var entity in Entities)
            {
                entity.Tick();
            }
        }

        public void AddTree(int count)
        {
            TreeCount += count;
            OnTreeCountChanged?.Invoke(TreeCount);
        }

        public void RegisterEntity(MapEntity entity)
        {
            Entities.Add(entity);
            
            switch (entity)
            {
                case IResource resource:
                    Resources.Add(resource);
                    break;
                case IEnemyTarget enemyTarget:
                    EnemyTarget.Add(enemyTarget);
                    break;
                case IBuilding building:
                    Buildings.Add(building);
                    break;
            }
        }

        public void AddRock(int count)
        {
            RockCount += count;
            OnRockCountChanged?.Invoke(RockCount);
        }
        
        public void AddWheat(int count)
        {
            WheatCount += count;
            OnWheatCountChanged?.Invoke(WheatCount);
        }

        [Button]
        public void Change()
        {
            ChangeAsync().Forget();
        }

        public async UniTask ChangeAsync()
        {
            if (isTreeCell)
            {
                HideUnits().Forget();
                treeCell.Hide();
                await mapController.HideAsync();
                await UniTask.Delay(200);
                await mapController.ShowAsync();
                rockCell.Show();
                ShowUnits().Forget();
                isTreeCell = false;
            }
            else
            {
                HideUnits().Forget();
                rockCell.Hide();
                await mapController.HideAsync();
                await UniTask.Delay(200);
                await mapController.ShowAsync();
                treeCell.Show();
                ShowUnits().Forget();
                isTreeCell = true;
            }
        }

        public async UniTask HideUnits()
        {
            foreach (var unit in units)
            {
                unit.transform.DOScale(Vector3.zero, 0.2f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => unit.gameObject.SetActive(false));
            }

            await UniTask.Delay(200);
        }

        public async UniTask ShowUnits()
        {
            foreach (var unit in units)
            {
                unit.gameObject.SetActive(true);
                unit.transform.DOScale(Vector3.one, 0.2f)
                    .SetEase(Ease.OutBack);
            }

            await UniTask.Delay(200);
        }
    }
}