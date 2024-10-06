namespace Code.MapEntities
{
    public class Wheat : ResourceEntity
    {
        protected override void AddResource(int count)
        {
            GameManager.Instance.AddWheat(count);
        }
    }
}