using System;
using System.Linq.Expressions;

namespace Hangfire.Pipelines.Expressions
{
    public interface IExpressionFactory
    {
        ExpressionContainer Create<T>(Expression<Action<T>> expression);
    }
}