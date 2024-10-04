using KvinterGames.Animations.TransformAnimations;
using UnityEngine;

namespace Plugins.KvintessentialGames.Animations.TransformAnimations
{
    public class ShakingTransformAnimation : BaseTransformAnimation
    {
        [SerializeField] private float frequency;
        [SerializeField] private float duration = 1f;
        [SerializeField] private float strength = 2.5f;
        [SerializeField] private float angle = 5f;

        private float timer;
        private float durationTimer;
        
        private Vector3 shakePosition;
        private float shakeRotation;

        protected override void ApplyTransform()
        {
            timer += Time.deltaTime;
            
            if (timer > frequency)
            {
                timer = 0;
                durationTimer = duration;
                var vectorShift = Random.insideUnitCircle * (strength * 0.1f);
                shakePosition = vectorShift;
                shakeRotation = Random.Range(-angle, angle);
            }
            
            if (durationTimer > 0)
            {
                durationTimer -= Time.deltaTime;
            }
            else
            {
                shakePosition = Vector3.zero;
                shakeRotation = 0;
            }

            transform.localPosition = originalPosition + shakePosition;
            transform.localRotation = originalRotation * Quaternion.Euler(0, 0, shakeRotation);
        }
    }
}