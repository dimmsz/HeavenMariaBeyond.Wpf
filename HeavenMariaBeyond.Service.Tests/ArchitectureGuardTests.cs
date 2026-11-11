using System.IO;

namespace HeavenMariaBeyond.Service.Tests;

public class ArchitectureGuardTests
{
    [Fact]
    public void WpfProject_DoesNotReferenceInfrastructureProject()
    {
        var root = FindSolutionRoot();
        var wpfCsproj = Path.Combine(root, "HeavenMariaBeyond.Wpf", "HeavenMariaBeyond.Wpf.csproj");
        var content = File.ReadAllText(wpfCsproj);

        Assert.DoesNotContain(@"..\HeavenMariaBeyond.Infrastructure\HeavenMariaBeyond.Infrastructure.csproj", content);
    }

    [Fact]
    public void AppBootstrapper_UsesCoreRegistrationExtension()
    {
        var root = FindSolutionRoot();
        var appXamlCs = Path.Combine(root, "HeavenMariaBeyond.Wpf", "App.xaml.cs");
        var content = File.ReadAllText(appXamlCs);

        Assert.Contains("RegisterHeavenMariaBeyondCore()", content);
        Assert.DoesNotContain("using HeavenMariaBeyond.Infrastructure.", content);
        Assert.DoesNotContain("using HeavenNetCollector.Infrastructure.", content);
    }

    private static string FindSolutionRoot()
    {
        var current = new DirectoryInfo(AppContext.BaseDirectory);
        while (current != null)
        {
            var slnPath = Path.Combine(current.FullName, "HeavenMariaBeyond.sln");
            if (File.Exists(slnPath))
            {
                return current.FullName;
            }

            current = current.Parent;
        }

        throw new DirectoryNotFoundException("Could not find HeavenMariaBeyond.sln from test base directory.");
    }
}
