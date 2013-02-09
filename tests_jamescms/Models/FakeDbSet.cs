using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data;
using System.Data.Entity;

namespace tests_jamescms.Models
{

    //see https://github.com/khalidabuhakmeh/NoRepositories

    public class FakeDbSet<T> : IDbSet<T> where T : class
    {
        private readonly List<T> list = new List<T>();

        public Func<CancellationToken, object[], Task<T>> FindAsyncResult { get; set; }
        public Func<object[], T> FindResult { get; set; }

        public FakeDbSet()
        {
            list = new List<T>();
        }

        public FakeDbSet(IEnumerable<T> contents)
        {
            this.list = contents.ToList();
        }

        #region IDbSet<T> Members

        public Task<T> FindAsync(CancellationToken cancellationToken, params object[] keyValues)
        {
            if (FindAsyncResult != null)
                return FindAsyncResult.Invoke(cancellationToken, keyValues);

            return Task.Factory.StartNew(() => default(T));
        }

        public T Add(T entity)
        {
            this.list.Add(entity);
            return entity;
        }

        public T Attach(T entity)
        {
            this.list.Add(entity);
            return entity;
        }

        public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
        {
            throw new NotImplementedException();
        }

        public T Create()
        {
            throw new NotImplementedException();
        }

        public T Find(params object[] keyValues)
        {
            if (FindResult != null)
                return FindResult.Invoke(keyValues);

            return default(T);
        }

        public System.Collections.ObjectModel.ObservableCollection<T> Local
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public T Remove(T entity)
        {
            this.list.Remove(entity);
            return entity;
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        #endregion

        #region IQueryable Members

        public Type ElementType
        {
            get { return this.list.AsQueryable().ElementType; }
        }

        public System.Linq.Expressions.Expression Expression
        {
            get { return this.list.AsQueryable().Expression; }
        }

        public IQueryProvider Provider
        {
            get { return this.list.AsQueryable().Provider; }
        }

        #endregion
    }
}
