// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;

using NatsunekoLaboratory.CsprojHooks.Features.Abstractions;

using UnityEditor;

using UnityEngine;

namespace NatsunekoLaboratory.CsprojHooks
{
    public class CsprojHooksAssetPreprocessor : AssetPostprocessor
    {
        private static readonly List<ICsprojHooksFeature> InternalFeatures;

        public static IReadOnlyCollection<ICsprojHooksFeature> Features => InternalFeatures.AsReadOnly();

        static CsprojHooksAssetPreprocessor()
        {
            InternalFeatures = CollectFeatures();

            foreach (var feature in Features)
                if (feature is ICsprojHooksConfigurableFeature configurable)
                {
                    if (CsprojHooksSettingsStore.Instance.Store.ContainsKey(configurable.Id))
                    {
                        var json = CsprojHooksSettingsStore.Instance.Store[configurable.Id];
                        var obj = JsonUtility.FromJson(json, configurable.T);
                        configurable.Initialize(obj, () => SaveConfiguration(configurable.Id, obj));
                    }
                    else
                    {
                        var obj = Activator.CreateInstance(configurable.T);
                        CsprojHooksSettingsStore.Instance.Store[configurable.Id] = JsonUtility.ToJson(obj);

                        configurable.Initialize(obj, () => SaveConfiguration(configurable.Id, obj));
                    }
                }
        }

        private static List<ICsprojHooksFeature> CollectFeatures()
        {
            var features = new List<ICsprojHooksFeature>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            foreach (var t in assembly.GetTypes())
                if (typeof(ICsprojHooksFeature).IsAssignableFrom(t) && !t.IsInterface)
                    features.Add(Activator.CreateInstance(t) as ICsprojHooksFeature);

            return features;
        }

        private static string OnGeneratedCSProject(string path, string content)
        {
            return Features.Aggregate(content, (c, w) => w.OnGeneratedCSProject(path, c));
        }

        private static string OnGeneratedSlnSolution(string path, string content)
        {
            return Features.OfType<ICsprojHooksSlnFeature>().Aggregate(content, (c, w) => w.OnGeneratedSlnSolution(path, c));
        }

        private static void SaveConfiguration(string id, object obj)
        {
            CsprojHooksSettingsStore.Instance.Store[id] = JsonUtility.ToJson(obj);
            CsprojHooksSettingsStore.Instance.Save();
        }
    }
}