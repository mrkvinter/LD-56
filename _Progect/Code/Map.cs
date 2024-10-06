using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code
{
    public class Map : MonoBehaviour
    {
        [SerializeField] private List<MapCell> cells;
        [SerializeField] private Vector2Int startCellPosition;

        private MapCell currentCell;
        private bool isBusy;
        private bool isHidden;

        private void Start()
        {
            foreach (var cell in cells)
            {
                cell.gameObject.SetActive(true);
                cell.UI.SetSelection(false);
                cell.HideInstantly();
            }

            currentCell = GetCell(startCellPosition);
            GameManager.Instance.MapController.HideAsync().Forget();
            isHidden = true;
        }

        public void ShowStartCell()
        {
            ChangeAsync(currentCell).Forget();
        }

        private void Update()
        {
            if (isBusy || GameManager.Instance.GameCameraController.IsLookAtDeck || !GameManager.Instance.IsPlaying)
                return;

            if (Input.GetKeyDown(KeyCode.W))
            {
                Move(Vector2Int.down);
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Move(Vector2Int.up);
            }
            else if (Input.GetKeyDown(KeyCode.A))
            {
                Move(Vector2Int.left);
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                Move(Vector2Int.right);
            }
        }

        private void Move(Vector2Int direction)
        {
            var newCellPosition = currentCell.Position + direction;
            var newCell = GetCell(newCellPosition);
            if (newCell == null)
                return;

            ChangeAsync(newCell).Forget();
        }

        public void HideCells()
        {
            foreach (var cell in cells)
            {
                cell.Hide();
            }
        }

        private async UniTask ChangeAsync(MapCell newCell)
        {
            isBusy = true;
            currentCell.UI.SetSelection(false);
            newCell.UI.SetSelection(true);
            GameManager.Instance.HideUnits().Forget();
            currentCell.Hide();
            if (!isHidden)
            {
                await GameManager.Instance.MapController.HideAsync();
                await UniTask.Delay(100);
            }

            await GameManager.Instance.MapController.ShowAsync();
            isHidden = false;
            newCell.Show();
            currentCell = newCell;
            await GameManager.Instance.ShowUnits();
            isBusy = false;
        }
        
        private void HideScroll()
        {
            GameManager.Instance.MapController.HideAsync().Forget();
        }

        private MapCell GetCell(Vector2Int position)
        {
            return cells.Find(cell => cell.Position == position);
        }
    }
}