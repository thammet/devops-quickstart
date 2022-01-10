using Newtonsoft.Json;

namespace DevopsQuickstart.Models.Devops
{
	public class Pipeline
	{
		[JsonProperty("_links")]
		public PipelineLinks Links { get; set; }

		[JsonProperty("name")]
		public string Name { get; set; }
		
		public class PipelineLinks
		{
			public Link Web { get; set; }
		
			public class Link
			{
				public string Href { get; set; }
			}
		}
	}
}