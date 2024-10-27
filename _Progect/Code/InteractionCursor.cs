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
        private Texture2D transparentTexture;
        
        private void Awake()
        {
            Cursor.visible = false;
            rectTransform = GetComponent<RectTransform>();
            transparentTexture = CreateTransparentTexture(1, 1);
        }

        protected void LateUpdate()
        {
            var rayCamera = Camera.main;
            if (rayCamera == null) return;

            Cursor.visible = false;
            Cursor.SetCursor(transparentTexture, Vector2.zero, CursorMode.Auto);
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
        
        private Texture2D CreateTransparentTexture(int width, int height)
        {
            var texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            var pixels = new Color[width * height];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = new Color(0, 0, 0, 0); 
            }

            texture.SetPixels(pixels);
            texture.Apply();

            return texture;
        }

        public enum PointerType
        {
            Default,
            Up,
            Down
        }
    }
}