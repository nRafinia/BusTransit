using System;
using ANA.Common.Containers;
using Mapster;

namespace ANA.Common.Mappers
{
    public interface IMapper
    {
        TTarget Map<TSource, TTarget>(TSource source, TTarget target);
        TTarget Map<TTarget>(object source);
        TypeAdapterSetter<TSource, TTarget> Bind<TSource, TTarget>();
    }
}