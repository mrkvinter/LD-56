using UnityEngine;
using UnityEngine.UI;

namespace KvinterGames.CameraUtils
{
    [ExecuteAlways]
    public class TextureCameraSetter : MonoBehaviour
    {
        public Camera textureCamera;
        public RawImage renderTexture;
        public RawImage[] additionalRenderTexture;

        private void Start()
        {
            var rectTransform = renderTexture.rectTransform.rect;
            var texture = new RenderTexture((int)rectTransform.width, (int)rectTransform.height, 16, UnityEngine.Experimental.Rendering.DefaultFormat.HDR);
            texture.filterMode = FilterMode.Point;
            texture.useMipMap = false;
            texture.Create();

            if (textureCamera==null)
            {
                return;
            }

            textureCamera.targetTexture = texture;
            renderTexture.texture = textureCamera.targetTexture;
            if (additionalRenderTexture != null)
            {
                foreach (var e in additionalRenderTexture)
                {
                    e.texture = textureCamera.targetTexture;
                }
            }
        }
    }
}