// ------------------------------------------------------------------------------------------
//  Copyright (c) Natsuneko. All rights reserved.
//  Licensed under the MIT License. See LICENSE in the project root for license information.
// ------------------------------------------------------------------------------------------

namespace NatsunekoLaboratory.CsprojHooks.Features.Abstractions
{
    public interface ICsprojHooksFeature
    {
        string OnGeneratedCSProject(string path, string content);
    }
}