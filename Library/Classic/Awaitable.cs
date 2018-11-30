using System;
using System.Runtime.CompilerServices;

namespace Microsoft.Bot.Builder.Internals.Fibers
{
    public interface IAwaiter<out T> : INotifyCompletion
    {
        bool IsCompleted { get; }

        T GetResult();
    }
}

namespace Microsoft.Bot.Builder.Dialogs
{
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder.Internals.Fibers;

    /// <summary>
    /// Explicit interface to support the compiling of async/await.
    /// </summary>
    /// <typeparam name="T">The type of the contained value.</typeparam>
    public interface IAwaitable<out T>
    {
        /// <summary>
        /// Get the awaiter for this awaitable item.
        /// </summary>
        /// <returns>The awaiter.</returns>
        Builder.Internals.Fibers.IAwaiter<T> GetAwaiter();
    }

    /// <summary>
    /// Creates a <see cref="IAwaitable{T}"/> from item passed to constructor.
    /// </summary>
    /// <typeparam name="T"> The type of the item.</typeparam>
    public sealed class AwaitableFromItem<T> : IAwaitable<T>, IAwaiter<T>
    {
        private readonly T item;

        public AwaitableFromItem(T item)
        {
            this.item = item;
        }

        bool IAwaiter<T>.IsCompleted
        {
            get { return true; }
        }

        T IAwaiter<T>.GetResult()
        {
            return item;
        }

        void INotifyCompletion.OnCompleted(Action continuation)
        {
            throw new NotImplementedException();
        }

        IAwaiter<T> IAwaitable<T>.GetAwaiter()
        {
            return this;
        }
    }

    /// <summary>
    /// Creates a <see cref="IAwaitable{T}"/> from source passed to constructor.
    /// </summary>
    /// <typeparam name="TSource"> The type of the source for build the item.</typeparam>
    /// <typeparam name="TItem">The type if the item.</typeparam>
    public sealed class AwaitableFromSource<TSource, TItem> : IAwaitable<TItem>, IAwaiter<TItem>
    {
        private readonly TSource source;
        private readonly Func<TSource, Task<TItem>> resolver;

        private bool resolved = false;
        private TItem result;

        public AwaitableFromSource(TSource source, Func<TSource, Task<TItem>> resolver)
        {
            this.source = source;
            this.resolver = resolver;
        }

        public bool IsCompleted
        {
            get
            {
                return this.resolved;
            }
        }

        public IAwaiter<TItem> GetAwaiter()
        {
            return this;
        }

        public TItem GetResult()
        {
            return this.result;
        }

        public void OnCompleted(Action continuation)
        {
            if (!resolved)
            {
                this.result = Task.Run(() => this.resolver(this.source)).Result;
                this.resolved = true;
            }

            continuation();
        }
    }

    public partial class Awaitable
    {
        /// <summary>
        /// Wraps item in a <see cref="IAwaitable{T}"/>.
        /// </summary>
        /// <typeparam name="T">Type of the item.</typeparam>
        /// <param name="item">The item that will be wrapped.</param>
        public static IAwaitable<T> FromItem<T>(T item)
        {
            return new AwaitableFromItem<T>(item);
        }

        /// <summary>
        /// Wraps source to build item as a <see cref="IAwaitable{T}"/>.
        /// </summary>
        /// <typeparam name="TSource"> The type of the source for build the item.</typeparam>
        /// <typeparam name="TItem">The type if the item.</typeparam>
        /// <param name="source">The source item that will be wrapped.</param>
        /// <param name="resolver">The resolution function from source to item.</param>
        public static IAwaitable<TItem> FromSource<TSource, TItem>(TSource source, Func<TSource, Task<TItem>> resolver)
        {
            return new AwaitableFromSource<TSource, TItem>(source, resolver);
        }
    }
}
