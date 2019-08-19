using UnityEngine;

public class Poolee : MonoBehaviour
{

    public PoolType PoolType { get; private set; }
    public int Key { get; private set; }

    public void Init(PoolType poolType, int key)
    {
        PoolType = poolType;
        Key = key;
    }
}
