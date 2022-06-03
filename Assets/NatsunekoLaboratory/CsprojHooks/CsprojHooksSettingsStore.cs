// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.IO;

using JetBrains.Annotations;

using NatsunekoLaboratory.CsprojHooks.SerDe;

using UnityEngine;

namespace NatsunekoLaboratory.CsprojHooks
{
    [Serializable]
    internal class CsprojHooksSettingsStore : ScriptableObject
    {
        private const string Path = "ProjectSettings/CsprojHooksSettings.json";

        private static CsprojHooksSettingsStore Load()
        {
            if (File.Exists(Path))
            {
                var instance = CreateInstance<CsprojHooksSettingsStore>();
                JsonUtility.FromJsonOverwrite(File.ReadAllText(Path), instance);

                return instance;
            }

            return CreateInstance<CsprojHooksSettingsStore>();
        }

        #region Instance

        [CanBeNull]
        private static CsprojHooksSettingsStore _instance;

        public static CsprojHooksSettingsStore Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Load();

                return _instance;
            }
        }

        #endregion

        public CsprojHooksSettingsStore()
        {
            Store = new Dictionary<string, string>(new System.Collections.Generic.Dictionary<string, string>());
        }

        public void Save()
        {
            var str = JsonUtility.ToJson(_instance, true);
            File.WriteAllText(Path, JsonUtility.ToJson(_instance));
        }

        #region Store

        [SerializeField]
        private Dictionary<string, string> _store;

        public Dictionary<string, string> Store
        {
            get => _store;
            set => _store = value;
        }

        #endregion
    }
}