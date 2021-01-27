using LinqSpecs;
using ProjectX.Common.Specifications;
using System;
using System.Linq.Expressions;

namespace ProjectX.Common
{
    public class SpecificationBuilder<TEntity>
    {
        protected Specification<TEntity> Specification;

        public SpecificationBuilder() {}

        public SpecificationBuilder(Specification<TEntity> specification)
        {
            SetSpecification(specification);
        }

        public SpecificationBuilder(Expression<Func<TEntity, bool>> expression)
        {
            SetExpression(expression);
        }

        public virtual SpecificationBuilder<TEntity> SetSpecification(Specification<TEntity> specification, LogicalOperator logicalOperator = LogicalOperator.AND)
        {
            if (Specification == null)
            {
                Specification = specification;
            }
            else
            {
                if (logicalOperator == LogicalOperator.AND)
                    Specification &= specification;

                else if (logicalOperator == LogicalOperator.OR)
                    Specification |= specification;

                else throw new Exception($"Invalid logical operator: {logicalOperator}.");
            }

            return this;
        }

        public virtual SpecificationBuilder<TEntity> SetExpression(Expression<Func<TEntity, bool>> expression)
        {
            SetSpecification(new AdHocSpecification<TEntity>(expression));
            return this;
        }

        public virtual Specification<TEntity> GetSpecification()
        {
            Specification ??= new EmptySpecification<TEntity>();
            var result = Specification;
            Clear();
            return result;
        }

        protected virtual void Clear()
        {
            Specification = null;
        }

        public static implicit operator Specification<TEntity>(SpecificationBuilder<TEntity> builder)
        {
            return builder.GetSpecification();
        }
    }
}
