using UnityEngine;

namespace Code.MapEntities
{
    public class MillEntity : BuildingEntity
    {
        [SerializeField] private int wheatCount;

        public override int WheatCount => wheatCount;

        private float timer;

        public override void Tick()
        {
            if (!IsBuilt)
                return;

            timer += Time.deltaTime;
            if (timer >= 1)
            {
                timer = 0;
                GameManager.Instance.AddWheat(wheatCount);
                ShowMessage($"+{wheatCount}");
            }
        }
    }
}