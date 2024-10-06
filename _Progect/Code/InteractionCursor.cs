using UnityEngine;

namespace Game.Scripts
{
    public class InteractionCursor : MonoBehaviour
    {
        [SerializeField] private GameObject @default;
        [SerializeField] private GameObject up;
        [SerializeField] private GameObject down;
        
        private PointerType pointerType;
        private RectTransform rectTransform;
        
        private void Awake()
        {
            Cursor.visible = false;
            rectTransform = GetComponent<RectTransform>();
        }

        protected void LateUpdate()
        {
            var rayCamera = Camera.main;
            if (rayCamera == null) return;

            Cursor.visible = false;
            var screenPosition = Input.mousePosition;
            rectTransform.position = screenPosition;
        }

        public void SetPointer(PointerType pointerType)
        {
            this.pointerType = pointerType;
            UpdatePointer();
        }

        private void UpdatePointer()
        {
            @default.SetActive(pointerType == PointerType.Default);
            up.SetActive(pointerType == PointerType.Up);
            down.SetActive(pointerType == PointerType.Down);
        }

        public enum PointerType
        {
            Default,
            Up,
            Down
        }
    }
}