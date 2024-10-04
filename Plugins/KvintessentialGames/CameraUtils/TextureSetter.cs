using UnityEngine;
using UnityEngine.UI;

namespace KvinterGames.CameraUtils
{
    [ExecuteAlways]
    public class TextureSetter : MonoBehaviour
    {
        public RawImage targetImage;
        public RawImage renderTexture;

        private void Start()
        {
            renderTexture.texture = targetImage.texture;
        }
    }
}