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

        public Transform Transform => transform;

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Time.time - lastClickTime < cooldown)
                return;

            lastClickTime = Time.time;
            Add(1);
        }

        private void Add(int count)
        {
            AddResource(count);
            ShowMessage($"+{count}");

            if (!animTask.IsActive())
                animTask = visual.DOPunchScale(Vector3.one * 0.05f, 0.5f);
        }

        public void Collect(int count)
        {
            SoundController.Instance.PlaySound("click", pitchRandomness: 0.2f, volume: 0.75f);
            Add(count);
        }
        
        protected abstract void AddResource(int count);
    }
}