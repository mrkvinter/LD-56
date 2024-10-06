using System;
using System.Collections;
using Game.Scripts;
using UnityEngine;

namespace Code
{
    public class GameCameraController : MonoBehaviour
    {
        [SerializeField] private Transform root;
        [SerializeField] private float speed = 1f;
        [SerializeField] private CameraSettings deckSettings;
        [SerializeField] private CameraSettings pcSettings;
        
        [SerializeField] private GameObject lookAtDeckTip;
        [SerializeField] private GameObject lookAtPCTip;

        private bool isLookAtDeck = false;
        private bool isBusy = false;
        
        public bool IsLookAtDeck => isLookAtDeck;

        private void Update()
        {
            GameManager.Instance.InteractionCursor.SetPointer(InteractionCursor.PointerType.Default);
            lookAtDeckTip.SetActive(false);
            lookAtPCTip.SetActive(false);

            if (isBusy || !GameManager.Instance.IsPlaying)
                return;

            lookAtDeckTip.SetActive(!isLookAtDeck);
            lookAtPCTip.SetActive(isLookAtDeck);

            if (isLookAtDeck && (Input.mousePosition.y > Screen.height * 0.9f))
            {
                GameManager.Instance.InteractionCursor.SetPointer(InteractionCursor.PointerType.Up);
                if (Input.GetMouseButtonDown(0))
                {
                    isLookAtDeck = false;
                    StartCoroutine(ChangeCameraSettings(pcSettings));
                }
            }
            else if (!isLookAtDeck && (Input.mousePosition.y < Screen.height * 0.1f))
            {
                GameManager.Instance.InteractionCursor.SetPointer(InteractionCursor.PointerType.Down);
                if (Input.GetMouseButtonDown(0))
                {
                    isLookAtDeck = true;
                    StartCoroutine(ChangeCameraSettings(deckSettings));
                }
            }
        }
    
        public void LookAtDeck()
        {
            isLookAtDeck = true;
            StartCoroutine(ChangeCameraSettings(deckSettings));
        }
        
        public void LookAtPC()
        {
            isLookAtDeck = false;
            StartCoroutine(ChangeCameraSettings(pcSettings));
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

