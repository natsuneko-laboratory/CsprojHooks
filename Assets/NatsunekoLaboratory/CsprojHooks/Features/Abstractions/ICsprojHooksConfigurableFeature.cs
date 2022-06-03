// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

using System;

namespace NatsunekoLaboratory.CsprojHooks.Features.Abstractions
{
    public interface ICsprojHooksConfigurableFeature
    {
        Type T { get; }

        string Id { get; }

        void Initialize(object obj, Action save);

        void OnGUI();
    }
}