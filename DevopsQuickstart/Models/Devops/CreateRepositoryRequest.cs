using Newtonsoft.Json;

namespace DevopsQuickstart.Models.Devops
{
	public class CreateRepositoryRequest
	{
		[JsonProperty("name")]
		public string Name { get; set; }
		
		[JsonProperty("project")]
		public CreateRepositoryRequestProject Project { get; set; }
		
		public class CreateRepositoryRequestProject
		{
			[JsonProperty("id")]
			public string Id { get; set; }
		}
	}
}