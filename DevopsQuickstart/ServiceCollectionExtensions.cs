using DevopsQuickstart.Models.Options;
using DevopsQuickstart.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DevopsQuickstart
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddDevopsQuickstart(this IServiceCollection serviceCollection, DevopsOptions devopsOptions, GitOptions gitOptions)
		{
			return serviceCollection
				.AddTransient<IDevopsQuickstartService, DevopsQuickstartService>()
				.AddTransient<IGitService, GitService>()
				.AddTransient<IDevopsService, DevopsService>()
				.AddTransient<IInteractiveService, CliService>()
				.AddSingleton(devopsOptions)
				.AddSingleton(gitOptions);
		}
	}
}