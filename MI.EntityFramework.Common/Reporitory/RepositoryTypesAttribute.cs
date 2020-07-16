using MI.Core.Domain.Repositories;
using System;

namespace MI.EntityFramework.Common.Reporitory
{
    /// <summary>
    /// 仓储类型声明
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class RepositoryTypesAttribute : Attribute
    {
        /// <summary>
        ///     声明DbContext所关联的仓储
        /// </summary>
        /// <param name="repositoryInterface">其主键被定义为int的默认仓储 </param>
        /// <param name="repositoryInterfaceWithPrimaryKey">自定义主键的仓储 </param>
        /// <param name="repositoryImplementation">
        ///     仓储的实现, 其应该继承自 <paramref name="repositoryInterface" />
        /// </param>
        /// <param name="repositoryImplementationWithPrimaryKey">
        ///     仓储的实现, 其应该继承自 
        ///     <paramref name="repositoryInterfaceWithPrimaryKey" />
        /// </param>
        public RepositoryTypesAttribute(
            Type repositoryInterface,
            Type repositoryInterfaceWithPrimaryKey,
            Type repositoryImplementation,
            Type repositoryImplementationWithPrimaryKey)
        {
            RepositoryInterface = repositoryInterface;
            RepositoryInterfaceWithPrimaryKey = repositoryInterfaceWithPrimaryKey;
            RepositoryImplementation = repositoryImplementation;
            RepositoryImplementationWithPrimaryKey = repositoryImplementationWithPrimaryKey;
        }

        /// <summary>
        ///     声明DbContext所关联的仓储
        /// </summary>
        /// <param name="repositoryImplementation">
        ///     仓储的实现, 其应该继承自 <see cref="IRepository{TEntity}" />
        /// </param>
        /// <param name="repositoryImplementationWithPrimaryKey">
        ///     仓储的实现, 其应该继承自
        ///     <see cref="IRepository" />
        /// </param>
        public RepositoryTypesAttribute(Type repositoryImplementation,
            Type repositoryImplementationWithPrimaryKey)
        {
            RepositoryInterface = typeof(IRepository<>);
            RepositoryInterfaceWithPrimaryKey = typeof(IRepository<,>);
            RepositoryImplementation = repositoryImplementation;
            RepositoryImplementationWithPrimaryKey = repositoryImplementationWithPrimaryKey;
        }

        /// <summary>
        ///     默认主键为int的仓储接口定义
        /// </summary>
        public Type RepositoryInterface { get; private set; }

        /// <summary>
        ///     自定义主键的仓储接口定义
        /// </summary>
        public Type RepositoryInterfaceWithPrimaryKey { get; private set; }

        /// <summary>
        ///     默认主键为int的仓储实现
        /// </summary>
        public Type RepositoryImplementation { get; private set; }

        /// <summary>
        ///     自定义主键的仓储实现
        /// </summary>
        public Type RepositoryImplementationWithPrimaryKey { get; private set; }
    }
}
