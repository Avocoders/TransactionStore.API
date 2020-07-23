using Autofac;
using TransactionStore.Data;

namespace TransactionStore.API.Configuration
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TransactionRepository>().As<ITransactionRepository>();
        }
    }
}
