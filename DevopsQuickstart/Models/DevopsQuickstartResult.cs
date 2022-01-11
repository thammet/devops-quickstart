using System.Collections.Generic;
using DevopsQuickstart.Models.Devops;

namespace DevopsQuickstart.Models
{
	public class DevopsQuickstartResult
	{
		public Repository Repository { get; set; }
		public List<Pipeline> Pipelines { get; set; }
	}
}