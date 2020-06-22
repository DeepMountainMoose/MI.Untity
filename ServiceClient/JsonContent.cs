using Microsoft.Extensions.ObjectPool;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ServiceClient
{
    /// <summary>
    ///     将对象转化为Json内容
    /// </summary>
    public class JsonContent: HttpContent
    {
        private static readonly Lazy<JsonSerializer> DefaultJsonSerializer = new Lazy<JsonSerializer>(JsonSerializer.CreateDefault);

        private PooledStream _pooledStream;

        /// <summary>
        ///     Ctor
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="jsonSerializer"></param>
        public JsonContent(object obj, JsonSerializer jsonSerializer = null)
        {
            _pooledStream = PooledStream.Acquire();

            (jsonSerializer ?? DefaultJsonSerializer.Value)
                .Serialize(_pooledStream.StreamWriter, obj);

            _pooledStream.StreamWriter.Flush();

            Headers.ContentType = new MediaTypeHeaderValue("application/json");
        }

        /// <inheritdoc />
        protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
        {
            if (!_pooledStream.MemoryStream.TryGetBuffer(out var buffer))
                throw new InvalidOperationException("No Buffer Found");

            return stream.WriteAsync(buffer.Array, 0, buffer.Count);
        }

        /// <inheritdoc />
        protected override bool TryComputeLength(out long length)
        {
            length = _pooledStream.MemoryStream.Length;
            return true;
        }

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if (disposing && _pooledStream != null)
            {
                PooledStream.Return(_pooledStream);
                _pooledStream = null;
            }

            base.Dispose(disposing);
        }

        private class PooledStream
        {
            private static readonly ObjectPool<PooledStream> Pool
                = new DefaultObjectPool<PooledStream>(new PooledStreamPolicy(), 256);

            public readonly StreamWriter StreamWriter;
            public readonly MemoryStream MemoryStream;

            private PooledStream()
            {
                MemoryStream = new MemoryStream();
                StreamWriter = new StreamWriter(MemoryStream);
            }

            public static PooledStream Acquire()
            {
                return Pool.Get();
            }

            public static void Return(PooledStream pooledStream)
            {
                pooledStream.MemoryStream.SetLength(0);
                Pool.Return(pooledStream);
            }

            private class PooledStreamPolicy : PooledObjectPolicy<PooledStream>
            {
                public override PooledStream Create()
                {
                    return new PooledStream();
                }

                public override bool Return(PooledStream obj)
                {
                    obj.MemoryStream.SetLength(0);
                    return true;
                }
            }
        }
    }
}
