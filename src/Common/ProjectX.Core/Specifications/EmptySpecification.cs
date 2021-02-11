using LinqSpecs;
using System;
using System.Linq.Expressions;

namespace ProjectX.Core.Specifications
{
    public class EmptySpecification<T> : Specification<T>
    {
        public override Expression<Func<T, bool>> ToExpression()
            => e => true;
    }
}
