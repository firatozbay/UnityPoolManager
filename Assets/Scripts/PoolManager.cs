using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PoolSystem
{
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
}