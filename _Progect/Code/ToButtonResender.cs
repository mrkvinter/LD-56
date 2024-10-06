using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Code
{
    public class ToButtonResender : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] public Button button;

        public void OnPointerClick(PointerEventData eventData)
        {
            button.OnPointerClick(eventData);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            button.OnPointerEnter(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            button.OnPointerExit(eventData);
        }
        
        public void OnPointerDown(PointerEventData eventData)
        {
            button.OnPointerDown(eventData);
        }
        
        public void OnPointerUp(PointerEventData eventData)
        {
            button.OnPointerUp(eventData);
        }
    }
}