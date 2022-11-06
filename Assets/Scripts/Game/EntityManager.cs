using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EntityManager : NetworkSingleton<EntityManager>
{
    [SerializeField]
    GameObject _towerBasePrefab;

    [SerializeField]
    GameObject _enemyPrefab;

    List<GameObject> _enemies;
    List<GameObject> _bullets;
    List<GameObject> _towers;

    void Awake()
    {
        _enemies = new List<GameObject>();
        _bullets = new List<GameObject>();
        _towers = new List<GameObject>();
    }

    void Start()
    {
        // This class will manage the lifetime of tower, bullet and enemies.
        // When we need to spawn, disable or kill one of those, we must use this class
    }

    #region Enemies

    public void SpawnEnemy(EnemyData data)
    {
        GameObject enemy = Instantiate(_enemyPrefab, Vector3.zero, Quaternion.identity);
        enemy.GetComponent<EnemyBehaviour>().data = data;
        enemy.GetComponent<NetworkObject>().Spawn();
        _enemies.Add(enemy);
    }

    public void DestroyEnemy(GameObject enemy)
    {
        _enemies.Remove(enemy);
        enemy.GetComponent<NetworkObject>().Despawn();
    }

    public List<GameObject> GetEnemies()
    {
        return _enemies;
    }

    public bool AreAllEnemyDead()
    {
        return _enemies.Count == 0;
    }

    #endregion

    #region Bullets


    [ClientRpc]
    public void SpawnBulletClientRpc(BulletType bulletType, ulong ownerId, ulong targetId)
    {
        if (!IsHost)
        {
            SpawnBullet(bulletType, ownerId, targetId);
        }
    }

    public void SpawnBulletServer(BulletType bulletType, ulong ownerId, ulong targetId)
    {
        SpawnBullet(bulletType, ownerId, targetId);
        SpawnBulletClientRpc(bulletType, ownerId, targetId);
    }

    public void SpawnBullet(BulletType bulletType, ulong ownerId, ulong targetId)
    {
        GameObject bullet = null;
        var owner = NetworkManager.Singleton.SpawnManager.SpawnedObjects[ownerId];
        var target = NetworkManager.Singleton.SpawnManager.SpawnedObjects[targetId];

        if (owner != null && target != null)
        {
            bullet = Factory.instance.CreateBullet(bulletType);
            bullet.GetComponent<BulletBehaviour>().owner = owner.gameObject;
            bullet.GetComponent<BulletBehaviour>().target = target.gameObject;
            bullet.transform.position = owner.transform.position;

            _bullets.Add(bullet);
        }
    }

    public void DestroyBullet(GameObject bullet)
    {
        _bullets.Remove(bullet);
        GameObject.Destroy(bullet);
    }

    #endregion

    #region Towers

    [ServerRpc(RequireOwnership = false)]
    public void SpawnTowerServerRpc(TowerType towerType, ulong playerId, Vector3 position)
    {
        TowerData data = DataManager.instance.GetTowerData(towerType);
        PlayerBehaviour player = NetworkManager.Singleton.ConnectedClients[playerId].PlayerObject.GetComponent<PlayerBehaviour>();
        Vector2Int coord = player.grid.GetCoordFromPosition(position);

        if (player.grid.IsEmpty(coord.x, coord.y) && player.gold >= data.cost)
        {
            GameObject tower = Instantiate(_towerBasePrefab, Vector3.zero, Quaternion.identity);
            tower.transform.position = position;

            if (player.grid.CanPlaceObject(tower))
            {
                tower.GetComponent<NetworkObject>().SpawnWithOwnership(playerId);
                tower.GetComponent<TowerBehaviour>().towerType = towerType;
                player.grid.SetEmpty(coord.x, coord.y, false);
                player.gold -= data.cost;
                _towers.Add(tower);
            }
            else
            {
                GameObject.Destroy(tower);
            }
        }
    }

    public void DestroyTower(GameObject tower)
    {
        _towers.Remove(tower);
        tower.GetComponent<NetworkObject>().Despawn();
    }

    #endregion
}
