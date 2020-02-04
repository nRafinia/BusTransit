using System;
using Common.Containers;
using Raven.Client.Documents;

namespace Common.Data
{
    public interface IBaseDbConnection<out T> : ITransient//, IDisposable
    {
        T Connection { get; }
    }
}