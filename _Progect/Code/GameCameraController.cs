using System;
using System.Collections;
using UnityEngine;

namespace Code
{
    public class GameCameraController : MonoBehaviour
    {
        [SerializeField] private Transform root;
        [SerializeField] private float speed = 1f;
        [SerializeField] private CameraSettings deckSettings;
        [SerializeField] private CameraSettings pcSettings;

        private bool isLookAtDeck = true;
        private bool isBusy = false;

        private void Update()
        {
            if (isBusy)
                return;

            if (isLookAtDeck && Input.GetMouseButtonDown(0) && (Input.mousePosition.y > Screen.height * 0.9f))
            {
                isLookAtDeck = false;
                StartCoroutine(ChangeCameraSettings(pcSettings));
            }
            else if (!isLookAtDeck && Input.GetMouseButtonDown(0) && (Input.mousePosition.y < Screen.height * 0.1f))
            {
                isLookAtDeck = true;
                StartCoroutine(ChangeCameraSettings(deckSettings));
            }
        
        }
    
        private IEnumerator ChangeCameraSettings(CameraSettings settings)
        {
            isBusy = true;
            var startSettings = new CameraSettings
            {
                Position = root.transform.position,
                Rotation = root.transform.eulerAngles
            };
        
            var time = 0f;
            while (time < 1f)
            {
                time += Time.deltaTime * speed;
                root.transform.position = Vector3.Lerp(startSettings.Position, settings.Position, time);
                root.transform.eulerAngles = Vector3.Lerp(startSettings.Rotation, settings.Rotation, time);
                yield return null;
            }
        
            isBusy = false;
        }

        [Serializable]
        public struct CameraSettings
        {
            public Vector3 Position;
            public Vector3 Rotation;
        }
    }
}

