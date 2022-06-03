// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Linq;

using UnityEngine;

namespace NatsunekoLaboratory.CsprojHooks.SerDe
{
    [Serializable]
    internal class Dictionary<TKey, TValue> : ISerializationCallbackReceiver
    {
        private System.Collections.Generic.Dictionary<TKey, TValue> _dictionary;

        [SerializeField]
        private KeyValuePair<TKey, TValue>[] _store = default;

        public TValue this[TKey index]
        {
            get => _dictionary[index];
            set => _dictionary[index] = value;
        }

        public bool ContainsKey(TKey key)
        {
            return _dictionary.ContainsKey(key);
        }

        public void Add(TKey key, TValue value)
        {
            _dictionary.Add(key, value);
        }

        public Dictionary(System.Collections.Generic.Dictionary<TKey, TValue> dictionary)
        {
            _dictionary = dictionary;
        }


        public void OnBeforeSerialize()
        {
            _store = _dictionary.Select(w => new KeyValuePair<TKey, TValue>(w.Key, w.Value))
                                .ToArray();
        }

        public void OnAfterDeserialize()
        {
            _dictionary = _store.ToDictionary(w => w.Key, w => w.Value);
        }
    }
}