using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace PoolSystem
{
    public class Pool
    {
        private Dictionary<int, Poolee> _activeObjects;
        private Dictionary<int, Poolee> _inActiveObjects;

        private readonly GameObject _prefab;
        private readonly PoolType _type;

        public Transform PoolTransform;

        public Pool(PoolType type, GameObject prefab, int count)
        {
            PoolTransform = (new GameObject(type.ToString())).transform;

            _inActiveObjects = new Dictionary<int, Poolee>();
            _activeObjects = new Dictionary<int, Poolee>();

            _prefab = prefab;
            _type = type;

            Poolee temp;
            for (int i = 0; i < count; i++)
            {
                temp = Create(_type, _prefab);
                temp.gameObject.SetActive(false);
                temp.transform.SetParent(PoolTransform);
                _inActiveObjects.Add(temp.GetInstanceID(), temp);
            }
        }

        private Poolee Create(PoolType type, GameObject prefab)
        {
            var go =
                GameObject.Instantiate(prefab, Vector3.zero, Quaternion.identity);
            var poolee = go.AddComponent<Poolee>();
            poolee.Init(type, poolee.GetInstanceID());
            return poolee;
        }

        public GameObject Spawn()
        {
            Poolee poolee;
            if (_inActiveObjects.Count > 0)
            {
                poolee = _inActiveObjects.First().Value;
                _inActiveObjects.Remove(poolee.Key);
            }
            else
            {
                poolee = Create(_type, _prefab);
            }
            _activeObjects.Add(poolee.Key, poolee);
            poolee.gameObject.SetActive(true);
            return poolee.gameObject;
        }

        public void Despawn(Poolee poolee)
        {
            if (!_activeObjects.ContainsKey(poolee.Key))
                return;
            _inActiveObjects.Add(poolee.Key, poolee);
            _activeObjects.Remove(poolee.Key);
            poolee.gameObject.SetActive(false);
        }
    }
}