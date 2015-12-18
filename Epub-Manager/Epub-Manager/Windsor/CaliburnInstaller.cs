using Caliburn.Micro;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Epub_Manager.Windows;
using Epub_Manager.Windsor.Facilities;

namespace Epub_Manager.Windsor
{
    public class CaliburnInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For<IWindowManager>().ImplementedBy<EpubWindowManager>().LifestyleSingleton(),
                Component.For<IEventAggregator>().ImplementedBy<EventAggregator>().LifestyleSingleton());

            container.AddFacility<EventRegistrationFacility>();

        }
    }
}