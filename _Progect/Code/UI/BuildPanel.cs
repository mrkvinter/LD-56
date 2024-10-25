using System;
using Code.MapEntities;
using KvinterGames;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class BuildPanel : MonoBehaviour
    {
        [SerializeField] private Transform buildPanel;
        [SerializeField] private BuildPrice treePriceText;
        [SerializeField] private BuildPrice rockPriceText;
        [SerializeField] private BuildPrice wheatPriceText;
        [SerializeField] private TMP_Text buildButtonText;
        [SerializeField] private Button buildButton;
        [SerializeField] private Transform notEnoughSpaceMessage;
        
        private bool isNeedSpace;
        public void Set(Price price, string title, bool needSpace, Action onCLick)
        {
            treePriceText.SetPrice(price.Tree, ResourceType.Tree);
            rockPriceText.SetPrice(price.Rock, ResourceType.Rock);
            wheatPriceText.SetPrice(price.Wheat, ResourceType.Wheat);
            buildButtonText.text = title;
            isNeedSpace = needSpace;
            buildButton.onClick.RemoveAllListeners();
            buildButton.onClick.AddListener(() =>
            {
                SoundController.Instance.PlaySound("click", pitchRandomness: 0.1f, volume: 0.75f);
                onCLick();
            });
            buildPanel.gameObject.SetActive(true);
            
            if (needSpace)
            {
                notEnoughSpaceMessage.gameObject.SetActive(GameManager.Instance.UnitCount >= GameManager.Instance.CreatureCapacity);
            }
            else
            {
                notEnoughSpaceMessage.gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            var enoughSpace = !isNeedSpace || GameManager.Instance.UnitCount < GameManager.Instance.CreatureCapacity;
            if (buildPanel.gameObject.activeSelf && buildButton != null)
            {
                if (treePriceText.IsEnough() && rockPriceText.IsEnough() && wheatPriceText.IsEnough() && enoughSpace)
                {
                    buildButton.interactable = true;
                }
                else
                {
                    buildButton.interactable = false;
                }
            }
            if (notEnoughSpaceMessage == null)
                return;

            if (isNeedSpace)
            {
                notEnoughSpaceMessage.gameObject.SetActive(GameManager.Instance.UnitCount >= GameManager.Instance.CreatureCapacity);
            }
            else
            {
                notEnoughSpaceMessage.gameObject.SetActive(false);
            }
        }

        public void Hide()
        {
            buildPanel.gameObject.SetActive(false);
        }
    }
}