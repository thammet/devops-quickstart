using Newtonsoft.Json;

namespace DevopsQuickstart.Models.Devops
{
	public class CreatePipelineRequest
	{
		[JsonProperty("name")]
		public string Name { get; set; }
		
		[JsonProperty("configuration")]
		public CreatePipelineRequestConfiguration Configuration { get; set; }
		
		public class CreatePipelineRequestConfiguration
		{
			[JsonProperty("type")]
			public string Type { get; set; }
			
			[JsonProperty("path")]
			public string Path { get; set; }
			
			[JsonProperty("repository")]
			public CreatePipelineRequestConfigurationRepository Repository { get; set; }
			
			public class CreatePipelineRequestConfigurationRepository
			{
				[JsonProperty("id")]
				public string Id { get; set; }
				
				[JsonProperty("name")]
				public string Name { get; set; }
				
				[JsonProperty("type")]
				public string Type { get; set; }
			}
		}
	}
}