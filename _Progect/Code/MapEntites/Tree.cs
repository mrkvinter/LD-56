namespace Code.MapEntities
{
    public class Tree : ResourceEntity
    {
        protected override void AddResource(float count)
        {
            GameManager.Instance.AddTree(count);
        }
    }
}