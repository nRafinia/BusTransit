﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
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
        public Task<T> Get<T>(object id, CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            return _session.LoadAsync<T>((string) id, cancellationToken);
        }

        public async Task<IEnumerable<T>> Get<T>(IEnumerable<object> ids, CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            var res = await _session.LoadAsync<T>((IEnumerable<string>) ids, cancellationToken);
            return res.Select(t => t.Value);
        }

        /// <inheritdoc />
        public async Task<T> Get<T, TIncType>(object id, Expression<Func<T, string>> field,
            Expression<Func<T, TIncType>> targetField, CancellationToken cancellationToken = default)
            where T : DbEntity
            where TIncType : DbEntity
        {
            var res = await _session
                .Include(field)
                .LoadAsync<T>((string) id, cancellationToken);

            var inc = await _session.LoadAsync<TIncType>(res.GetPropertyValue(field), cancellationToken);

            res.SetPropertyValue(targetField, inc);

            return res;
        }

        /// <summary>
        /// Get document by id and include related documents
        /// </summary>
        /// <typeparam name="T">Type of document</typeparam>
        /// <typeparam name="TIncType">Type of list of included document</typeparam>
        /// <param name="id">Id of document</param>
        /// <param name="field">Field with have list of related document id</param>
        /// <param name="targetField">Target field for set related document</param>
        /// <param name="cancellationToken"></param>
        /// <returns>T</returns>
        public async Task<T> Get<T, TIncType>(string id, Expression<Func<T, IEnumerable<string>>> field,
            Expression<Func<T, IEnumerable<TIncType>>> targetField, CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            var res = await _session
                .Include(field)
                .LoadAsync<T>(id, cancellationToken);

            var keys = res.GetPropertyValue(field);

            var inc = await _session.LoadAsync<TIncType>(keys, cancellationToken);

            var v = inc.Select(p => p.Value).ToList().AsEnumerable();

            res.SetPropertyValue(targetField, v);
            return res;
        }

        /// <inheritdoc />
        public async Task<T> Get<T, TIncType1, TIncType2>(string id, Expression<Func<T, string>> field1,
            Expression<Func<T, TIncType1>> targetField1, Expression<Func<T, string>> field2,
            Expression<Func<T, TIncType2>> targetField2, CancellationToken cancellationToken = default)
            where T : DbEntity
            where TIncType1 : DbEntity
            where TIncType2 : DbEntity
        {
            var res = await _session
                .Include(field1)
                .Include(field2)
                .LoadAsync<T>(id, cancellationToken);

            var inc1 = await _session.LoadAsync<TIncType1>(res.GetPropertyValue(field1), cancellationToken);
            res.SetPropertyValue(targetField1, inc1);

            var inc2 = await _session.LoadAsync<TIncType2>(res.GetPropertyValue(field2), cancellationToken);
            res.SetPropertyValue(targetField2, inc2);

            return res;
        }

        #endregion

        #region Insert

        /// <inheritdoc />
        public Task Add<T>(T entity, CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            return _session.StoreAsync(entity, cancellationToken);
        }

        /// <inheritdoc />
        public async Task Add<T>(IEnumerable<T> entities)
            where T : DbEntity
        {
            await using var bulk = _dbConnection.Connection.BulkInsert();
            foreach (var entity in entities)
            {
                await bulk.StoreAsync(entity);
            }
        }

        #endregion

        #region Update

        /// <inheritdoc />
        public Task Update<T>(T entity, CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            entity.ModifiedOn = DateTime.Now;
            return _session.StoreAsync(entity, (string) entity.Id, cancellationToken);
        }

        /// <inheritdoc />
        public async Task Update<T>(IEnumerable<T> entities)
            where T : DbEntity
        {
            await using var bulk = _dbConnection.Connection.BulkInsert();
            foreach (var entity in entities)
            {
                entity.ModifiedOn = DateTime.Now;
                await bulk.StoreAsync(entity, (string) entity.Id);
            }
        }

        /// <inheritdoc />
        public Task Update<T, TField>(T entity, Expression<Func<T, TField>> field, TField value,
            CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            entity.SetPropertyValue(field, value);
            return Update(entity, cancellationToken);
        }

        /// <inheritdoc />
        public async Task Update<T, TField>(Expression<Func<T, bool>> filter,
            Expression<Func<T, TField>> field, TField value, CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            var items = await _session.Query<T>().Where(filter, true).ToListAsync(cancellationToken);
            foreach (var item in items)
            {
                item.SetPropertyValue(field, value);
            }

            await Update(items);
        }

        #endregion

        #region Delete

        /// <inheritdoc />
        public void Delete<T>(object id)
            where T : DbEntity
        {
            _session.Delete(id);
        }

        /// <inheritdoc />
        public async Task Delete<T>(T entity, CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            _session.Delete(entity);
        }

        /// <inheritdoc />
        public async Task Delete<T>(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            //await _dbConnection.Connection.Operations.SendAsync(new DeleteByQueryOperation<T>(filter));
            var items = await _session.Query<T>().Where(filter, true).ToListAsync(cancellationToken);
            foreach (var item in items)
            {
                Delete(item);
            }
        }

        /// <inheritdoc />
        public async Task DeleteAll<T>(CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            var items = await _session.Query<T>().ToListAsync(cancellationToken);
            foreach (var item in items)
            {
                Delete(item);
            }
        }

        #endregion

        #region Find

        /// <inheritdoc />
        public async Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> filter,
            CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            return await _session.Query<T>().Where(filter, true).ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> filter, Expression<Func<T, object>> order,
            int pageIndex, int size, CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            return await Find(filter, order, pageIndex, size, false, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> filter, int pageIndex, int size, CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            return await Find(filter, f => f.Id, pageIndex, size, cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<T>> Find<T>(Expression<Func<T, bool>> filter,
            Expression<Func<T, object>> order,
            int pageIndex, int size, bool isDescending, CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            var items = _session.Query<T>().Where(filter, true);
            var oItems = !isDescending
                ? LinqExtensions.OrderBy(items, order)
                : LinqExtensions.OrderByDescending(items, order);
            return await oItems
                .Skip(pageIndex * size)
                .Take(size)
                .ToListAsync(cancellationToken);
        }

        #endregion

        #region Util

        /// <inheritdoc />
        public async Task<long> Count<T>(CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            return await _session.Query<T>().CountAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<long> Count<T>(Expression<Func<T, bool>> filter,
            CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            return await _session.Query<T>().CountAsync(filter, cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> Any<T>(CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            return _session.Query<T>().AnyAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> Any<T>(Expression<Func<T, bool>> filter, CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            return _session.Query<T>().AnyAsync(filter, cancellationToken);
        }

        #endregion

        #region Max/Min

        /// <inheritdoc />
        public Task<T> Max<T>(Expression<Func<T, bool>> filter, Expression<Func<T, object>> field,
            CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            return _session.Query<T>()
                .Where(filter, true)
                .OrderByDescending(field)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<T> Max<T>(Expression<Func<T, object>> field, CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            return Max(f => true, field, cancellationToken);
        }

        /// <inheritdoc />
        public Task<T> Min<T>(Expression<Func<T, bool>> filter, Expression<Func<T, object>> field,
            CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            return _session.Query<T>()
                .Where(filter, true)
                .OrderBy(field)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public Task<T> Min<T>(Expression<Func<T, object>> field, CancellationToken cancellationToken = default)
            where T : DbEntity
        {
            return Min(f => true, field, cancellationToken);
        }

        #endregion
    }
}