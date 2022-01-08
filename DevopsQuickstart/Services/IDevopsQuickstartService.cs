using System.Threading.Tasks;

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

			await CreateRepository();
			await CreatePipelines();
		}

		private async Task CreateRepository()
		{
			_gitService.CreateRepository();
			
			_interactiveService.Repository = await _devopsService.CreateRepository(_interactiveService.GetCreateRepositoryRequest());
			
			_gitService.PushToDevopsRepository(_interactiveService.Repository);
			
			_interactiveService.ShowMessage($"Created Repository: {_interactiveService.Repository.WebUrl}");
		}

		private async Task CreatePipelines()
		{
			var createPipelineRequests = _interactiveService.GetCreatePipelineRequests();

			foreach (var createPipelineRequest in createPipelineRequests)
			{
				var pipeline = await _devopsService.CreatePipeline(_interactiveService.Repository, createPipelineRequest);
				_interactiveService.ShowMessage($"Created Pipeline: {pipeline.Links.Web.Href}");
			}
		}
	}
}