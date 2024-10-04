using UnityEngine;

namespace KvinterGames.Animations.TransformAnimations
{
    public abstract class BaseTransformAnimation : MonoBehaviour
    {
        protected Vector3 originalPosition;
        protected Quaternion originalRotation;
        protected Vector3 originalScale;
        
        protected virtual void Start()
        {
            originalPosition = transform.localPosition;
            originalRotation = transform.localRotation;
            originalScale = transform.localScale;
        }

        protected void Update()
        {
            ApplyTransform();
        }

        protected abstract void ApplyTransform();
    }
}