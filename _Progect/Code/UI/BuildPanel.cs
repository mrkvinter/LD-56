using System;
using Code.MapEntities;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.UI
{
    public class BuildPanel : MonoBehaviour
    {
        [SerializeField] private Transform buildPanel;
        [SerializeField] private TMP_Text treePriceText;
        [SerializeField] private TMP_Text rockPriceText;
        [SerializeField] private TMP_Text wheatPriceText;
        [SerializeField] private TMP_Text buildButtonText;
        [SerializeField] private Button buildButton;
        
        public void SetBuilding(IBuilding building, Action onCLick)
        {
            treePriceText.text = building.TreeCount.ToString();
            rockPriceText.text = building.RockCount.ToString();
            wheatPriceText.text = building.WheatCount.ToString();
            buildButtonText.text = building.IsBuilt ? "Upgrade" : "Build";
            buildButton.onClick.RemoveAllListeners();
            buildButton.onClick.AddListener(() =>
            {
                onCLick();
            });
            buildPanel.gameObject.SetActive(true);
        }
        
        public void Hide()
        {
            buildPanel.gameObject.SetActive(false);
        }
    }
}