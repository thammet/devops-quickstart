namespace DevopsQuickstart.Models.Options
{
	public class DevopsOptions
	{
		private readonly string _organizationUrl;

		public string OrganizationUrl
		{
			get => _organizationUrl;
			init
			{
				if (!value.EndsWith("/"))
				{
					value += "/";
				}

				_organizationUrl = value;
			}
		}
		public string AuthHeader { get; init; }
	}
}