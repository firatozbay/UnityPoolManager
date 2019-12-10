using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PoolSystem
{
    public enum PoolType
    {
        Example1 = 10,  // Rename them however you like
        Example2 = 20,
        Example3 = 30,
        // Add more enums as you desire
    }

    [System.Serializable]
    public struct PoolInfo
    {
        public PoolType PoolType;
        public GameObject Prefab;
        public int Count;
    }

    // Only concern here is the script execution order
    // PoolManager's Awake must execute before any Spawn
    // or Despawn calls otherwise it will not work.
    public class PoolManager : MonoBehaviour
    {
        #region Fields
        private static PoolManager _instance;

        [SerializeField]
        private PoolInfo[] _poolInfos;

        private Dictionary<PoolType, Pool> _poolDictionary;

        private Transform _poolContainer;
        #endregion

        #region Unity Messages
        private void Awake()
        {
            _instance = this;

            _poolDictionary = new Dictionary<PoolType, Pool>();

            _poolContainer = (new GameObject("Pools")).transform;

            foreach (var poolInfo in _poolInfos)
            {
                _poolDictionary[poolInfo.PoolType] =
                    new Pool(poolInfo.PoolType, poolInfo.Prefab, poolInfo.Count);
                _poolDictionary[poolInfo.PoolType].PoolTransform.SetParent(_poolContainer);
            }

        }
        #endregion

        #region Spawn
        public static GameObject Spawn(PoolType poolType, Vector3 position)
        {
            return Spawn(poolType, position, Quaternion.identity);
        }

        public static GameObject Spawn(
            PoolType poolName, Vector3 position, Quaternion rotation)
        {
            if (!_instance._poolDictionary.ContainsKey(poolName))
                return null;
            var temp = _instance._poolDictionary[poolName].Spawn();
            temp.transform.position = position;
            temp.transform.rotation = rotation;
            return temp;
        }

        public static GameObject Spawn(PoolType poolType, Transform parent)
        {
            var temp = Spawn(poolType, parent.position, parent.rotation);
            if (temp == null)
                return null;
            temp.transform.SetParent(parent);
            return temp;
        }
        #endregion

        #region Despawn
        public static void Despawn(GameObject go)
        {
            if (!go.GetComponent<Poolee>())
                return;
            Despawn(go.GetComponent<Poolee>());
        }

        public static void Despawn(GameObject go, float time)
        {
            if (!go.GetComponent<Poolee>())
                return;
            Despawn(go.GetComponent<Poolee>(), time);
        }

        private static void Despawn(Poolee poolee)
        {
            if (!_instance._poolDictionary.ContainsKey(poolee.PoolType))
                return;
            _instance._poolDictionary[poolee.PoolType].Despawn(poolee);
        }

        private static void Despawn(Poolee poolee, float time)
        {
            _instance.StartCoroutine(DespawnCoroutine(poolee, time));
        }

        private static IEnumerator DespawnCoroutine(
            Poolee poolee, float time)
        {
            yield return new WaitForSeconds(time);
            Despawn(poolee);
        }
        #endregion
    }

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