using System;

namespace FunctionalStuff
{
    public interface IFunctor<out T>
    {
        IFunctor<TOut> Map<TOut>(Func<T, TOut> mapping);
    }
}
