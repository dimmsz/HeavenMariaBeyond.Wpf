using HeavenMariaBeyond.Domain.Interfaces;
using HeavenMariaBeyond.Infrastructure.Compression;
using HeavenMariaBeyond.Infrastructure.Generators;
using HeavenMariaBeyond.Infrastructure.Repositories;
using HeavenNetCollector.Infrastructure.Providers;
using Prism.Ioc;

namespace HeavenMariaBeyond.Service
{
    public static class ContainerRegistryExtensions
    {
        public static void RegisterHeavenMariaBeyondCore(this IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IDiaryRepository, DiaryRepository>();
            containerRegistry.RegisterSingleton<IGirlStatusRepository, GirlStatusRepository>();
            containerRegistry.RegisterSingleton<IGirlWithStatusRepository, GirlWithStatusRepository>();

            containerRegistry.RegisterSingleton<IPathProvider, PathProvider>();
            containerRegistry.RegisterSingleton<IDiaryHtmlGenerator, DiaryHtmlGenerator>();

            containerRegistry.RegisterSingleton<IConnectionStringProvider, ConnectionStringProvider>();
            containerRegistry.RegisterSingleton<ICompressionService, ZstdCompressionService>();

            containerRegistry.RegisterSingleton<IMainWindowQueryService, MainWindowQueryService>();
            containerRegistry.RegisterSingleton<IMainWindowStatusService, MainWindowStatusService>();
            containerRegistry.RegisterSingleton<ISelectModeService, SelectModeService>();
            containerRegistry.RegisterSingleton<ISelectModeStateService, SelectModeStateService>();
            containerRegistry.RegisterSingleton<IExternalLauncherService, ExternalLauncherService>();
            containerRegistry.RegisterSingleton<IDiaryDisplayService, DiaryDisplayService>();
        }
    }
}
