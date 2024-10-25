using System;
using System.Collections.Generic;
using System.Linq;
using Code.MapEntities;
using Code.UI;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Game.Scripts;
using GameAnalyticsSDK;
using KvinterGames;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
            }
            else
            {
                Destroy(gameObject);
            }
        }

        #endregion

        [SerializeField] private MapController mapController;

        [SerializeField] private InteractionCursor interactionCursor;
        [SerializeField] private BuildPanel buildPanel;
        [SerializeField] private BuildPanel createCreaturePanel;
        [SerializeField] private BuildPanel createKnightCreaturePanel;
        [SerializeField] private BoxCollider mapCollider;
        [Space] 
        [SerializeField] private Transform mainMenu;

        [SerializeField] private Transform gameUI;
        [SerializeField] private Transform gameTower;
        [SerializeField] private Transform gameBuilding;
        [SerializeField] private Transform gameOver;
        [SerializeField] private Transform winMenu;
        
        [SerializeField] private Button startButton;
        [SerializeField] private Button restartButton;

        [SerializeField] private Map map;
        [SerializeField] private GameCameraController gameCameraController;
        [SerializeField] private Transform messageHasEnemy;
        
        [SerializeField] private TMP_Text debugText;

        private bool isTreeCell;

        public event Action<float> OnTreeCountChanged;
        public event Action<float> OnRockCountChanged;
        public event Action<float> OnWheatCountChanged;
        public event Action<int> OnUnitCountChanged;
        public event Action<int> OnCreatureCapacityChanged;

        public InteractionCursor InteractionCursor => interactionCursor;
        public Bounds MapBounds => mapCollider.bounds;
        public BuildPanel BuildPanel => buildPanel;
        public BuildPanel CreateCreaturePanel => createCreaturePanel;
        public BuildPanel CreateKnightCreaturePanel => createKnightCreaturePanel;
        public float TreeCount { get; private set; } = 0;
        public float RockCount { get; private set; } = 0;
        public float WheatCount { get; private set; } = 15;
        public float AllGameTreeCount { get; private set; } = 0;
        public float AllGameRockCount { get; private set; } = 0;
        public float AllGameWheatCount { get; private set; } = 15;
        public int CreatureCapacity => 3 + Buildings.Where(e => e.IsBuilt).Sum(e => e.CreatureCapacity);

        public float AdditionalTreeCount => Buildings.Where(e => e.IsBuilt).Sum(e => e.TreeCount / 2f);
        public float AdditionalRockCount => Buildings.Where(e => e.IsBuilt).Sum(e => e.RockCount / 2f);
        public float AdditionalWheatCount => Buildings.Where(e => e.IsBuilt).Sum(e => e.WheatCount / 2f);
        
        public bool HasActiveEnemy => Entities.OfType<Unit>().Any(e => e.IsEnemy && e.IsActivated);

        public MapController MapController => mapController;
        public Map Map => map;
        public GameCameraController GameCameraController => gameCameraController;

        public List<Unit> units { get; } = new();
        [ShowInInspector] public List<MapEntity> Entities { get; } = new();
        public List<IResource> Resources { get; } = new();
        public List<IEnemyTarget> EnemyTarget { get; } = new();
        public List<IBuilding> Buildings { get; } = new();
        public int UnitCount => units.Count;
        public bool IsCollectingStarted => (AllGameTreeCount - originalTreeCount) + (AllGameRockCount - originalRockCount) + (AllGameWheatCount - originalWheatCount) > 5;
        public bool IsPlaying { get; private set; }
        
        private SoundController.LoopSound music;
        
        private float previousTreeCount;
        private float previousRockCount;
        private float previousWheatCount;
        private float timer;
        
        private float originalTreeCount;
        private float originalRockCount;
        private float originalWheatCount;
        
        private float[] speedGame = {1, 2, 4};
        private int speedGameIndex = 0;

        private void Start()
        {
            GameAnalytics.Initialize();
            originalTreeCount = previousTreeCount = AllGameTreeCount = TreeCount;
            originalRockCount = previousRockCount = AllGameRockCount = RockCount;
            originalWheatCount = previousWheatCount = AllGameWheatCount = WheatCount;
            
            startButton.onClick.AddListener(StartGame);
            restartButton.onClick.AddListener(RestartGame);

            music = SoundController.Instance.PlayLoop("music");
            music.AudioSource.volume = 0.35f;
        }
        
        private void StartGame()
        {
            SoundController.Instance.PlaySound("click", pitchRandomness: 0.1f);
            mainMenu.gameObject.SetActive(false);
            gameUI.gameObject.SetActive(true);
            gameTower.gameObject.SetActive(true);
            gameBuilding.gameObject.SetActive(true);
            map.ShowStartCell();
            IsPlaying = true;
        }
        
        private void RestartGame() => UniTask.Create(async  () => 
        {
            music.AudioSource.DOFade(0, 0.5f);
            await UniTask.Delay(500);
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
        
        public void GameOver()
        {
            GameAnalytics.NewDesignEvent("GameOver");
            if (gameCameraController.IsLookAtDeck)
            {
                gameCameraController.LookAtPC();
            }
            map.HideCells();
            gameUI.gameObject.SetActive(false);
            gameTower.gameObject.SetActive(false);
            gameBuilding.gameObject.SetActive(false);
            gameOver.gameObject.SetActive(true);
            IsPlaying = false;
        }
        
        public void Win()
        {
            GameAnalytics.NewDesignEvent("Win");
            if (gameCameraController.IsLookAtDeck)
            {
                gameCameraController.LookAtPC();
            }
            map.HideCells();
            gameUI.gameObject.SetActive(false);
            gameTower.gameObject.SetActive(false);
            gameBuilding.gameObject.SetActive(false);
            winMenu.gameObject.SetActive(true);
            IsPlaying = false;
        }

        private void Update()
        {
            messageHasEnemy.gameObject.SetActive(HasActiveEnemy);
            foreach (var entity in Entities)
            {
                entity.Tick();
            }
            
            OnCreatureCapacityChanged?.Invoke(CreatureCapacity);
            
            if (Input.GetKeyDown(KeyCode.F2))
            {
                speedGameIndex = (speedGameIndex + 1) % speedGame.Length;
                Time.timeScale = speedGame[speedGameIndex];
            }

            var treeAtSecond = (TreeCount - previousTreeCount); 
            var rockAtSecond = (RockCount - previousRockCount);
            var wheatAtSecond = (WheatCount - previousWheatCount);
            var sb = new System.Text.StringBuilder();
            sb.AppendLine($"Speed: {Time.timeScale}x");
            sb.AppendLine($"Tree: {TreeCount} (+{treeAtSecond:F2}/s)");
            sb.AppendLine($"Rock: {RockCount} (+{rockAtSecond:F2}/s)");
            sb.AppendLine($"Wheat: {WheatCount} (+{wheatAtSecond:F2}/s)");
            debugText.text = sb.ToString();
            
            timer += Time.deltaTime;
            if (timer >= 1)
            {
                previousTreeCount = TreeCount;
                previousRockCount = RockCount;
                previousWheatCount = WheatCount;
                timer = 0;
            }
        }

        public void AddTree(float count)
        {
            TreeCount += count;
            if (count > 0)
            {
                AllGameTreeCount += count;
            }

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
                    if (enemyTarget is Unit { IsEnemy: false } unit)
                    {
                        units.Add(unit);
                        OnUnitCountChanged?.Invoke(units.Count);
                    }
                    break;
                case IBuilding building:
                    Buildings.Add(building);
                    if (building.CreatureCapacity > 0)
                    {
                        OnCreatureCapacityChanged?.Invoke(CreatureCapacity);
                    }
                    break;
            }
        }

        public void AddRock(float count)
        {
            RockCount += count;
            if (count > 0)
            {
                AllGameRockCount += count;
            }

            OnRockCountChanged?.Invoke(RockCount);
        }

        public void AddWheat(float count)
        {
            WheatCount += count;
            if (count > 0)
            {
                AllGameWheatCount += count;
            }

            OnWheatCountChanged?.Invoke(WheatCount);
        }

        public async UniTask HideUnits()
        {
            foreach (var unit in units)
            {
                unit.transform.DOScale(Vector3.zero, 0.15f)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => unit.gameObject.SetActive(false));
            }

            await UniTask.Delay(150);
        }

        public async UniTask ShowUnits()
        {
            foreach (var unit in units.ToList())
            {
                unit.gameObject.SetActive(true);
                unit.transform.DOScale(Vector3.one, 0.15f)
                    .SetEase(Ease.OutBack);
            }

            await UniTask.Delay(150);
        }

        public void UnregisterEntity(MapEntity mapEntity)
        {
            Entities.Remove(mapEntity);

            switch (mapEntity)
            {
                case IResource resource:
                    Resources.Remove(resource);
                    break;
                case IEnemyTarget enemyTarget:
                    EnemyTarget.Remove(enemyTarget);
                    if (enemyTarget is Unit { IsEnemy: false } unit)
                    {
                        units.Remove(unit);
                        OnUnitCountChanged?.Invoke(units.Count);
                    }
                    break;
                case IBuilding building:
                    Buildings.Remove(building);
                    if (building.CreatureCapacity > 0)
                    {
                        OnCreatureCapacityChanged?.Invoke(CreatureCapacity);
                    }
                    break;
            }
        }
    }
}