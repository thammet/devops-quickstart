using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DevopsQuickstart.Models.Devops;
using DevopsQuickstart.Models.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DevopsQuickstart.Services
{
	public interface IDevopsService
	{
		Task<List<Project>> GetAvailableProjects();
		Task<Repository> CreateRepository(CreateRepositoryRequest request);
		Task<Pipeline> CreatePipeline(Repository repository, CreatePipelineRequest request);
	}

	public class DevopsService : IDevopsService
	{
		protected readonly HttpClient DevopsHttpClient;

		public DevopsService(DevopsOptions devopsOptions)
		{
			DevopsHttpClient = new HttpClient();
			DevopsHttpClient.BaseAddress = new Uri(devopsOptions.OrganizationUrl);
			DevopsHttpClient.DefaultRequestHeaders.Accept.Clear();
			DevopsHttpClient.DefaultRequestHeaders.Add("User-Agent", "VstsRestApiSamples");
			DevopsHttpClient.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
			DevopsHttpClient.DefaultRequestHeaders.Add("Authorization", devopsOptions.AuthHeader);
		}
		
		public async Task<List<Project>> GetAvailableProjects()
		{
			var response = await DevopsHttpClient.GetAsync("_apis/projects?stateFilter=All&api-version=2.2");
			
			response.EnsureSuccessStatusCode();

			var responseString = await response.Content.ReadAsStringAsync();
			return JObject.Parse(responseString).GetValue("value").ToObject<List<Project>>();
		}

		public async Task<Repository> CreateRepository(CreateRepositoryRequest request)
		{
			var response = await DevopsHttpClient.PostAsync("_apis/git/repositories?api-version=6.0", GetRequestBody(request));

			response.EnsureSuccessStatusCode();
            
			return JsonConvert.DeserializeObject<Repository>(await response.Content.ReadAsStringAsync());
		}

		public async Task<Pipeline> CreatePipeline(Repository repository, CreatePipelineRequest request)
		{
			var response = await DevopsHttpClient.PostAsync($"{repository.Project.Id}/_apis/pipelines?api-version=6.0-preview.1", GetRequestBody(request));
			
			response.EnsureSuccessStatusCode();
            
			return JsonConvert.DeserializeObject<Pipeline>(await response.Content.ReadAsStringAsync());
		}

		private static StringContent GetRequestBody(object obj)
		{
			return new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");
		}
	}
}