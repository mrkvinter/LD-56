using System;
using System.Collections.Generic;
using Code.MapEntities;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Code
{
    public class TowerCell : MonoBehaviour
    {
        [SerializeField] private List<Wave> waves;
        [SerializeField] private Slider waveSlider;
        
        private int currentWaveIndex;
        private float waveTimer;
        
        private void Update()
        {
            waveTimer += Time.deltaTime;
            if (waveTimer >= waves[currentWaveIndex].delay)
            {
                waveTimer = 0;
                foreach (var unit in waves[currentWaveIndex].units)
                {
                    unit.gameObject.SetActive(true);
                    unit.transform.DOScale(Vector3.one, 0.2f)
                        .SetEase(Ease.OutBack);
                }
                currentWaveIndex++;
            }
            
            waveSlider.value = waveTimer / waves[currentWaveIndex].delay;
        }
        
        [Serializable]
        private struct Wave
        {
            public float delay;
            public List<Unit> units;
        }
    }
}