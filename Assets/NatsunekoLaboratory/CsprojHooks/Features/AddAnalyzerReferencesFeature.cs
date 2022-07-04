// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using NatsunekoLaboratory.CsprojHooks.Features.Abstractions;
using NatsunekoLaboratory.CsprojHooks.Store;

using UnityEditor;

using UnityEditorInternal;

using UnityEngine;

namespace NatsunekoLaboratory.CsprojHooks.Features
{
    public class AddAnalyzerReferencesFeature : ICsprojHooksFeature, ICsprojHooksConfigurableFeature
    {
        private ReorderableList _reorderableList;
        private AddAnalyzerReferencesConfiguration _configuration;
        private Action _saveCallback;

        public Type T => typeof(AddAnalyzerReferencesConfiguration);

        public string Id => "NatsunekoLaboratory.CsprojHooks.AddAnalyzerReferences";

        public void Initialize(object obj, Action save)
        {
            var config = obj as AddAnalyzerReferencesConfiguration;
            if (config == null)
                return;

            _configuration = config;
            _saveCallback = save;

            var addAnalyzerReferencesProjects = new List<string>(config.AddAnalyzerReferencesProjects ?? new List<string>());
            _reorderableList = new ReorderableList(addAnalyzerReferencesProjects, typeof(string), true, false, true, true)
            {
                drawNoneElementCallback = rect => EditorGUI.LabelField(rect, "No project specified that reference Roslyn analyzers"),
                drawElementCallback = (rect, index, active, focus) =>
                {
                    using (var scope = new EditorGUI.ChangeCheckScope())
                    {
                        var item = addAnalyzerReferencesProjects[index];

                        const int controlGap = 4;
                        const int buttonWidth = 32;

                        rect.width -= buttonWidth + controlGap;

                        EditorGUI.LabelField(rect, item);

                        rect.x += rect.width + controlGap;
                        rect.width = buttonWidth;

                        if (GUI.Button(rect, "..."))
                        {
                            var path = EditorUtility.OpenFilePanelWithFilters("Add csproj to add analyzer references", Path.GetDirectoryName(Application.dataPath), new[] { "C# Project File (*.csproj)", "csproj" });
                            if (string.IsNullOrEmpty(path))
                                return;

                            var root = Path.GetFullPath(Path.Combine(Application.dataPath, "..")).Replace("\\", "/");
                            addAnalyzerReferencesProjects[index] = path.Replace($"{root}/", "");
                            GUI.changed = true;
                        }

                        if (scope.changed)
                        {
                            config.AddAnalyzerReferencesProjects = addAnalyzerReferencesProjects;
                            save();
                        }
                    }
                },
                onChangedCallback = _ =>
                {
                    config.AddAnalyzerReferencesProjects = addAnalyzerReferencesProjects;
                    save();
                }
            };
        }

        public void OnGUI()
        {
            using (new EditorGUILayout.VerticalScope())
            {
                using (var scope = new EditorGUI.ChangeCheckScope())
                {
                    _configuration.IsEnableAddAnalyzerReferences = EditorGUILayout.ToggleLeft("Add Analyzer References", _configuration.IsEnableAddAnalyzerReferences);

                    if (scope.changed)
                        _saveCallback();
                }

                if (_configuration.IsEnableAddAnalyzerReferences)
                {
                    _reorderableList.DoLayoutList();

                    EditorGUILayout.Space(5);

                    var analyzers = AssetDatabase.FindAssets("l:RoslynAnalyzer").Select(AssetDatabase.GUIDToAssetPath).ToArray();
                    foreach (var analyzer in analyzers)
                        EditorGUILayout.LabelField(analyzer);

                    EditorGUILayout.HelpBox("Analyzers must be labeled as 'RoslynAnalyzer'", MessageType.Info);
                }
            }
        }

        public string OnGeneratedCSProject(string path, string content)
        {
            if (_configuration == null)
                return content;

            if (!_configuration.AddAnalyzerReferencesProjects.Contains(path))
                return content;

            var analyzers = AssetDatabase.FindAssets("l:RoslynAnalyzer").Select(AssetDatabase.GUIDToAssetPath).ToArray();
            var document = XDocument.Parse(content);
            var @namespace = (XNamespace)"http://schemas.microsoft.com/developer/msbuild/2003";
            var project = document.Element(@namespace + "Project");
            var itemGroup = new XElement(@namespace + "ItemGroup");

            foreach (var analyzer in analyzers)
            {
                var include = new XAttribute("Include", analyzer);
                var reference = new XElement(@namespace + "Analyzer", include);

                itemGroup.Add(reference);
            }

            project?.Add(itemGroup);

            return document.ToString();
        }
    }
}