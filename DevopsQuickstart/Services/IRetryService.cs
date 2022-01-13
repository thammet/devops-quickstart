using System;
using System.Threading.Tasks;

namespace DevopsQuickstart.Services
{
	public interface IRetryService
	{
		void Retry(Action action, bool canCancel = true);
		Task Retry(Func<Task> func, bool canCancel = true);
	}

	public class RetryService : IRetryService
	{
		private readonly IInteractiveService _interactiveService;

		public RetryService(IInteractiveService interactiveService)
		{
			_interactiveService = interactiveService;
		}

		public void Retry(Action action, bool canCancel)
		{
			while (true)
			{
				try
				{
					action();
					return;
				}
				catch (Exception e)
				{
					_interactiveService.ShowError("An error has occured");
					_interactiveService.ShowError(e.Message);
					if (canCancel && !_interactiveService.ShouldRetry())
					{
						return;
					}
				}
			}
		}
		
		public async Task Retry(Func<Task> func, bool canCancel)
		{
			while (true)
			{
				try
				{
					await func();
					return;
				}
				catch (Exception e)
				{
					_interactiveService.ShowError("An error has occured");
					_interactiveService.ShowError(e.Message);
					if (canCancel && !_interactiveService.ShouldRetry())
					{
						return;
					}
				}
			}
		}
	}
}