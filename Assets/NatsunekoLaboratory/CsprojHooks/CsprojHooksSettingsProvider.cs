// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Reflection;

using NatsunekoLaboratory.CsprojHooks.Features.Abstractions;

using Unity.CodeEditor;

using UnityEditor;

using UnityEngine;
using UnityEngine.UIElements;

namespace NatsunekoLaboratory.CsprojHooks
{
    internal class CsprojHooksSettingsProvider : SettingsProvider
    {
        public CsprojHooksSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords) { }

        [SettingsProvider]
        public static SettingsProvider CreateCsprojHooksSettingsProvider()
        {
            return new CsprojHooksSettingsProvider("Project/Editor/Csproj Hooks", SettingsScope.Project, new[] { "C#", "csproj", "Roslyn", "Analyzer" });
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            foreach (var feature in CsprojHooksAssetPreprocessor.Features)
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

        public override void OnDeactivate()
        {
            CsprojHooksSettingsStore.Instance.Save();
        }

        public override void OnGUI(string searchContext)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.inspectorFullWidthMargins))
            {
                foreach (var feature in CsprojHooksAssetPreprocessor.Features)
                    if (feature is ICsprojHooksConfigurableFeature configurable)
                    {
                        configurable.OnGUI();
                        GUILayout.Space(10);
                    }

                if (GUILayout.Button("Regenerate .csproj files"))
                {
                    var editor = CodeEditor.CurrentEditor.GetType().Name;
                    if (editor == "DefaultExternalCodeEditor" || editor == "VisualStudioEditor")
                    {
                        var t1 = Type.GetType("UnityEditor.SyncVS, UnityEditor");
                        var t2 = Type.GetType("UnityEditor.VisualStudioIntegration.SolutionSynchronizer, UnityEditor");

                        if (t1 == null || t2 == null)
                        {
                            // VisualStudioIntegration.SolutionSynchronizer is removed by Unity Technologies.
                            var t3 = Type.GetType("Microsoft.Unity.VisualStudio.Editor.VisualStudioEditor, Unity.VisualStudio.Editor");
                            var t4 = Type.GetType("Microsoft.Unity.VisualStudio.Editor.ProjectGeneration, Unity.VisualStudio.Editor");
                            if (t3 == null || t4 == null)
                                throw new InvalidOperationException("failed to reflect Visual Studio Integration types");

                            var generator = t3.GetField("_generator", BindingFlags.Instance | BindingFlags.NonPublic);
                            var sync = t4.GetMethod("Sync", BindingFlags.Instance | BindingFlags.Public);

                            if (generator == null || sync == null)
                                throw new InvalidOperationException("failed to reflect one or more members");

                            var val = generator.GetValue(CodeEditor.CurrentEditor);
                            sync.Invoke(val, new object[] { });
                        }
                        else
                        {
                            // UnityEditor.VisualStudioIntegration.SolutionSynchronizer is alive.
                            var synchronizer = t1.GetField("Synchronizer", BindingFlags.Static | BindingFlags.NonPublic);
                            var sync = t2.GetMethod("Sync", BindingFlags.Instance | BindingFlags.Public);
                            if (synchronizer == null || sync == null)
                                throw new InvalidOperationException("failed to reflect one or more members");

                            var val = synchronizer.GetValue(null);
                            sync.Invoke(val, new object[] { });
                        }
                    }
                }
            }
        }

        private static void SaveConfiguration(string id, object obj)
        {
            CsprojHooksSettingsStore.Instance.Store[id] = JsonUtility.ToJson(obj);
            CsprojHooksSettingsStore.Instance.Save();
        }
    }
}