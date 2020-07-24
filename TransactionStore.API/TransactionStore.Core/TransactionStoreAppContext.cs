using Autofac;

namespace TransactionStore.Core
{
    public static class TransactionStoreAppContext
    {
        public static IContainer Container { get; private set; } = (new ContainerBuilder()).Build();
        public static void SetContainer(IContainer container)
        {
            Container = container;
        }
        public static void BuildContainer(ContainerBuilder containerBuilder)
        {
            Container = containerBuilder.Build();
        }
        public static T GetService<T>() => Container.Resolve<T>();
    }
}
