using Caliburn.Micro;
using Castle.Core;
using Castle.MicroKernel.Facilities;

namespace Epub_Manager.Windsor.Facilities
{
    /// <summary>
    /// A castle windsor facility that automatically registers Caliburn.Micro event handler instances.
    /// </summary>
    public class EventRegistrationFacility : AbstractFacility
    {
        /// <summary>
        /// The custom initialization for the Facility.
        /// </summary>
        protected override void Init()
        {
            this.Kernel.ComponentCreated += this.ComponentCreated;
        }
        /// <summary>
        /// Executed when a component was created.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="instance">The instance.</param>
        private void ComponentCreated(ComponentModel model, object instance)
        {
            if (instance is IHandle == false)
                return;

            var eventAggregator = this.Kernel.Resolve<IEventAggregator>();
            eventAggregator.Subscribe(instance);
        }
    }
}
