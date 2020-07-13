using System;
namespace TransactionStore.Data
{
    public class DataWrapper<T>
    {
        public T Data;
        public bool WasWithoutExceptions;
        public string ExceptionMessage;

        public DataWrapper()
        {

        }


    }
}
