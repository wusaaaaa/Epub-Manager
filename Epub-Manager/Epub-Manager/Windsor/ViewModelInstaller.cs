using Caliburn.Micro;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Epub_Manager.Extensions;
using Epub_Manager.Views.Shell;

namespace Epub_Manager.Windsor
{
    public class ViewModelInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Classes
                .FromThisAssembly()
                .BasedOn<PropertyChangedBase>()
                .WithServiceSelf()
                .LifestyleTransient()

                .ConfigureFor<IShellItem>(f =>
                    f.Forward<IShellItem>())

                .ConfigureIf(f => typeof(ConductorBaseWithActiveItem<>).IsAssignableFromGenericType(f.Implementation), f =>
                   f.PropertiesIgnore(d => d.Name == nameof(ConductorBaseWithActiveItem<object>.ActiveItem)))

                .ConfigureFor<ShellViewModel>(f =>
                    f.LifestyleSingleton())
                );
        }
    }
}