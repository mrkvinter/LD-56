using UnityEngine;

namespace Code.UI
{
    public class MapCellUI : MonoBehaviour
    {
        [SerializeField] private Transform selection;
        
        public void SetSelection(bool value)
        {
            selection.gameObject.SetActive(value);
        }
    }
}