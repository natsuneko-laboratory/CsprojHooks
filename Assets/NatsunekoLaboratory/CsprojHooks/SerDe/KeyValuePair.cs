// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;

using UnityEngine;

namespace NatsunekoLaboratory.CsprojHooks.SerDe
{
    [Serializable]
    internal struct KeyValuePair<TKey, TValue>
    {
        public KeyValuePair(TKey key, TValue value)
        {
            _key = key;
            _value = value;
        }

        #region Key

        [SerializeField]
        private TKey _key;

        public TKey Key
        {
            get => _key;
            set => _key = value;
        }

        #endregion

        #region Value

        [SerializeField]
        private TValue _value;

        public TValue Value
        {
            get => _value;
            set => _value = value;
        }

        #endregion
    }
}