using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DevopsQuickstart.Models;
using DevopsQuickstart.Models.Devops;

namespace DevopsQuickstart.Services
{
	public interface IDevopsQuickstartService
	{
		Task<DevopsQuickstartResult> QuickStart();
	}

	public class DevopsQuickstartService : IDevopsQuickstartService
	{
		private readonly IInteractiveService _interactiveService;
		private readonly IDevopsService _devopsService;
		private readonly IGitService _gitService;
		private readonly IRetryService _retryService;

		public DevopsQuickstartService(IInteractiveService interactiveService, IDevopsService devopsService, IGitService gitService, IRetryService retryService)
		{
			_interactiveService = interactiveService;
			_devopsService = devopsService;
			_gitService = gitService;
			_retryService = retryService;
		}

		public virtual async Task<DevopsQuickstartResult> QuickStart()
		{
			_interactiveService.ShowMessage("Loading projects...");
			_interactiveService.Projects = await _devopsService.GetAvailableProjects();

			var repository = await CreateRepository();
			
			PushCode(repository);
			
			var pipelines = await CreatePipelines();
			
			return new DevopsQuickstartResult
			{
				Repository = repository,
				Pipelines = pipelines
			};
		}

		private async Task<Repository> CreateRepository()
		{
			await _retryService.Retry(async () =>
			{
				var request = _interactiveService.GetCreateRepositoryRequest();
				
				_interactiveService.ShowMessage($"Creating repository '{request.Name}'");
				_interactiveService.Repository = await _devopsService.CreateRepository(request);
				_interactiveService.ShowMessage($"Created Repository '{_interactiveService.Repository.Name}': {_interactiveService.Repository.WebUrl}");
			});
			
			return _interactiveService.Repository;
		}

		private void PushCode(Repository repository)
		{
			if (!_interactiveService.ShouldPushCodeNow())
			{
				return;
			}
			
			_retryService.Retry(() =>
			{
				_interactiveService.ShowMessage($"Pushing code to '{repository.Name}'");
				_gitService.PushToDevopsRepository(repository);
			});
		}

		private async Task<List<Pipeline>> CreatePipelines()
		{
			var ymlFiles = GetYmlFiles();
			
			if (!ymlFiles.Any() || !_interactiveService.ShouldCreatePipelinesNow())
			{
				return new List<Pipeline>();
			}

			var pipelines = new List<Pipeline>();

			while (true)
			{
				var createPipelineRequest = _interactiveService.GetCreatePipelineRequest(ymlFiles);

				if (createPipelineRequest is null)
				{
					break;
				}
				
				await _retryService.Retry(async () =>
				{
					_interactiveService.ShowMessage($"Creating pipeline '{createPipelineRequest.Name}' from '{createPipelineRequest.Configuration.Path}'");
					var pipeline = await _devopsService.CreatePipeline(_interactiveService.Repository, createPipelineRequest);
					pipelines.Add(pipeline);
					
					_interactiveService.ShowMessage($"Created Pipeline '{pipeline.Name}': {pipeline.Links.Web.Href}");
				});
			}
			
			return pipelines;
		}
		
		private static List<string> GetYmlFiles()
		{
			var directory = Directory.GetCurrentDirectory();
			return Directory.GetFiles(directory, "*.yml")
				.Select(path => path.Replace(directory, ""))
				.ToList();
		}
	}
}