using UnityEngine;

namespace Code.MapEntities
{
    public interface IResource
    {
        Transform Transform { get; }
        void Collect(int count);
    }
}