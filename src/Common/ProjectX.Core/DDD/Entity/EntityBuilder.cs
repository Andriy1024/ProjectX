using System;

namespace ProjectX.Core
{
    public abstract class EntityBuilder<TEntity>
        where TEntity : class
    {
        protected TEntity Entity;

        public virtual TEntity Build()
        {
            EnsureCreated();

            var result = Entity;
            Clear();

            return result;
        }

        public static implicit operator TEntity(EntityBuilder<TEntity> builder)
        {
            return builder.Build();
        }

        protected virtual void EnsureCreated()
        {
            if (Entity == null)
                throw new InvalidOperationException($"{typeof(TEntity).Name} was not created.");
        }

        protected virtual void Clear()
        {
            Entity = null;
        }
    }
}
