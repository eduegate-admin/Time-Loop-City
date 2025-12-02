using UnityEngine;
using System.Collections.Generic;

namespace TimeLoopCity.Core
{
    /// <summary>
    /// Generic Object Pool for performance optimization.
    /// </summary>
    public class ObjectPool<T> where T : Component
    {
        private readonly T prefab;
        private readonly Transform parent;
        private readonly Queue<T> pool = new Queue<T>();

        public ObjectPool(T prefab, int initialSize, Transform parent = null)
        {
            this.prefab = prefab;
            this.parent = parent;

            for (int i = 0; i < initialSize; i++)
            {
                T obj = Object.Instantiate(prefab, parent);
                obj.gameObject.SetActive(false);
                pool.Enqueue(obj);
            }
        }

        public T Get()
        {
            T obj;
            if (pool.Count > 0)
            {
                obj = pool.Dequeue();
            }
            else
            {
                obj = Object.Instantiate(prefab, parent);
            }
            
            obj.gameObject.SetActive(true);
            return obj;
        }

        public void Return(T obj)
        {
            obj.gameObject.SetActive(false);
            pool.Enqueue(obj);
        }
    }
}
