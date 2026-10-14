using Autofac;
using System.Reflection;

namespace Carglass.TechnicalAssessment.Backend.BL;

public class Module : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        RegisterApplicationServices(builder);
    }

    private static void RegisterApplicationServices(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly())
            .Where(x => !x.IsAbstract && !x.IsInterface && x.Name.EndsWith("AppService"))
            .AsImplementedInterfaces()
            .InstancePerDependency();
    }
}