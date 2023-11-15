using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;

namespace Acme.IdentityServer.WebApi.Tests {
    public class TestAsyncQueryProvider<TEntity> : IAsyncQueryProvider {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner) {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression) {
            return new TestAsyncEnumerable<TEntity>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression) {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression) {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression) {
            return _inner.Execute<TResult>(expression);
        }

        public IAsyncEnumerable<TResult> ExecuteAsync<TResult>(Expression expression) {
            return new TestAsyncEnumerable<TResult>(expression);
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) {
            return Task.FromResult(Execute<TResult>(expression));
        }

        TResult IAsyncQueryProvider.ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken) {
            var expectedResultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = ((IQueryProvider)this).Execute(expression);

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                .MakeGenericMethod(expectedResultType)
                .Invoke(null, new[] { executionResult });
        }
    }

    public class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T> {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable) { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression) { }

        public IAsyncEnumerator<T> GetEnumerator() {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default) {
            throw new System.NotImplementedException();
        }
    }

    public class TestAsyncEnumerator<T> : IAsyncEnumerator<T> {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner) {
            _inner = inner;
        }

        public void Dispose() {
            _inner.Dispose();
        }

        public Task<bool> MoveNext(CancellationToken cancellationToken) {
            return Task.FromResult(_inner.MoveNext());
        }

        public ValueTask<bool> MoveNextAsync() {
            throw new System.NotImplementedException();
        }

        public ValueTask DisposeAsync() {
            throw new System.NotImplementedException();
        }

        public T Current {
            get { return _inner.Current; }
        }
    }
}
