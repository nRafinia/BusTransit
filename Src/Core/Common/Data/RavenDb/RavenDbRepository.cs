using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Common.Extensions;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;

namespace Common.Data.RavenDb
{
    public class RavenDbRepository : IRepository
    {
        private readonly IRavenDbConnection _dbConnection;
        private readonly IAsyncDocumentSession _session;

        public RavenDbRepository(IRavenDbConnection connection)
        {
            _dbConnection = connection;
            _session = connection.Connection.OpenAsyncSession();
        }

        public void Dispose()
        {
            _session.Dispose();
        }

        public async Task SaveChanges()
        {
            await _session.SaveChangesAsync();
        }

        #region Get

        /// <inheritdoc />
        public async Task<T> Get<T>(string id)
            where T : DbEntity
        {
            return await _session.LoadAsync<T>(id);
        }

        public async Task<IEnumerable<T>> Get<T>(IEnumerable<string> ids)
            where T : DbEntity
        {
            var res = await _session.LoadAsync<T>(ids);
            return res.Select(t => t.Value);
        }

        /// <inheritdoc />
        public async Task<T> Get<T, TIncType>(string id, Expression<Func<T, string>> field,
            Expression<Func<T, TIncType>> targetField)
            where T : DbEntity
        {
            var res = await _session
                .Include(field)
                .LoadAsync<T>(id);

            var inc = await _session.LoadAsync<TIncType>(res.GetPropertyValue(field));

            res.SetPropertyValue(targetField, inc);

            return res;
        }

        /// <inheritdoc />
        public async Task<T> Get<T, TIncType>(string id, Expression<Func<T, IEnumerable<string>>> field,
            Expression<Func<T, IEnumerable<TIncType>>> targetField)
            where T : DbEntity
        {
            var res = await _session
                .Include(field)
                .LoadAsync<T>(id);

            var keys = res.GetPropertyValue(field);

            var inc = await _session.LoadAsync<TIncType>(keys);

            var v = inc.Select(p => p.Value).ToList().AsEnumerable();
            
            res.SetPropertyValue(targetField, v);
            return res;
        }

        /// <inheritdoc />
        public async Task<T> Get<T, TIncType1, TIncType2>(string id, Expression<Func<T, string>> field1,
            Expression<Func<T, TIncType1>> targetField1, Expression<Func<T, string>> field2,
            Expression<Func<T, TIncType2>> targetField2)
            where T : DbEntity
        {
            var res = await _session
                .Include(field1)
                .Include(field2)
                .LoadAsync<T>(id);

            var inc1 = await _session.LoadAsync<TIncType1>(res.GetPropertyValue(field1));
            res.SetPropertyValue(targetField1, inc1);

            var inc2 = await _session.LoadAsync<TIncType2>(res.GetPropertyValue(field2));
            res.SetPropertyValue(targetField2, inc2);

            return res;
        }

        #endregion

        #region Insert

        /// <inheritdoc />
        public async Task<T> Add<T>(T entity)
            where T : DbEntity
        {
            await _session.StoreAsync(entity);
            return entity;
        }

        /// <inheritdoc />
        public async Task Add<T>(IEnumerable<T> entities)
            where T : DbEntity
        {
            using (var bulk = _dbConnection.Connection.BulkInsert())
            {
                foreach (var entity in entities)
                {
                    await bulk.StoreAsync(entity);
                }
            }
        }

        #endregion

        #region Update

        /// <inheritdoc />
        public async Task<T> Update<T>(T entity)
            where T : DbEntity
        {
            entity.ModifiedOn = DateTime.Now;
            await _session.StoreAsync(entity, entity.Id);
            return entity;
        }

        /// <inheritdoc />
        public async Task Update<T>(IEnumerable<T> entities)
            where T : DbEntity
        {
            using (var bulk = _dbConnection.Connection.BulkInsert())
            {
                foreach (var entity in entities)
                {
                    entity.ModifiedOn = DateTime.Now;
                    await bulk.StoreAsync(entity, entity.Id);
                }
            }
        }

        /// <inheritdoc />
        public async Task<bool> Update<T, TField>(T entity, Expression<Func<T, TField>> field, TField value)
            where T : DbEntity
        {
            entity.SetPropertyValue(field, value);
            entity.SetPropertyValue(f => f.ModifiedOn, DateTime.Now);
            await Update(entity);
            return true;
        }

