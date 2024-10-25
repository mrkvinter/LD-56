using DG.Tweening;
using KvinterGames;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Code.MapEntities
{
    public abstract class ResourceEntity : MapEntity, IResource, IPointerClickHandler
    {
        [SerializeField] private float cooldown = 1f;
        [SerializeField] private Transform visual;

        private float lastClickTime;
        private Tweener animTask;
        private float lastTime;
        private float collected;
        // сколько ресурсов собирается за 1 секунду
        private float collectRate = 1f;

        public Transform Transform => transform;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Time.time - lastClickTime < cooldown)
                return;

            lastClickTime = Time.time;
            Add(0.15f);
        }

        private void Add(float count)
        {
            if (Time.time - lastTime > 1)
            {
                collected = 0;
            }

            lastTime = Time.time;
            collected += count;
            AddResource(count);
            ShowMessage($"+{collected:0.0}", 1 + 1f * (collected / 10f)); 

            if (!animTask.IsActive())
                animTask = visual.DOPunchScale(Vector3.one * 0.05f, 0.5f);
        }

        public void Collect(int count)
        {
            SoundController.Instance.PlaySound("click", pitchRandomness: 0.2f, volume: 0.75f);
            Add(count);
        }
        
        protected abstract void AddResource(float count);
    }
}