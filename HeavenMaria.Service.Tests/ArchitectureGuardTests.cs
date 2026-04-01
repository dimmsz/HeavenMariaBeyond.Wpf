using System.IO;

namespace HeavenMaria.Service.Tests;

public class ArchitectureGuardTests
{
    [Fact]
    public void WpfProject_DoesNotReferenceInfrastructureProject()
    {
        var root = FindSolutionRoot();
        var wpfCsproj = Path.Combine(root, "HeavenMaria.Wpf", "HeavenMaria.Wpf.csproj");
        var content = File.ReadAllText(wpfCsproj);

        Assert.DoesNotContain(@"..\HeavenMaria.Infrastructure\HeavenMaria.Infrastructure.csproj", content);
    }

    [Fact]
    public void AppBootstrapper_UsesCoreRegistrationExtension()
    {
        var root = FindSolutionRoot();
        var appXamlCs = Path.Combine(root, "HeavenMaria.Wpf", "App.xaml.cs");
        var content = File.ReadAllText(appXamlCs);

        Assert.Contains("RegisterHeavenMariaCore()", content);
        Assert.DoesNotContain("using HeavenMaria.Infrastructure.", content);
        Assert.DoesNotContain("using HeavenNetCollector.Infrastructure.", content);
    }

    private static string FindSolutionRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            var slnPath = Path.Combine(current.FullName, "HeavenMaria.Wpf.sln");
            if (File.Exists(slnPath))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not find HeavenMaria.Wpf.sln from test base directory.");
    }
}
