using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SKU
{

    public class Attribute : IAttribute
    {
        public delegate void OnValueChanged(Attribute attribute);
        event OnValueChanged _onValueChanged;

        float _baseValue;
        float _value;
        float _prevValue;

        Dictionary<GameObject, List<IAttributeModifier>> _relativeModifiers;
        Dictionary<GameObject, List<IAttributeModifier>> _absoluteModifiers;

        public Attribute()
            : this(0f) { }

        public Attribute(float value)
        {
            _prevValue = _value;
            _baseValue = value;
            _relativeModifiers = new Dictionary<GameObject, List<IAttributeModifier>>();
            _absoluteModifiers = new Dictionary<GameObject, List<IAttributeModifier>>();
            Update();
        }

        public void Update()
        {
            float relativeBonus = 1f;
            foreach (var kvp in _relativeModifiers)
            {
                List<IAttributeModifier> modifiers = kvp.Value;
                for (int i = modifiers.Count - 1; i >= 0; i--)
                {
                    relativeBonus *= 1f + modifiers[i].ApplyModifier();
                    if (modifiers[i].isOver)
                    {
                        modifiers.RemoveAt(i);
                    }
                }
            }

            float absoluteBonus = 0f;
            foreach (var kvp in _absoluteModifiers)
            {
                List<IAttributeModifier> modifiers = kvp.Value;
                for (int i = modifiers.Count - 1; i >= 0; i--)
                {
                    absoluteBonus += modifiers[i].ApplyModifier();
                    if (modifiers[i].isOver)
                    {
                        modifiers.RemoveAt(i);
                    }
                }
            }
            // TODO remove modifiers from source if empty

            _prevValue = _value;
            _value = _baseValue * relativeBonus + absoluteBonus;
            _value = Mathf.Max(_value, 0f);

            if (_onValueChanged != null && _prevValue != _value)
            {
                _onValueChanged(this);
            }
        }

        public void AddRelativeModifier(GameObject source, IAttributeModifier modifier)
        {
            if (!_relativeModifiers.ContainsKey(source))
            {
                _relativeModifiers[source] = new List<IAttributeModifier>();
            }
            _relativeModifiers[source].Add(modifier);
        }

        public List<IAttributeModifier> GetRelativeModifier(GameObject source)
        {
            return _relativeModifiers[source];
        }

        public void RemoveRelativeModifierFromSource(GameObject source)
        {
            _relativeModifiers[source].Clear();
        }

        public void AddAbsoluteModifier(GameObject source, IAttributeModifier modifier)
        {
            if (!_absoluteModifiers.ContainsKey(source))
            {
                _absoluteModifiers[source] = new List<IAttributeModifier>();
            }
            _absoluteModifiers[source].Add(modifier);
        }

        public List<IAttributeModifier> GetAbsoluteModifier(GameObject source)
        {
            return _absoluteModifiers[source];
        }

        public void RemoveAbsoluteModifierFromSource(GameObject source)
        {
            _absoluteModifiers[source].Clear();
        }

        public float Value
        {
            get { return _value; }
        }

        public float BaseValue
        {
            get { return _baseValue; }
            set { _baseValue = value; }
        }

        public void AddOnValueChangedListener(OnValueChanged onValueChanged)
        {
            _onValueChanged += onValueChanged;
        }

        public void RemoveOnValueChangedListener(OnValueChanged onValueChanged)
        {
            _onValueChanged -= onValueChanged;
        }
    }
}