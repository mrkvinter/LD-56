using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Code.MapEntities
{
    public class WaveRootEntity : MapEntity
    {
        [SerializeField] private List<Unit> units;

        public bool IsActivated { get; private set; }
        private bool isShowed;

        protected void Awake()
        {
            CollectUnits();
            
            foreach (var unit in units)
            {
                unit.IsActivated = false;
            }
            
            HideUnits();
        }

        public void Activate()
        {
            IsActivated = true;
            foreach (var unit in units)
            {
                unit.IsActivated = true;
            }
            
            if (isShowed)
            {
                ShowUnits();
            }
        }

        public override void ShowEntity()
        {
            isShowed = true;
            gameObject.SetActive(true);
            gameObject.transform.localScale = Vector3.one;
            if (IsActivated)
            {
                ShowUnits();
            }
        }
        
        public override void HideEntity()
        {
            isShowed = false;
            HideUnits();
            UniTask.Delay(200).ContinueWith(() => gameObject.SetActive(false));
        }

        private void CollectUnits()
        {
            units = new List<Unit>(GetComponentsInChildren<Unit>(true));
        }
        
        private void ShowUnits()
        {
            foreach (var unit in units)
            {
                if (unit.IsAlive)
                    unit.ShowEntity();
            }
        }
        
        private void HideUnits()
        {
            foreach (var unit in units)
            {
                if (unit.IsAlive)
                    unit.HideEntity();
            }
        }
    }
}