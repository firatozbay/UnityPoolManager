using UnityEngine;
using UnityEngine.UI;
using PoolSystem;

// You don't need this class for PoolManager to work
// It's just an example for Spawning a Pooled Object
// Keep in mind you can only spawn Pooled Objects
public class SpawnButton : MonoBehaviour
{
    private Button _button;

    [SerializeField]
    private PoolType _type;

    private void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() => Spawn());
    }
    
    private void Spawn()
    {
        var go = PoolManager.Spawn(_type, Vector3.zero);

        //Very similar to Instantiate
        //var go2 = Instantiate(prefabName, Vector3.zero, Quaternion.identity);
        //Also supports different instantiation methods
        //PoolManager.Spawn(PoolType type, Vector3 position, Quaternion rotation)
        //PoolManager.Spawn(PoolType type, Transform parent)
    }

}
