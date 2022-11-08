using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EnemyBehaviour : NetworkBehaviour, ISelectable, ITargetable
{
    [SerializeField] UIEnemyHUD _enemyUI;
    [SerializeField] Transform _modelParent;

    SKU.ResourceAttribute _healthAtt = null;

    EnemyData _data;
    public EnemyData data { get { return _data; } set { _data = value; } }

    NetworkVariable<float> _health = new NetworkVariable<float>();
    public float health { get { return _health.Value; } set { _health.Value = value; } }

    NetworkVariable<float> _healthMax = new NetworkVariable<float>();
    public float healthMax { get { return _healthMax.Value; } set { _healthMax.Value = value; } }

    NetworkVariable<int> _waveIndex = new NetworkVariable<int>();
    public int waveIndex { get { return _waveIndex.Value; } set { _waveIndex.Value = value; } }

    PlayerBehaviour _player;
    public PlayerBehaviour player { get { return _player; } set { _player = value; } }

    EnemyMovement _movement;

    void Start()
    {
        _health.OnValueChanged += (float old, float value) => { UpdateHealthUI(); };
    }

    public override void OnDestroy()
    {
        if (UIManager.instance != null)
        {
            UIManager.instance.HidePanel(PanelType.Enemy);
        }
    }

    public override void OnNetworkSpawn()
    {
        GameObject model = Instantiate(DataManager.instance.waves[waveIndex].enemyData.model);
        model.transform.SetParent(_modelParent, false);

        if (IsServer)
        {
            health = _data.health;
            _healthAtt = new SKU.ResourceAttribute(_data.health, _data.health, 1f, 0.5f);
            _healthAtt.AddOnValueChangedListener(UpdateHealth);

            AttributeManager attributeManager = gameObject.AddComponent<AttributeManager>();
            attributeManager.Add(AttributeType.Health, _healthAtt);

            _movement = gameObject.AddComponent<EnemyMovement>();
            _movement.InitServer(player, _data.speed);
        }
    }
    
    void UpdateHealth(SKU.ResourceAttribute attribute)
    {
        health = attribute.Value;
        healthMax = attribute.Max.Value;
    }

    void UpdateHealthUI()
    {
        _enemyUI.SetHealthBar(health / healthMax);
        UIManager.instance.UpdatePanel(PanelType.Enemy, this);
    }

    #region ISelectable

    public void Select()
    {
        UIManager.instance.ShowPanel(PanelType.Enemy, this);
    }

    public void UnSelect()
    {
        UIManager.instance.HidePanel(PanelType.Enemy);
    }

    #endregion

    #region ITargetable

    public void OnHit(GameObject emitter)
    {
        Debug.Log("OnHit " + emitter.GetComponent<NetworkObject>().OwnerClientId);
        if (IsServer)
        {
            Debug.Log("OnHit server only");
            var attacker = emitter.GetComponent<IAttacker>();
            if (attacker != null)
            {
                attacker.ApplyOnHitEffect(gameObject);
            }

            if (_healthAtt.Value <= 0)
            {
                Die(emitter);
            }
        }
    }

    public void Die(GameObject killer)
    {
        EntityManager.instance.DestroyEnemy(gameObject);
        //var player = PlayerObjectRegistry.GetPlayer(killer.GetComponent<TowerBehaviour>().entity.controller);
        //player.behavior.state.Score += _data.score;
        //player.behavior.state.Gold += _data.gold;
    }

    #endregion

    void OnCollisionEnter(Collision collision)
    {
        if (IsServer)
        {
            CheckPoint checkPoint = collision.gameObject.GetComponent<CheckPoint>();
            if (checkPoint != null && _movement.destination == checkPoint)
            {
                if (checkPoint.isLast)
                {
                    EntityManager.instance.DestroyEnemy(gameObject);
                }
                else
                {
                    _movement.SetNextDestination(checkPoint.next);
                }
            }
        }
    }
}
