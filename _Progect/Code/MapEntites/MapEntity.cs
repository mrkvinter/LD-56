using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Code.MapEntities
{
    public class MessageData
    {
        public RectTransform RectTransform;
        public CanvasGroup CanvasGroup;
        public TMP_Text Text;
        public Sequence Sequence;
    }
    public class MapEntity : MonoBehaviour
    {
        [SerializeField] private GameObject messageTextPrefab;
        [SerializeField] private Transform messageTextParent;
        [SerializeField] private float radius;
        
        private MessageData messageData;
        public MapCell ParentCell { get; set; }
        public float Radius => radius;
        
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

        protected void ShowMessage(string message, float size)
        {
            if (messageData != null)
            {
                messageData.CanvasGroup.alpha = 1;
                messageData.RectTransform.localScale -= Vector3.one * .1f;
                messageData.Sequence.Kill();
                messageData.Text.text = message;
                messageData.Sequence = null;
                StartAnimation(size);
                return;
            }

            var messageText = Instantiate(messageTextPrefab, messageTextParent).GetComponent<CanvasGroup>();
            var rectTransform = messageText.GetComponent<RectTransform>();
            var text = messageText.GetComponentInChildren<TMP_Text>();
            text.text = message;
            
            messageData = new MessageData
            {
                RectTransform = rectTransform,
                CanvasGroup = messageText,
                Text = text,
            };
            
            StartAnimation(size);
        }

        private void StartAnimation(float size)
        {
            var sequence = DOTween.Sequence()
                .Insert(0, messageData.CanvasGroup.DOFade(0, 2f).SetEase(Ease.InSine))
                .Insert(0, messageData.RectTransform.DOScale(Vector3.one * size, .4f).SetEase(Ease.OutBack))
                .OnComplete(() =>
                {
                    Destroy(messageData.CanvasGroup.gameObject);
                    messageData = null;
                });

            messageData.Sequence = sequence;
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

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, radius);
        }
    }
}