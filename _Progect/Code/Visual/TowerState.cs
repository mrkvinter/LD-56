using Code.MapEntities;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Code.Visual
{
    public class TowerState : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Sprite[] towerSprites;
        [SerializeField] private TowerEntity towerEntity;
        [SerializeField] private Button goToTowerButton;
        [SerializeField] private Transform nextWaveSlider;
        [SerializeField] private Transform message;
        [SerializeField] private TMP_Text messageText;

        private void Awake()
        {
            DOTween.Sequence()
                .Append(messageText.DOFade(0, 0.5f))
                .Append(messageText.DOFade(1, 0.5f))
                .SetLoops(-1, LoopType.Restart);
            
            goToTowerButton.onClick.AddListener(() =>
            {
                GameManager.Instance.Map.MoveToCell(new Vector2Int(1, 1));
            });
        }

        private void Update()
        {
            var maxHealth = towerEntity.MaxHealth;
            var currentHealth = towerEntity.Health;
            
            var towerState = (int) Mathf.Ceil((float) currentHealth / maxHealth * towerSprites.Length);
            towerState = Mathf.Clamp(towerState, 0, towerSprites.Length);
            image.sprite = towerSprites[^towerState];

            goToTowerButton.gameObject.SetActive(GameManager.Instance.HasActiveEnemy);
            message.gameObject.SetActive(GameManager.Instance.HasActiveEnemy);
            nextWaveSlider.gameObject.SetActive(!GameManager.Instance.HasActiveEnemy);
        }
    }
}