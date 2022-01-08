using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DevopsQuickstart.Models.Devops;

namespace DevopsQuickstart.Services
{
	public interface IInteractiveService
	{
		List<Project> Projects { get; set; }
		Repository Repository { get; set; }
		CreateRepositoryRequest GetCreateRepositoryRequest();
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
			var repositoryName = PromptForRepositoryName();
				
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
			
			while (true)
			{
				for (var i = 0; i < ymlFiles.Count; i++)
				{
					Console.WriteLine($"{i}: {ymlFiles[i]}");
				}

				var text = Prompt("Select the yml files to create a pipeline for by entering the index number and then a name. Enter 'exit' after selecting all desired files");
				if (text == "exit")
				{
					return requests;
				}

				if (!int.TryParse(text, out var indexNumber) || indexNumber < 0 || indexNumber >= ymlFiles.Count)
				{
					continue;
				}

				requests.Add(new CreatePipelineRequest
				{
					Name = PromptForPipelineName(),
					Configuration = new CreatePipelineRequest.CreatePipelineRequestConfiguration
					{
						Path = ymlFiles[indexNumber],
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

		private Project PromptToSelectProject()
		{
			for (var i = 0; i < Projects.Count; i++)
			{
				Console.WriteLine($"{i}: {Projects[i].Name} - {Projects[i].Id}");
			}
            
			Console.Write("Select the project to create the repository under by entering the index number: ");
				
			var indexNumber = int.Parse(Console.ReadLine());

			return Projects[indexNumber];
		}

		private static string PromptForRepositoryName()
		{
			return Prompt("Repository name");
		}

		private static string PromptForPipelineName()
		{
			return Prompt("Pipeline name");
		}

		private static string Prompt(string text)
		{
			Console.Write($"{text}: ");
			return Console.ReadLine();
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