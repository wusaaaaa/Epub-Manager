using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Epub_Manager.Windsor
{
    public class ArrayInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var collectionResolver = new CollectionResolver(container.Kernel, allowEmptyCollections: true);
            container.Kernel.Resolver.AddSubResolver(collectionResolver);
        }
    }
}