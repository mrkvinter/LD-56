using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

namespace Plugins.KvintessentialGames.Animations.TransformAnimations
{
    public class TextColorSineAnimation : BaseSineTransformAnimation
    {
        [Title("Text Color Sine Settings")]
        [SerializeField] private Color colorA;
        [SerializeField] private Color colorB;

        private Graphic graphics;
        
        protected override void Start()
        {
            base.Start();
            graphics = GetComponent<Graphic>();
        }

        protected override void ApplyTransform()
        {
            var sin = GetSinValue();
            graphics.color = Color.Lerp(colorA, colorB, sin);
        }
    }
}