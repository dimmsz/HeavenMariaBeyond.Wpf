using HeavenMaria.Domain.Interfaces;
using HeavenMaria.Infrastructure.Compression;
using HeavenMaria.Infrastructure.Generators;
using HeavenMaria.Infrastructure.Repositories;
using HeavenNetCollector.Infrastructure.Providers;
using Prism.Ioc;

namespace HeavenMaria.Service
{
    public static class ContainerRegistryExtensions
    {
        public static void RegisterHeavenMariaCore(this IContainerRegistry containerRegistry)
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
