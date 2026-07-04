using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace Game.Core
{
    public class PoolManager
    {
        private readonly Dictionary<string, ObjectPool<GameObject>> _poolDict = new();
        private readonly Dictionary<GameObject, string> _instanceToKeyDict = new();
        private Transform _poolRoot;

        public T Get<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : Component
        {
            var gameObject = Get(prefab.gameObject, position, rotation, parent);
            return gameObject.GetComponent<T>();
        }

        public GameObject Get(GameObject prefab, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            var key = prefab.name;

            if (!_poolDict.ContainsKey(key))
            {
                _poolDict[key] = new ObjectPool<GameObject>(
                    createFunc: () => Object.Instantiate(prefab, GetPoolRoot()),
                    actionOnGet: instance =>
                    {
                        instance.SetActive(true);
                    },
                    actionOnRelease: instance =>
                    {
                        instance.SetActive(false);
                        instance.transform.SetParent(GetPoolRoot(), false);
                    },
                    actionOnDestroy: instance =>
                    {
                        _instanceToKeyDict.Remove(instance);
                        Object.Destroy(instance);
                    }
                );
            }

            var pooledInstance = _poolDict[key].Get();
            _instanceToKeyDict[pooledInstance] = key;

            if (parent != null)
            {
                pooledInstance.transform.SetParent(parent, true);
            }
            pooledInstance.transform.SetPositionAndRotation(position, rotation);

            return pooledInstance;
        }

        public void Release(GameObject instance)
        {
            if (!_instanceToKeyDict.TryGetValue(instance, out var key))
            {
                GameLogger.LogWarning($"[PoolManager] Unknown instance: {instance.name}");
                _instanceToKeyDict.Remove(instance);
                Object.Destroy(instance);
                return;
            }

            if (_poolDict.TryGetValue(key, out var pool))
            {
                pool.Release(instance);
            }
            else
            {
                GameLogger.LogWarning($"[PoolManager] Pool not found for key: {key}");
                _instanceToKeyDict.Remove(instance);
                Object.Destroy(instance);
            }
        }

        private Transform GetPoolRoot()
        {
            if (_poolRoot == null)
            {
                _poolRoot = new GameObject("PoolRoot").transform;
            }

            return _poolRoot;
        }
    }
}
