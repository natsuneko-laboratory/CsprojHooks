// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;

using UnityEngine;

namespace NatsunekoLaboratory.CsprojHooks.Store
{
    [Serializable]
    public class AddAnalyzerReferencesConfiguration
    {
        #region IsEnableAddAnalyzerReferences

        [SerializeField]
        private bool _isEnableAddAnalyzerReferences;

        public bool IsEnableAddAnalyzerReferences
        {
            get => _isEnableAddAnalyzerReferences;
            set => _isEnableAddAnalyzerReferences = value;
        }

        #endregion

        #region AddAnalyzerReferencesProjects

        [SerializeField]
        private List<string> _addAnalyzerReferencesProjects;

        public List<string> AddAnalyzerReferencesProjects
        {
            get => _addAnalyzerReferencesProjects;
            set => _addAnalyzerReferencesProjects = value;
        }

        #endregion
    }
}