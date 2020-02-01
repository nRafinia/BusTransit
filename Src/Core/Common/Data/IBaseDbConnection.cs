using System;
using Common.Containers;
using Raven.Client.Documents;

namespace Common.Data
{
    public interface IBaseDbConnection<out T> : ISingleton//, IDisposable
    {
        T Connection { get; }
    }
}