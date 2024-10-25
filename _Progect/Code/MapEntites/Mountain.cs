namespace Code.MapEntities
{
    public class Mountain : ResourceEntity
    {
        protected override void AddResource(float count)
        {
            GameManager.Instance.AddRock(count);
        }
    }
}