// The reason that OnSpawn and OnDespawn are not contained in the same interface
// is interface segregation, with this method you do not need to implement an empty
// method if you do not need OnSpawn
namespace PoolSystem
{
    // Not necessary to implement in every script in the pooled objects
    public interface ISpawnable
    {
        void OnSpawn();
    }
}