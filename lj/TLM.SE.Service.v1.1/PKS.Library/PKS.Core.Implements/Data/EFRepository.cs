﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PKS.Utils;
using PKS.Data;

namespace PKS.Data
{
    /// <summary>EF数据仓储访问</summary>
    public class EFRepository<TEntity> : IRepository<TEntity>, IDisposable
        where TEntity : class
    {
        /// <summary>EF数据访问</summary>
        protected DbContext _context;

        /// <summary>构造函数</summary>
        public EFRepository(IDbContext context)
        {
            _context = context.As<DbContext>();
            Store = _context.Set<TEntity>();
        }

        /// <summary>实体集</summary>
        protected DbSet<TEntity> Store { get; set; }

        /// <summary>释放对象</summary>
        public void Dispose()
        {
            _trans?.Rollback();
        }

        /// <summary>
        ///     查询接口
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> GetQuery()
        {
            return Store;
        }

        /// <summary>
        ///     增加
        /// </summary>
        public void Add(TEntity entity, bool submit = true)
        {
            Store.Add(entity);

            if (submit) Submit();
        }

        /// <summary>增加</summary>
        public async Task AddAsync(TEntity entity)
        {
            Store.Add(entity);
            await SubmitAsync();
        }
        /// <summary>附加实体</summary>
        public void Update(TEntity entity, bool submit = true)
        {
            DbEntityEntry entry;
            try
            {
                entry = _context.Entry(entity);
            }
            catch (InvalidOperationException)
            {
                var objectContext = _context.As<IObjectContextAdapter>().ObjectContext;
                var objectSet = objectContext.CreateObjectSet<TEntity>();
                var entityKey = objectContext.CreateEntityKey(objectSet.EntitySet.Name, entity);
                object foundEntity;
                if (!objectContext.TryGetObjectByKey(entityKey, out foundEntity)) throw;
                objectContext.Detach(foundEntity);
                entry = _context.Entry(entity);
            }
            if (entry.State != EntityState.Modified)
            {
                entry.State = EntityState.Modified;
            }
            if (submit) Submit();
        }

        /// <summary>
        /// 更新
        /// </summary>
        public async Task UpdateAsync(TEntity entity)
        {
            Update(entity, false);
            await SubmitAsync();
        }
        /// <summary>
        ///     删除
        /// </summary>
        public void Delete(TEntity entity, bool submit = true)
        {
            Store.Remove(entity);
            if (submit) Submit();
        }

        /// <summary>
        /// 删除
        /// </summary>
        public async Task DeleteAsync(TEntity entity)
        {
            Store.Remove(entity);
            await SubmitAsync();
        }
        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="whereExpr"></param>
        public virtual void DeleteList(Expression<Func<TEntity, bool>> whereExpr)
        {
            IEnumerable<TEntity> entities = GetQuery().Where(whereExpr).ToList();

            DeleteList(entities);
        }

        /// <summary>
        ///     删除
        /// </summary>
        /// <param name="entities"></param>
        public virtual void DeleteList(IEnumerable<TEntity> entities)
        {
            foreach (var entity in entities)
                Store.Remove(entity);

            Submit();
        }

        /// <summary>
        ///     判断是否存在
        /// </summary>
        /// <param name="whereExpr"></param>
        /// <returns></returns>
        public bool Exist(Expression<Func<TEntity, bool>> whereExpr)
        {
            return Count(whereExpr) > 0;
        }

        /// <summary>
        ///     获取记录数
        /// </summary>
        /// <param name="whereExpr"></param>
        /// <returns></returns>
        public int Count(Expression<Func<TEntity, bool>> whereExpr)
        {
            return GetQuery().Where(whereExpr).Count();
        }

        /// <summary>
        ///     获取全部
        /// </summary>
        /// <returns></returns>
        public List<TEntity> GetAll()
        {
            return GetQuery().ToList();
        }

        /// <summary>
        ///     查找实体对象
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public TEntity Find(object key)
        {
            return Store.Find(key);
        }

        /// <summary>
        ///     查找实体对象
        /// </summary>
        /// <param name="whereExpr"></param>
        /// <returns></returns>
        public TEntity Find(Expression<Func<TEntity, bool>> whereExpr)
        {
            return GetQuery().Where(whereExpr).FirstOrDefault();
        }

        /// <summary>
        ///     查找实体对象列表
        /// </summary>
        public IEnumerable<TEntity> FindList<TKey>(Expression<Func<TEntity, bool>> whereExpr,
            Expression<Func<TEntity, TKey>> orderbyExpr, int orderDirection)
        {
            return FindList(whereExpr, t => t, orderbyExpr, orderDirection);
        }


        /// <summary>
        ///     查找实体对象列表
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="whereExpr"></param>
        /// <param name="selectExpr"></param>
        /// <param name="orderbyExpr"></param>
        /// <param name="orderDirection"></param>
        /// <param name="returnCount"></param>
        /// <returns></returns>
        public IEnumerable<TResult> FindList<TResult, TKey>(Expression<Func<TEntity, bool>> whereExpr,
            Expression<Func<TEntity, TResult>> selectExpr, Expression<Func<TResult, TKey>> orderbyExpr,
            int orderDirection, int returnCount = -1)
        {
            var result = GetQuery().Where(whereExpr).Select(selectExpr);
            if (result != null && result.Count() > 0)
            {
                returnCount = result.Count();
                if (returnCount > 0)
                    if (orderDirection > 0)
                        result = result.OrderByDescending(orderbyExpr).Take(returnCount);
                    else
                        result = result.OrderBy(orderbyExpr).Take(returnCount);
                return result.ToList();
            }
            return null;
        }

        /// <summary>
        ///     分页查找实体对象列表
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <typeparam name="TKey"></typeparam>
        /// <param name="whereExpr"></param>
        /// <param name="selectExpr"></param>
        /// <param name="orderbyExpr"></param>
        /// <param name="orderDirection"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNo"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public IEnumerable<TResult> FindListByPage<TResult, TKey>(Expression<Func<TEntity, bool>> whereExpr,
            Expression<Func<TEntity, TResult>> selectExpr, Expression<Func<TResult, TKey>> orderbyExpr,
            int orderDirection, int pageSize, int pageNo, out int recordCount)
        {
            var result = GetQuery().Where(whereExpr).Select(selectExpr);
            recordCount = result.Count();

            if (pageNo > recordCount) pageNo = recordCount;
            if (pageNo <= 0) pageNo = 1;

            if (recordCount > 0)
                if (recordCount > pageSize)
                {
                    if (orderDirection > 0)
                        return result.OrderByDescending(orderbyExpr).Skip((pageNo - 1) * pageSize).Take(pageSize)
                            .ToList();
                    return result.OrderBy(orderbyExpr).Skip((pageNo - 1) * pageSize).Take(pageSize).ToList();
                }
                else
                {
                    if (orderDirection > 0)
                        return result.OrderByDescending(orderbyExpr).ToList();
                    return result.OrderBy(orderbyExpr).ToList();
                }
            return null;
        }

        /// <summary>
        ///     提交保存所有变更操作
        /// </summary>
        public void Submit()
        {
            _context.SaveChanges();
        }

        /// <summary>
        /// 提交保存所有变更操作
        /// </summary>
        public async Task SubmitAsync()
        {
            await _context.SaveChangesAsync();
        }

        #region 事务

        /// <summary>
        ///     事务
        /// </summary>
        private DbContextTransaction _trans;

        /// <summary>
        ///     开启事务
        /// </summary>
        public virtual void BeginTrans()
        {
            _trans = _context.Database.BeginTransaction();
        }

        /// <summary>
        ///     事务提交
        ///     判断:_trans对象不为空前提下
        /// </summary>
        public virtual void EndTrans()
        {
            try
            {
                if (_trans != null)
                {
                    _trans.Commit();
                    _trans.Dispose();
                    _trans = null;
                }
            }
            catch
            {
                RollbackTrans();
                throw;
            }
        }

        /// <summary>
        ///     事务回滚
        ///     判断:_trans对象不为空前提下
        /// </summary>
        public virtual void RollbackTrans()
        {
            if (_trans != null)
            {
                _trans.Rollback();
                _trans.Dispose();
                _trans = null;
            }
        }

        #endregion
    }
}