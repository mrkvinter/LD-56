using System;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Code.MapEntities
{
    public class MapEntity : MonoBehaviour
    {
        [SerializeField] private GameObject messageTextPrefab;
        [SerializeField] private Transform messageTextParent;
        
        public MapCell ParentCell { get; set; }
        
        private bool wasRegistered;

        protected virtual void Start()
        {
        }

        protected void OnEnable()
        {
            if (!wasRegistered)
            {
                GameManager.Instance.RegisterEntity(this);
                wasRegistered = true;
            }
        }

        private void OnDestroy()
        {
            if (wasRegistered)
            {
                GameManager.Instance.UnregisterEntity(this);
            }
        }

        protected void ShowMessage(string message)
        {
            var messageText = Instantiate(messageTextPrefab, messageTextParent).GetComponent<CanvasGroup>();
            var rectTransform = messageText.GetComponent<RectTransform>();
            var position = rectTransform.anchoredPosition;
            var text = messageText.GetComponentInChildren<TMP_Text>();
            messageText.DOFade(0, 2f).SetEase(Ease.InSine).OnComplete(() => Destroy(messageText.gameObject));
            rectTransform.DOAnchorPosY(position.y + 90, 2f);
            text.text = message;
        }

        public virtual void Tick()
        {
        }
        
        public virtual void ShowEntity()
        {
            gameObject.SetActive(true);
            transform.DOScale(Vector3.one, 0.15f).SetEase(Ease.OutBack);
        }
        
        public virtual void HideEntity()
        {
            transform.DOScale(Vector3.zero, 0.15f).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
        }
        
        public void HideEntityInstantly()
        {
            gameObject.SetActive(false);
            transform.localScale = Vector3.zero;
        }
    }
}