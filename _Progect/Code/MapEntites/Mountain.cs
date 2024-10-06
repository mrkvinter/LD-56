namespace Code.MapEntities
{
    public class Mountain : ResourceEntity
    {
        protected override void AddResource(int count)
        {
            GameManager.Instance.AddRock(count);
        }
    }
}