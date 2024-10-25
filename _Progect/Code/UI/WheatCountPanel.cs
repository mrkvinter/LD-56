using System.Globalization;
using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class WheatCountPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text wheatCountText;
        [SerializeField] private TMP_Text additionalCountText;

        private void Start()
        {
            GameManager.Instance.OnWheatCountChanged += OnWheatCountChanged;
            UpdateUI();
        }

        private void OnWheatCountChanged(float count)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            wheatCountText.text = $"{GameManager.Instance.WheatCount:0.0}";
            if (GameManager.Instance.AdditionalWheatCount == 0)
            {
                additionalCountText.text = "";
            }
            else
            {
                additionalCountText.text = $"(+{GameManager.Instance.AdditionalWheatCount:0.0}s)";
            }
        }
    }
}