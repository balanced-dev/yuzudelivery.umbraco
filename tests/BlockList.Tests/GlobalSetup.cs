using System;
using System.Collections.Generic;
using System.Threading;
using NSubstitute.Core;
using NSubstitute.Core.DependencyInjection;
using NSubstitute.Routing.AutoValues;

// ReSharper disable CheckNamespace
// ReSharper disable ClassNeverInstantiated.Global
// ReSharper disable AccessToModifiedClosure
// ReSharper disable ConvertToLocalFunction
#pragma warning disable CA1050

[SetUpFixture]
public class GlobalSetup
{
    [OneTimeSetUp]
    public void BeforeTests()
    {
        var container = NSubstituteDefaultFactory.DefaultContainer.Customize();
        container.RegisterPerScope<IAutoValueProvidersFactory, CustomAutoProvidersFactory>();

        SubstitutionContext.Current = container.Resolve<ISubstitutionContext>();
    }
}

#pragma warning restore CA1050
