using System;
using System.Linq;
using System.Threading.Tasks;
using CommandLine;
using DevopsQuickstart.Models.Options;
using DevopsQuickstart.Services;
using DevopsQuickstart.Tool.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;

namespace DevopsQuickstart.Tool
{
	class Program
	{
		static void Main(string[] args)
		{
			Parser.Default.ParseArguments<CommandLineOptions>(args)
				.WithParsed(opts => RunOptions(opts).Wait());
		}
		
		static async Task RunOptions(CommandLineOptions opts)
		{
			var authenticationResult = await GetAuthenticationResult(opts);
			var serviceProvider = GetServiceProvider(opts, authenticationResult);

			var quickstartService = serviceProvider.GetRequiredService<IDevopsQuickstartService>();
			await quickstartService.QuickStart();
		}

		private static IServiceProvider GetServiceProvider(CommandLineOptions opts, AuthenticationResult authenticationResult)
		{
			var serviceCollection = new ServiceCollection();
			serviceCollection.AddDevopsQuickstart(new DevopsOptions
				{
					OrganizationUrl = opts.OrganizationUrl,
					AuthHeader = authenticationResult.CreateAuthorizationHeader()
				},
				new GitOptions
				{
					Username = authenticationResult.Account.Username,
					Password = authenticationResult.AccessToken
				});

			return serviceCollection.BuildServiceProvider();
		}
		
		private static async Task<AuthenticationResult> GetAuthenticationResult(CommandLineOptions options)
		{
			var authority = $"https://login.microsoftonline.com/{options.TenantId}/v2.0";

			var application = PublicClientApplicationBuilder.Create(options.ClientId)
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
				return await application.AcquireTokenWithDeviceCode(scopes, result =>
				{
					Console.WriteLine(result.Message);
					return Task.FromResult(0);
				}).ExecuteAsync();
			}
		}
	}
}