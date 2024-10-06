using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class RockCountPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text rockCountText;
        [SerializeField] private TMP_Text additionalCountText;

        private void Start()
        {
            GameManager.Instance.OnRockCountChanged += OnRockCountChanged;
            UpdateUI();
        }

        private void OnRockCountChanged(int count)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            rockCountText.text = GameManager.Instance.RockCount.ToString();
            if (GameManager.Instance.AdditionalRockCount == 0)
            {
                additionalCountText.text = "";
            }
            else
            {
                additionalCountText.text = $"(+{GameManager.Instance.AdditionalRockCount}s)";
            }
        }
    }
}