using System;
using System.Collections.Generic;
using Code.MapEntities;
using DG.Tweening;
using GameAnalyticsSDK;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code
{
    public interface IMapCellExtension
    {
        bool IsActivated { get; set; }
        void OnShow();
        void OnHide();
    }

    public class TowerCell : MonoBehaviour, IMapCellExtension
    {
        [SerializeField] private List<Wave> waves;
        [SerializeField] private Slider waveSlider;
        [SerializeField] private Slider waveSlider2;
        [SerializeField] private Price creaturePrice;
        [SerializeField] private Price knightPrice;
        [SerializeField] private Transform creaturesRoot;
        [SerializeField] private Unit unitPrefab;
        [SerializeField] private Unit knightPrefab;
        [SerializeField] private TMP_Text waveNumberText;
        [SerializeField] private TMP_Text waveNumberText2;

        private int currentWaveIndex;
        private float waveTimer;
        
        private bool isResting;
        private float restTimer;

        public bool IsActivated { get; set; }

        private void Update()
        {
            if (!GameManager.Instance.IsPlaying)
                return;

            if (currentWaveIndex >= waves.Count) // All waves are done
            {
                if (!GameManager.Instance.HasActiveEnemy)
                {
                    GameManager.Instance.Win();
                }
                return;
            }
            
            if (isResting && !GameManager.Instance.HasActiveEnemy)
            {
                restTimer += Time.deltaTime;
                if (restTimer >= 5)
                {
                    isResting = false;
                    restTimer = 0;
                }
            }
            
            if (GameManager.Instance.IsCollectingStarted && !isResting)
                waveTimer += Time.deltaTime;
    
            if (waveTimer >= waves[currentWaveIndex].delay)
            {
                waveTimer = 0;
                waves[currentWaveIndex].waveRoot.Activate();
                isResting = true;
                // foreach (var unit in waves[currentWaveIndex].units)
                // {
                //     unit.gameObject.SetActive(true);
                //     unit.transform.DOScale(Vector3.one, 0.2f)
                //         .SetEase(Ease.OutBack);
                // }

                currentWaveIndex++;
                GameAnalytics.NewDesignEvent($"Game:Complete:Wave_{currentWaveIndex}");
            }

            waveSlider.value = waveTimer / waves[currentWaveIndex].delay;
            waveSlider2.value = waveTimer / waves[currentWaveIndex].delay;

            UpdateUI();
        }

        [Serializable]
        private struct Wave
        {
            public float delay;
            public WaveRootEntity waveRoot;
        }

        public void OnShow()
        {
            UpdateUI();
        }

        public void OnHide()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            if (currentWaveIndex >= waves.Count)
            {
                waveNumberText.text = "Last wave";
                waveNumberText2.text = "Last wave";
            }
            else
            {
                waveNumberText.text = $"{currentWaveIndex + 1}/{waves.Count}";
                waveNumberText2.text = $"{currentWaveIndex + 1}/{waves.Count}";
            }

            GameManager.Instance.CreateCreaturePanel.Set(creaturePrice, "Create creature", true, CreateUnit);
            GameManager.Instance.CreateKnightCreaturePanel.Set(knightPrice, "Create knight", true, CreateKnight);
        }

        private void CreateUnit()
        {
            if (GameManager.Instance.WheatCount < creaturePrice.Wheat ||
                GameManager.Instance.RockCount < creaturePrice.Rock ||
                GameManager.Instance.TreeCount < creaturePrice.Tree ||
                GameManager.Instance.UnitCount >= GameManager.Instance.CreatureCapacity)
            {
                return;
            }

            GameManager.Instance.AddWheat(-creaturePrice.Wheat);
            GameManager.Instance.AddRock(-creaturePrice.Rock);
            GameManager.Instance.AddTree(-creaturePrice.Tree);
            var unit = Instantiate(unitPrefab, creaturesRoot);
            unit.IsActivated = true;
            unit.transform.localPosition = Vector3.zero;
            unit.transform.localScale = Vector3.zero;
            unit.transform.DOScale(Vector3.one, 0.2f)
                .SetEase(Ease.OutBack);
        }
        
        private void CreateKnight()
        {
            if (GameManager.Instance.WheatCount < knightPrice.Wheat ||
                GameManager.Instance.RockCount < knightPrice.Rock ||
                GameManager.Instance.TreeCount < knightPrice.Tree ||
                GameManager.Instance.UnitCount >= GameManager.Instance.CreatureCapacity)
            {
                return;
            }

            GameManager.Instance.AddWheat(-knightPrice.Wheat);
            GameManager.Instance.AddRock(-knightPrice.Rock);
            GameManager.Instance.AddTree(-knightPrice.Tree);
            
            var unit = Instantiate(knightPrefab, creaturesRoot);
            unit.IsActivated = true;
            unit.transform.localPosition = Vector3.zero;
            unit.transform.localScale = Vector3.zero;
            unit.transform.DOScale(Vector3.one, 0.2f)
                .SetEase(Ease.OutBack);
        }
    }
}