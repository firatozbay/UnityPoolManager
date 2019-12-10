using UnityEngine;

namespace PoolSystem
{
    public class Poolee : MonoBehaviour
    {
        public PoolType PoolType { get; private set; }
        public int Key { get; private set; }

        private ISpawnable[] _spawnables;
        private IDespawnable[] _despawnables;

        public void Init(PoolType poolType, int key)
        {
            PoolType = poolType;
            Key = key;
            _spawnables = GetComponentsInChildren<ISpawnable>();
            _despawnables = GetComponentsInChildren<IDespawnable>();
        }

        public void Spawn()
        {
            foreach (var spawnable in _spawnables)
            {
                spawnable.OnSpawn();
            }
            gameObject.SetActive(true);

        }

        public void Despawn()
        {
            foreach (var despawnable in _despawnables)
            {
                despawnable.OnDespawn();
            }
            gameObject.SetActive(false);
        }
    }
}