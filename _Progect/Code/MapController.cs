using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Code
{
    public class MapController : MonoBehaviour
    {
        [SerializeField] private Transform scroll;
        [SerializeField] private Transform map;
        
        [SerializeField] private Vector3 openScrollPosition;
        [SerializeField] private Vector3 closeScrollPosition;
        [SerializeField] private float duration = 0.2f;
        [SerializeField] private float durationScale = 0.2f;
        [SerializeField] private float pauseTime = 1f;
        
        [SerializeField] private Transform[] mapElements;

        private float originalMapScaleX;

        private void Awake()
        {
            originalMapScaleX = map.localScale.x;
        }
        
        [Button]
        private void Play()
        {
            PlayAnimation().Forget();
        }

        public async UniTask HideAsync()
        {
            foreach (var mapElement in mapElements)
            {
                mapElement.DOScale(Vector3.zero, durationScale).SetEase(Ease.InBack);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(durationScale));
            scroll.DOLocalMove(closeScrollPosition, duration);
            map.DOScaleX(originalMapScaleX * 0.05f, duration);
            await UniTask.Delay(TimeSpan.FromSeconds(duration + pauseTime));
        }

        public async UniTask ShowAsync()
        {
            scroll.DOLocalMove(openScrollPosition, duration);
            map.DOScaleX(originalMapScaleX, duration);
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
            
            foreach (var mapElement in mapElements)
            {
                mapElement.DOScale(Vector3.one, durationScale).SetEase(Ease.OutBack);
            }
        }

        private async UniTask PlayAnimation()
        {
            foreach (var mapElement in mapElements)
            {
                mapElement.DOScale(Vector3.zero, durationScale).SetEase(Ease.InBack);
            }
            await UniTask.Delay(TimeSpan.FromSeconds(durationScale));
            scroll.DOLocalMove(closeScrollPosition, duration);
            map.DOScaleX(originalMapScaleX * 0.05f, duration);
            await UniTask.Delay(TimeSpan.FromSeconds(duration + pauseTime));
            scroll.DOLocalMove(openScrollPosition, duration);
            map.DOScaleX(originalMapScaleX, duration);
            await UniTask.Delay(TimeSpan.FromSeconds(duration));
            
            foreach (var mapElement in mapElements)
            {
                mapElement.DOScale(Vector3.one, durationScale).SetEase(Ease.OutBack);
            }
        }
    }
}