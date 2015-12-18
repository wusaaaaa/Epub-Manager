using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.ModelBuilder;
using Castle.MicroKernel.Registration;
using Epub_Manager.Windsor.Interceptors;
using System.Linq;
using System.Reflection;

namespace Epub_Manager.Windsor.Facilities
{
    public class MakeEpubExceptionFacility : AbstractFacility, IContributeComponentModelConstruction
    {
        protected override void Init()
        {
            this.Kernel.Register(Component.For<MakeEpubExceptionInterceptor>().LifestyleTransient());

            this.Kernel.ComponentModelBuilder.AddContributor(this);
        }

        void IContributeComponentModelConstruction.ProcessModel(IKernel kernel, ComponentModel model)
        {
            var hasAttribute = model.Implementation
                .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Any(f => CustomAttributeExtensions.GetCustomAttribute<CatchExceptionAttribute>((MemberInfo)f) != null);

            if (hasAttribute)
            {
                model.Interceptors.Add(new InterceptorReference(typeof(MakeEpubExceptionInterceptor)));
            }
        }
    }
}