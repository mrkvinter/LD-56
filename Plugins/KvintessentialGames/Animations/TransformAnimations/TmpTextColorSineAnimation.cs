using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Plugins.KvintessentialGames.Animations.TransformAnimations
{
    public class TmpTextColorSineAnimation : BaseSineTransformAnimation
    {
        [Title("Text Color Sine Settings")]
        [SerializeField] private Color colorA;
        [SerializeField] private Color colorB;

        private TMP_Text graphics;
        
        protected override void Start()
        {
            base.Start();
            graphics = GetComponent<TMP_Text>();
        }

        protected override void ApplyTransform()
        {
            var sin = GetSinValue();
            graphics.color = Color.Lerp(colorA, colorB, sin);
        }
    }
}