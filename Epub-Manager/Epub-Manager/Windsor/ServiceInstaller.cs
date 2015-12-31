using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Epub_Manager.Core.Services;
using Epub_Manager.Services;
using Epub_Manager.Windsor.Facilities;

namespace Epub_Manager.Windsor
{
    public class ServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IExceptionHandler>().ImplementedBy<ExceptionHandler>().LifestyleSingleton(),
                Component.For<ILoadingService>().ImplementedBy<LoadingService>().LifestyleSingleton(),
                Component.For<IMessageManager>().ImplementedBy<MessageManager>().LifestyleSingleton(),
                Component.For<IEpubService>().ImplementedBy<EpubService>().LifestyleSingleton(),
                Component.For<IRenameService>().ImplementedBy<RenameService>().LifestyleSingleton()
                );

            container.AddFacility<MakeEpubExceptionFacility>();
        }
    }
}