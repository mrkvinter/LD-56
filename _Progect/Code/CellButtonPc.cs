using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code
{
    public class CellButtonPc : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] private Vector2Int cellPosition;
        [SerializeField] private Image image;
        [SerializeField] private Color enterColor;
        [SerializeField] private Color exitColor;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            GameManager.Instance.Map.MoveToCell(cellPosition);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            Animate(enterColor);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            Animate(exitColor);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
        }
        
        private void Animate(Color color)
        {
            image.DOKill();
            image.DOColor(color, 0.2f);
        }
    }
}