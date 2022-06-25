using Unity.Netcode;
using UnityEngine;

public interface IAttacker
{
    void ApplyOnHitEffect(GameObject target);
    GameObject GetTarget();
}

public class TowerBehaviour : NetworkBehaviour, ISelectable, IAttacker
{
    TowerData _data;

    SKU.Attribute _attackRateAtt;
    SKU.Attribute _damageAtt;
    SKU.Attribute _rangeAtt;

    NetworkVariable<float> _attackRate = new NetworkVariable<float>();
    public float attackRate { get { return _attackRate.Value; } set { _attackRate.Value = value; } }

    NetworkVariable<float> _damage = new NetworkVariable<float>();
    public float damage { get { return _damage.Value; } set { _damage.Value = value; } }

    NetworkVariable<float> _range = new NetworkVariable<float>();
    public float range { get { return _range.Value; } set { _range.Value = value; } }

    NetworkVariable<TowerType> _towerType = new NetworkVariable<TowerType>();
    public TowerType towerType { get { return _towerType.Value; } set { _towerType.Value = value; } }

    GameObject _model = null;

    public override void OnNetworkSpawn()
    {
        Debug.Log("OnNetworkSpawn " + towerType);
        _attackRate.OnValueChanged += (float old, float value) => { UpdateStat(); };
        _damage.OnValueChanged += (float old, float value) => { UpdateStat(); };
        _range.OnValueChanged += (float old, float value) => { UpdateStat(); };
        _towerType.OnValueChanged += (TowerType old, TowerType value) => { UpdateData(); };

        if (towerType != TowerType.None)
        {
            _data = DataManager.instance.GetTowerData((TowerType)towerType);
            UpdateModel();
        }
    }

    void UpdateData()
    {
        Debug.Log("Tower type " + _towerType);
        _data = DataManager.instance.GetTowerData(_towerType.Value);

        if (IsServer)
        {
            if (_data.canAttack)
            {
                _attackRateAtt = new SKU.Attribute();
                _damageAtt = new SKU.Attribute();
                _rangeAtt = new SKU.Attribute();

                _attackRateAtt.AddOnValueChangedListener(UpdateAttackRate);
                _damageAtt.AddOnValueChangedListener(UpdateDamage);
                _rangeAtt.AddOnValueChangedListener(UpdateRange);

                AttributeManager attributeManager = gameObject.AddComponent<AttributeManager>();
                attributeManager.Add(AttributeType.AttackRate, _attackRateAtt);
                attributeManager.Add(AttributeType.Damage, _damageAtt);
                attributeManager.Add(AttributeType.Range, _rangeAtt);

                _attackRateAtt.BaseValue = _data.attackRate;
                _damageAtt.BaseValue = _data.damage;
                _rangeAtt.BaseValue = _data.range;

                // TODO init from data
                gameObject.AddComponent<ShootBullet>();
            }
        }

        UpdateModel();
    }

    void UpdateStat()
    {
        UIManager.instance.UpdatePanel(PanelType.Tower, this);
    }

    void UpdateModel()
    {
        if (_model)
        {
            Destroy(_model);
        }
        if (_data && _data.model)
        {
            _model = Instantiate(_data.model);
            _model.transform.SetParent(transform, false);
        }
    }

    void UpdateAttackRate(SKU.Attribute attribute)
    {
        _attackRate.Value = attribute.Value;
    }

    void UpdateDamage(SKU.Attribute attribute)
    {
        _damage.Value = attribute.Value;
    }

    void UpdateRange(SKU.Attribute attribute)
    {
        _range.Value = attribute.Value;
    }

    #region IAttacker

    public void ApplyOnHitEffect(GameObject target)
    {
        var attributes = target.GetComponent<AttributeManager>();
        attributes.Get<SKU.ResourceAttribute>(AttributeType.Health).Remove(_damageAtt.Value);

        // TODO: clean data for modifiers
        if (_data.modifiers != null)
        {
            for (int i = 0; i < _data.modifiers.Count; i++)
            {
                if (_data.modifiers[i] == ModifierType.Time)
                {
                    attributes.Get<SKU.Attribute>(AttributeType.Speed).AddRelativeModifier(Factory.CreateModifier(ModifierType.Time, 2f, -0.8f));
                }
            }
        }
    }

    public GameObject GetTarget()
    {
        return GetNearestEnemy();
    }

    #endregion

    #region ISelectable

    public void Select()
    {
        UIManager.instance.ShowPanel(PanelType.Tower, this);
    }

    public void UnSelect()
    {
        UIManager.instance.HidePanel(PanelType.Tower);
    }

    #endregion

    // TODO abstract strategy pour choisir le bon enemy (plus pret, plus de vie, plus proche de la fin, boss, etc...)
    // store current target to avoid useless computation
    GameObject GetNearestEnemy()
    {
        var enemies = EntityManager.instance.GetEnemies();

        float min = Mathf.Infinity;
        GameObject nearest = null;
        for (int i = 0; i < enemies.Count; i++)
        {
            float dist = Vector3.Distance(enemies[i].transform.position, gameObject.transform.position);
            if (dist < _rangeAtt.Value && dist < min)
            {
                min = dist;
                nearest = enemies[i];
            }
        }
        return nearest;
    }

    public BulletType GetBulletType()
    {
        return _data.bulletId;
    }
}
