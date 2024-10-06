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

        protected virtual void Start()
        {
            GameManager.Instance.RegisterEntity(this);
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
        
        public void ShowEntity()
        {
            gameObject.SetActive(true);
            transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);
        }
        
        public void HideEntity()
        {
            transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() => gameObject.SetActive(false));
        }
        
        public void HideEntityInstantly()
        {
            gameObject.SetActive(false);
            transform.localScale = Vector3.zero;
        }
    }
}