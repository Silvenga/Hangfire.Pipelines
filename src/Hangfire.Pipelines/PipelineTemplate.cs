using System;
using System.Collections.Generic;
using System.Linq.Expressions;

using Hangfire.Annotations;
using Hangfire.Pipelines.Models;

namespace Hangfire.Pipelines
{
    public class PipelineTemplate<T>
    {
        //public IList<LazyInvoker<T, IPipelineTask<T>>> Collection { get; } = new List<LazyInvoker<T, IPipelineTask<T>>>();

        //public void AddStep<TInvoker>([NotNull, InstantHandle] Expression<Action<TInvoker>> methodCall) where TInvoker : IPipelineTask<T>
        //{
        //    var i = new LazyInvoker<T, TInvoker>
        //    {
        //        Expression = methodCall,
        //        Invoker = typeof(TInvoker)
        //    };
        //    Collection.Add(i);
        //}
    }

    //public class LazyInvoker<T, TInvoker> where TInvoker : IPipelineTask<T>
    //{
    //    public Type Invoker { get; set; }

    //    public Expression<Action<TInvoker>> Expression { get; set; }
    //}
}