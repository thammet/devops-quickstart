using System;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using CommandLine;
using DevopsQuickstart.Models.Options;
using DevopsQuickstart.Services;
using Microsoft.Identity.Client;

namespace DevopsQuickstart
{
	class Program
	{
		private static CommandLineOptions CommandLineOptions;
		private static AuthenticationResult AuthenticationResult;
		private static IContainer Container;
		private static IInteractiveService InteractiveService;
		
		static void Main(string[] args)
		{
			Container = BuildContainer();
			InteractiveService = Container.Resolve<IInteractiveService>();
			
			Parser.Default.ParseArguments<CommandLineOptions>(args)
				.WithParsed(opts => RunOptions(opts).Wait());
		}
		
		static async Task RunOptions(CommandLineOptions opts)
		{
			CommandLineOptions = opts;
			
			try
			{
				AuthenticationResult = await GetAuthenticationResult();
			
				var quickStartService = Container.Resolve<IDevopsQuickstartService>();
				await quickStartService.QuickStart();
			}
			catch (Exception exception)
			{
				InteractiveService.ShowError(exception.Message);
			}
		}

		private static IContainer BuildContainer()
		{
			var containerBuilder = new ContainerBuilder();
			containerBuilder.RegisterModule<DevopsQuickStartModule>();

			containerBuilder.Register(context => new DevopsOptions
			{
				AuthHeader = AuthenticationResult.CreateAuthorizationHeader(),
				OrganizationUrl = CommandLineOptions.OrganizationUrl
			});

			containerBuilder.Register(context => new GitOptions
			{
				Username = AuthenticationResult.Account.Username,
				Password = AuthenticationResult.AccessToken
			});

			return containerBuilder.Build();
		}
		
		private static async Task<AuthenticationResult> GetAuthenticationResult()
		{
			var authority = $"https://login.microsoftonline.com/{CommandLineOptions.TenantId}/v2.0";

			var application = PublicClientApplicationBuilder.Create(CommandLineOptions.ClientId)
				.WithAuthority(authority)
				.WithDefaultRedirectUri()
				.Build();
            
			// Constant value to target Azure DevOps. Do not change! 
			var scopes = new string[] { "499b84ac-1321-427f-aa17-267ca6975798/user_impersonation" };
			
			try
			{
				var accounts = await application.GetAccountsAsync();
				// Try to acquire an access token from the cache. If device code is required, Exception will be thrown.
				return await application.AcquireTokenSilent(scopes, accounts.FirstOrDefault()).ExecuteAsync();
			}
			catch (MsalUiRequiredException)
			{
				return await application.AcquireTokenWithDeviceCode(scopes, deviceCodeResult =>
				{
					InteractiveService.ShowMessage(deviceCodeResult.Message);
					return Task.FromResult(0);
				}).ExecuteAsync();
			}
		}
	}
}