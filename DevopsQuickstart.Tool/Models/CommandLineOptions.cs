using CommandLine;

namespace DevopsQuickstart.Tool.Models
{
	public class CommandLineOptions
	{
		[Option('t', "tenantid", Required = true)]
		public string TenantId { get; set; }
		
		[Option('c', "clientid", Required = true)]
		public string ClientId { get; set; }
		
		[Option('o', "organizationurl", Required = true)]
		public string OrganizationUrl { get; set; }
	}
}