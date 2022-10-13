using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Exceptions;
using YuzuDelivery.Core.ViewModelBuilder;
using YuzuDelivery.Umbraco.Core.Settings;
using YuzuDelivery.Umbraco.Core.Startup;

namespace YuzuDelivery.Umbraco.Core.Tests.Startup;

public class DefaultUmbracoVmBuilderConfigTests
{
    [Test]
    [TestCase(@"c:\www\", @"c:\www\foo\bar", false, false)]
    [TestCase(@"c:\www\", @"c:\notwww\foo\bar", false, true)]
    [TestCase(@"c:\www\", @"c:\www\..\notwww\foo\bar", false, true)]
    [TestCase(@"c:\www\", @"c:\notwww\foo\bar", true, false)]
    public void Constructor_WithAbsolutePathsForVmGenerationDirectory_RespectsAllowUnsafeConfiguration(
        string contentRoot,
        string viewModelsDirectory,
        bool allowUnsafe,
        bool shouldThrow)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Assert.Ignore();
        }

        var hostEnvironment = Substitute.For<IHostEnvironment>();
        hostEnvironment.ContentRootPath.Returns(contentRoot);

        var settings = Substitute.For<IOptionsMonitor<VmGenerationSettings>>();
        settings.CurrentValue.Returns(new VmGenerationSettings
        {
            Directory = viewModelsDirectory,
            AcceptUnsafeDirectory = allowUnsafe
        });

        if (shouldThrow)
        {
            Assert.Throws<ConfigurationException>(() =>
            {
                _ = new DefaultUmbracoVmBuilderConfig(
                    Enumerable.Empty<IUpdateableVmBuilderConfig>(),
                    hostEnvironment,
                    settings);
            });
        }
        else
        {
            Assert.DoesNotThrow(() =>
            {
                _ = new DefaultUmbracoVmBuilderConfig(
                    Enumerable.Empty<IUpdateableVmBuilderConfig>(),
                    hostEnvironment,
                    settings);
            });
        }
    }

    [Test]
    [TestCase(@"c:\www\", @"./bar", false, false)]
    [TestCase(@"c:\www\", @"../bar", false, true)]
    [TestCase(@"c:\www\", @"../bar", true, false)]
    public void Constructor_WithRelativePathsForVmGenerationDirectory_RespectsAllowUnsafeConfiguration(
        string contentRoot,
        string viewModelsDirectory,
        bool allowUnsafe,
        bool shouldThrow)
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            Assert.Ignore();
        }

        var hostEnvironment = Substitute.For<IHostEnvironment>();
        hostEnvironment.ContentRootPath.Returns(contentRoot);

        var settings = Substitute.For<IOptionsMonitor<VmGenerationSettings>>();
        settings.CurrentValue.Returns(new VmGenerationSettings
        {
            Directory = viewModelsDirectory,
            AcceptUnsafeDirectory = allowUnsafe
        });

        if (shouldThrow)
        {
            Assert.Throws<ConfigurationException>(() =>
            {
                _ = new DefaultUmbracoVmBuilderConfig(
                    Enumerable.Empty<IUpdateableVmBuilderConfig>(),
                    hostEnvironment,
                    settings);
            });
        }
        else
        {
            Assert.DoesNotThrow(() =>
            {
                _ = new DefaultUmbracoVmBuilderConfig(
                    Enumerable.Empty<IUpdateableVmBuilderConfig>(),
                    hostEnvironment,
                    settings);
            });
        }
    }
}