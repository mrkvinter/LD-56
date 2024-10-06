using TMPro;
using UnityEngine;

namespace Code.UI
{
    public class CreaturesCountPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text creaturesCountText;

        private void Start()
        {
            GameManager.Instance.OnUnitCountChanged += OnUnitCountChanged;
            GameManager.Instance.OnCreatureCapacityChanged += OnUnitCountChanged;
            UpdateUI();
        }
        
        private void OnUnitCountChanged(int count)
        {
            UpdateUI();
        }
        
        private void UpdateUI()
        {
            creaturesCountText.text = $"{GameManager.Instance.UnitCount}/{GameManager.Instance.CreatureCapacity}";
        }
    }
}