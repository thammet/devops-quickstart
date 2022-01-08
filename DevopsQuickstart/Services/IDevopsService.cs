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
		private readonly HttpClient _devopsHttpClient;

		public DevopsService(DevopsOptions devopsOptions)
		{
			_devopsHttpClient = new HttpClient();
			_devopsHttpClient.BaseAddress = new Uri(devopsOptions.OrganizationUrl);
			_devopsHttpClient.DefaultRequestHeaders.Accept.Clear();
			_devopsHttpClient.DefaultRequestHeaders.Add("User-Agent", "VstsRestApiSamples");
			_devopsHttpClient.DefaultRequestHeaders.Add("X-TFS-FedAuthRedirect", "Suppress");
			_devopsHttpClient.DefaultRequestHeaders.Add("Authorization", devopsOptions.AuthHeader);
		}
		
		public async Task<List<Project>> GetAvailableProjects()
		{
			var response = await _devopsHttpClient.GetAsync("_apis/projects?stateFilter=All&api-version=2.2");
			
			response.EnsureSuccessStatusCode();

			var responseString = await response.Content.ReadAsStringAsync();
			return JObject.Parse(responseString).GetValue("value").ToObject<List<Project>>();
		}

		public async Task<Repository> CreateRepository(CreateRepositoryRequest request)
		{
			var response = await _devopsHttpClient.PostAsync("_apis/git/repositories?api-version=6.0", GetRequestBody(request));

			response.EnsureSuccessStatusCode();
            
			return JsonConvert.DeserializeObject<Repository>(await response.Content.ReadAsStringAsync());
		}

		public async Task<Pipeline> CreatePipeline(Repository repository, CreatePipelineRequest request)
		{
			var response = await _devopsHttpClient.PostAsync($"{repository.Project.Id}/_apis/pipelines?api-version=6.0-preview.1", GetRequestBody(request));
			
			response.EnsureSuccessStatusCode();
            
			return JsonConvert.DeserializeObject<Pipeline>(await response.Content.ReadAsStringAsync());
		}

		private static StringContent GetRequestBody(object obj)
		{
			return new StringContent(JsonConvert.SerializeObject(obj), Encoding.Default, "application/json");
		}
	}
}