        /// <inheritdoc />
        public async Task<bool> Update<T, TField>(Expression<Func<T, bool>> filter,
            Expression<Func<T, TField>> field, TField value)
            where T : DbEntity
        {
            var items = await _session.Query<T>().Where(filter, true).ToListAsync();
            foreach (var item in items)
            {
                item.SetPropertyValue(field, value);
                item.SetPropertyValue(f => f.ModifiedOn, DateTime.Now);
            }

            await Update(items);

            return true;
        }

        #endregion

        #region Delete

        /// <inheritdoc />
        public void Delete<T>(string id)
            where T : DbEntity
        {
            _session.Delete(id);
        }

        /// <inheritdoc />
        public void Delete<T>(T entity)
            where T : DbEntity
        {
            _session.Delete(entity);
        }

        /// <inheritdoc />
        public async Task Delete<T>(Expression<Func<T, bool>> filter)
            where T : DbEntity
        {
            //await _dbConnection.Connection.Operations.SendAsync(new DeleteByQueryOperation<T>(filter));
            var items = await _session.Query<T>().Where(filter, true).ToListAsync();
            foreach (var item in items)
            {
                Delete(item);
            }
        }

        /// <inheritdoc />
        public async Task DeleteAll<T>()
            where T : DbEntity
        {
            var items = await _session.Query<T>().ToListAsync();
            foreach (var item in items)
            {
                Delete(item);
            }
        }

        #endregion

        #region Find

        /// <inheritdoc />
        public async Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> filter)
            where T : DbEntity
        {
            return await _session.Query<T>().Where(filter, true).ToListAsync();
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> filter,
            Expression<Func<T, object>> order,
            int pageIndex, int size)
            where T : DbEntity
        {
            return await Find(filter, order, pageIndex, size, false);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> filter, int pageIndex, int size)
            where T : DbEntity
        {
            return await Find(filter, f => f.Id, pageIndex, size);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> filter,
            Expression<Func<T, object>> order,
            int pageIndex, int size, bool isDescending)
            where T : DbEntity
        {
            var items = _session.Query<T>().Where(filter, true);
            var oItems = !isDescending ? LinqExtensions.OrderBy(items, order) : LinqExtensions.OrderByDescending(items, order);
            return await oItems
                .Skip(pageIndex * size)
                .Take(size)
                .ToListAsync();
        }

        #endregion

        #region Util

        /// <inheritdoc />
        public async Task<long> Count<T>()
            where T : DbEntity
        {
            return await _session.Query<T>().CountAsync();
        }

        /// <inheritdoc />
        public async Task<long> Count<T>(Expression<Func<T, bool>> filter)
            where T : DbEntity
        {
            return await _session.Query<T>().CountAsync(filter);
        }

        /// <inheritdoc />
        public async Task<bool> Any<T>()
            where T : DbEntity
        {
            return await _session.Query<T>().AnyAsync();
        }

        /// <inheritdoc />
        public async Task<bool> Any<T>(Expression<Func<T, bool>> filter)
            where T : DbEntity
        {
            return await _session.Query<T>().AnyAsync(filter);
        }

        #endregion

        #region Max/Min

        /// <inheritdoc />
        public async Task<T> Max<T>(Expression<Func<T, bool>> filter, Expression<Func<T, object>> field)
            where T : DbEntity
        {
            return await _session.Query<T>()
                .Where(filter, true)
                .OrderByDescending(field)
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public async Task<T> Max<T>(Expression<Func<T, object>> field)
            where T : DbEntity
        {
            return await Max(f => true, field);
        }

        /// <inheritdoc />
        public async Task<T> Min<T>(Expression<Func<T, bool>> filter, Expression<Func<T, object>> field)
            where T : DbEntity
        {
            return await _session.Query<T>()
                .Where(filter, true)
                .OrderBy(field)
                .FirstOrDefaultAsync();
        }

        /// <inheritdoc />
        public async Task<T> Min<T>(Expression<Func<T, object>> field)
            where T : DbEntity
        {
            return await Min(f => true, field);
        }

        #endregion
    }
}