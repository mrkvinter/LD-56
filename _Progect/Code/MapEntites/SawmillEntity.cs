using UnityEngine;

namespace Code.MapEntities
{
    public class SawmillEntity : BuildingEntity
    {
        [SerializeField] private int woodCount;

        public override int TreeCount => woodCount;

        private float timer;

        public override void Tick()
        {
            if (!IsBuilt)
                return;

            timer += Time.deltaTime;
            if (timer >= 2)
            {
                timer = 0;
                GameManager.Instance.AddTree(woodCount);
                ShowMessage($"+{woodCount}");
            }
        }
    }
}