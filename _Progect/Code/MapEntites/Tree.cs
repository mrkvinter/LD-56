namespace Code.MapEntities
{
    public class Tree : ResourceEntity
    {
        protected override void AddResource(int count)
        {
            GameManager.Instance.AddTree(count);
        }
    }
}