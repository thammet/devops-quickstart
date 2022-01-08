namespace DevopsQuickstart.Models.Devops
{
	public class Repository
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Url { get; set; }
		public string RemoteUrl { get; set; }
		public string SshUrl { get; set; }
		public string WebUrl { get; set; }
		public RepositoryProject Project { get; set; }
		
		public class RepositoryProject
		{
			public string Id { get; set; }
			public string Name { get; set; }
		}
	}
}