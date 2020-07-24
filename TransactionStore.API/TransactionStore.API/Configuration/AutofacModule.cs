using Autofac;
using TransactionStore.Data;
using TransactionStore.Business;

namespace TransactionStore.API.Configuration
{
    public class AutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TransactionRepository>().As<ITransactionRepository>();
            builder.RegisterType<TransactionService>().As<ITransactionService>();
        }
    }
}
