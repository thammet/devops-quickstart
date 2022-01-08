using Autofac;

namespace DevopsQuickstart
{
	public class DevopsQuickStartModule : Module
	{
		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterAssemblyTypes(ThisAssembly).AsImplementedInterfaces();
		}
	}
}