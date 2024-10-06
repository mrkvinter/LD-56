using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class TreeCountPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text treeCountText;
        [SerializeField] private TMP_Text additionalCountText;

        private void Start()
        {
            GameManager.Instance.OnTreeCountChanged += OnTreeCountChanged;
            UpdateUI();
        }

        private void OnTreeCountChanged(int count)
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
            treeCountText.text = GameManager.Instance.TreeCount.ToString();
            if (GameManager.Instance.AdditionalTreeCount == 0)
            {
                additionalCountText.text = "";
            }
            else
            {
                additionalCountText.text = $"(+{GameManager.Instance.AdditionalTreeCount}s)";
            }
        }
    }
}