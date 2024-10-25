using UnityEngine;

namespace Code.MapEntities
{
    public interface IResource
    {
        Transform Transform { get; }
        float Radius { get; }
        void Collect(int count);
    }
}