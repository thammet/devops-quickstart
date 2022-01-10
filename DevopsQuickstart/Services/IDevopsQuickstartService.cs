using System.Collections.Generic;
using System.Threading.Tasks;
using DevopsQuickstart.Models.Devops;

namespace DevopsQuickstart.Services
{
	public interface IDevopsQuickstartService
	{
		Task QuickStart();
	}

	public class DevopsQuickstartService : IDevopsQuickstartService
	{
		private readonly IInteractiveService _interactiveService;
		private readonly IDevopsService _devopsService;
		private readonly IGitService _gitService;

		public DevopsQuickstartService(IInteractiveService interactiveService, IDevopsService devopsService, IGitService gitService)
		{
			_interactiveService = interactiveService;
			_devopsService = devopsService;
			_gitService = gitService;
		}

		public async Task QuickStart()
		{
			_interactiveService.Projects = await _devopsService.GetAvailableProjects();

			var repository = await CreateRepository();
			var pipelines = await CreatePipelines();
			
			_interactiveService.ShowMessage($"Created Repository '{repository.Name}': {repository.WebUrl}");
			foreach (var pipeline in pipelines)
			{
				_interactiveService.ShowMessage($"Created Pipeline '{pipeline.Name}': {pipeline.Links.Web.Href}");
			}
		}

		private async Task<Repository> CreateRepository()
		{
			_gitService.CreateRepository();
			
			_interactiveService.Repository = await _devopsService.CreateRepository(_interactiveService.GetCreateRepositoryRequest());

			if (_interactiveService.ShouldPushCodeNow())
			{
				_gitService.PushToDevopsRepository(_interactiveService.Repository);
			}

			return _interactiveService.Repository;
		}

		private async Task<List<Pipeline>> CreatePipelines()
		{
			if (!_interactiveService.ShouldCreatePipelinesNow())
			{
				return new List<Pipeline>();
			}
			
			var createPipelineRequests = _interactiveService.GetCreatePipelineRequests();
			var pipelines = new List<Pipeline>();

			foreach (var createPipelineRequest in createPipelineRequests)
			{
				var pipeline = await _devopsService.CreatePipeline(_interactiveService.Repository, createPipelineRequest);
				pipelines.Add(pipeline);
			}

			return pipelines;
		}
	}
}