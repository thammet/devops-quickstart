using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevopsQuickstart.Models.Devops;
using InteractiveConsole;

namespace DevopsQuickstart.Services
{
	public interface IInteractiveService
	{
		List<Project> Projects { get; set; }
		Repository Repository { get; set; }
		CreateRepositoryRequest GetCreateRepositoryRequest();
		bool ShouldPushCodeNow();
		bool ShouldCreatePipelinesNow();
		List<CreatePipelineRequest> GetCreatePipelineRequests();

		void ShowMessage(string message);
		void ShowError(string message);
	}

	public class CliService : IInteractiveService
	{
		public List<Project> Projects { get; set; }
		public Repository Repository { get; set; }

		public CreateRepositoryRequest GetCreateRepositoryRequest()
		{
			var selectedProject = PromptToSelectProject();
			var repositoryName = Input.Get($"Repository name for '{selectedProject.Name}'");
				
			return new CreateRepositoryRequest
			{
				Name = repositoryName,
				Project = new CreateRepositoryRequest.CreateRepositoryRequestProject
				{
					Id = selectedProject.Id
				}
			};
		}

		public List<CreatePipelineRequest> GetCreatePipelineRequests()
		{
			var ymlFiles = GetYmlFiles();
			var requests = new List<CreatePipelineRequest>();
			
			if (!ymlFiles.Any())
			{
				return requests;
			}
			
			var menu = new Menu<string>();

			foreach (var ymlFile in ymlFiles)
			{
				menu = menu.AddOption(ymlFile);
			}
			
			while (true)
			{
				var ymlFile = menu.Get("Select yml file to create a pipeline for", true);

				if (ymlFile is null)
				{
					return requests;
				}
				
				requests.Add(new CreatePipelineRequest
				{
					Name = Input.Get($"Pipeline name for '{ymlFile}'"),
					Configuration = new CreatePipelineRequest.CreatePipelineRequestConfiguration
					{
						Path = ymlFile,
						Type = "yaml",
						Repository = new CreatePipelineRequest.CreatePipelineRequestConfiguration.CreatePipelineRequestConfigurationRepository
						{
							Id = Repository.Id, 
							Name = Repository.Name,
							Type = "azureReposGit"
						}
					}
				});
			}
		}

		public bool ShouldPushCodeNow()
		{
			return Prompt.Get($"Do you want to push code to '{Repository.Name}' now?");
		}

		public bool ShouldCreatePipelinesNow()
		{
			return Prompt.Get($"Do you want to create pipelines for '{Repository.Name}' now?");
		}

		private Project PromptToSelectProject()
		{
			var menu = new Menu<Project>();

			foreach (var project in Projects)
			{
				menu = menu.AddOption(project.Name, project);
			}

			return menu.Get("Select a DevOps project");
		}

		private static List<string> GetYmlFiles()
		{
			var directory = Directory.GetCurrentDirectory();
			return Directory.GetFiles(directory, "*.yml")
				.Select(path => path.Replace(directory, ""))
				.ToList();
		}

		public void ShowMessage(string message)
		{
			Console.WriteLine(message);
		}

		public void ShowError(string message)
		{
			var originalForegroundColor = Console.ForegroundColor;
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine(message);
			Console.ForegroundColor = originalForegroundColor;
		}
	}
}