using System.Runtime.ConstrainedExecution;

namespace MI.Core.Dependency
{
    internal class DisposableDependencyObjectWrapper : DisposableDependencyObjectWrapper<object>, IDisposableDependencyObjectWrapper
    {
        public DisposableDependencyObjectWrapper(IIocResolver iocResolver, object obj)
            : base(iocResolver, obj)
        {

        }
    }

    internal class DisposableDependencyObjectWrapper<T> : CriticalFinalizerObject, IDisposableDependencyObjectWrapper<T>
    {
        private readonly IIocResolver _iocResolver;

        public T Object { get; private set; }

        public DisposableDependencyObjectWrapper(IIocResolver iocResolver, T obj)
        {
            _iocResolver = iocResolver;
            Object = obj;
        }

        public void Dispose()
        {
            _iocResolver.Release(Object);
        }

        ~DisposableDependencyObjectWrapper()
        {
            Dispose();
        }
    }
}
