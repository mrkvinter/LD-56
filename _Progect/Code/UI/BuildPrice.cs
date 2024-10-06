using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class BuildPrice : MonoBehaviour
    {
        [SerializeField] private TMP_Text treePriceText;
        [SerializeField] private Color enoughColor;
        [SerializeField] private Color notEnoughColor;

        private ResourceType resourceType;
        
        public void SetPrice(int price, ResourceType resourceType)
        {
            gameObject.SetActive(price != 0);
            this.resourceType = resourceType;
            treePriceText.text = price.ToString();
        }

        private void Update()
        {
            if (resourceType == ResourceType.Tree)
            {
                treePriceText.color = GameManager.Instance.TreeCount >= int.Parse(treePriceText.text) ? enoughColor : notEnoughColor;
            }
            else if (resourceType == ResourceType.Rock)
            {
                treePriceText.color = GameManager.Instance.RockCount >= int.Parse(treePriceText.text) ? enoughColor : notEnoughColor;
            }
            else if (resourceType == ResourceType.Wheat)
            {
                treePriceText.color = GameManager.Instance.WheatCount >= int.Parse(treePriceText.text) ? enoughColor : notEnoughColor;
            }
        }
    }
}