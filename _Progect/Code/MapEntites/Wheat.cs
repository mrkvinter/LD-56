namespace Code.MapEntities
{
    public class Wheat : ResourceEntity
    {
        protected override void AddResource(float count)
        {
            GameManager.Instance.AddWheat(count);
        }
    }
}