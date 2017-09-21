using MeetMeWeb.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace MeetMeWeb.Tests.Mocks
{
    public sealed class DbSetMock<T> : IDbSet<T>, IQueryable, IEnumerable<T>, IDbAsyncEnumerable<T> where T : class
    {
        readonly ObservableCollection<T> _localData;

        readonly ObservableCollection<T> _data;
        IQueryable _query;

        public DbSetMock()
        {
            _data = new ObservableCollection<T>();
            _localData = new ObservableCollection<T>();
            _query = _data.AsQueryable();
        }

        public Type ElementType
        {
            get
            {
                return _data.First().GetType();
            }
        }

        public Expression Expression
        {
            get
            {
                return _query.Expression;
            }
        }

        public ObservableCollection<T> Local
        {
            get
            {

                return _localData;
            }
        }

        public IQueryProvider Provider
        {
            get
            {
                return new TestDbAsyncQueryProvider<T>(_query.Provider);
            }
        }

        public T Add(T entity)
        {
            _data.Add(entity);
            _localData.Add(entity);
            return entity;
        }

        public T Attach(T entity)
        {
            _data.Add(entity);
            _localData.Add(entity);
            return entity;
        }

        public T Create()
        {
            return Activator.CreateInstance<T>();
        }

        public T Find(params object[] keyValues)
        {
            foreach (object keyValue in keyValues)
            {
                if (_data.First().GetType() == typeof(User))
                {
                    foreach (T user in _data)
                    {
                        User current = user as User;
                        if (current.Id == keyValue as string)
                        {
                            return current as T;
                        }
                    }
                }
            }

            return null;

        }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new TestDbAsyncEnumerator<T>(_data.GetEnumerator());
        }

        public IEnumerator<T> GetEnumerator()
        {
          return _data.GetEnumerator();
        }

        public T Remove(T entity)
        {
            _data.Remove(entity);
            _localData.Remove(entity);
            return entity;
        }

        TDerivedEntity IDbSet<T>.Create<TDerivedEntity>()
        {
            throw new NotImplementedException();
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }

    class TestDbAsyncQueryProvider<T> : IDbAsyncQueryProvider
    {
        readonly IQueryProvider _inner;

        internal TestDbAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestDbAsyncEnumerable<T>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestDbAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public Task<object> ExecuteAsync(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute(expression));
        }

        public Task<TResult> ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken)
        {
            return Task.FromResult(Execute<TResult>(expression));
        }
    }

    public class TestDbAsyncEnumerable<T> : EnumerableQuery<T>, IDbAsyncEnumerable<T>, IQueryable<T>
    {
        public TestDbAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestDbAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IDbAsyncEnumerator<T> GetAsyncEnumerator()
        {
            return new TestDbAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator()
        {
            return GetAsyncEnumerator();
        }

        IQueryProvider IQueryable.Provider
        {
            get { return new TestDbAsyncQueryProvider<T>(this); }
        }
    }

    class TestDbAsyncEnumerator<T> : IDbAsyncEnumerator<T>
    {
        readonly IEnumerator<T> _inner;

        public TestDbAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public void Dispose()
        {
            _inner.Dispose();
        }

        public Task<bool> MoveNextAsync(CancellationToken cancellationToken)
        {
            return Task.FromResult(_inner.MoveNext());
        }

        public T Current
        {
            get { return _inner.Current; }
        }

        object IDbAsyncEnumerator.Current
        {
            get { return Current; }
        }
    }

}