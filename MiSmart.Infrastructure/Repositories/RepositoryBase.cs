using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MiSmart.Infrastructure.Commands;
using MiSmart.Infrastructure.Responses;
using MiSmart.Infrastructure.ViewModels;
using Microsoft.EntityFrameworkCore;
using MiSmart.Infrastructure.Data;

namespace MiSmart.Infrastructure.Repositories
{
    public abstract class RepositoryBase<T> where T : class
    {
        protected DbContext context { get; set; }
        protected RepositoryBase(DbContext context) => this.context = context;
        public T Get(Expression<Func<T, Boolean>> expression)
        {
            return context.Set<T>().FirstOrDefault(expression);
        }
        public virtual TView GetView<TView>(Expression<Func<T, Boolean>> expression) where TView : class, IViewModel<T>, new()
        {
            var entity = context.Set<T>().FirstOrDefault(expression);
            if (entity is not null)
                return ViewModelHelpers.ConvertToViewModel<T, TView>(entity);
            return null;
        }
        public virtual ListResponse<TView> GetListResponseView<TView>(PageCommand pageCommand, Expression<Func<T, Boolean>> expression, Func<T, Object> order = null, Boolean ascending = true) where TView : class, IViewModel<T>, new()
        {
            var count = context.Set<T>().Count(expression);
            List<TView> data;
            var pageIndex = pageCommand.PageIndex;
            var pageSize = pageCommand.PageSize;
            var originData = context.Set<T>().Where(expression);
            if (pageIndex.HasValue && pageSize.HasValue)
            {

                if (order is Object)
                {
                    data = ascending ? originData.OrderBy(order).Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToList().Select(ww => ViewModelHelpers.ConvertToViewModel<T, TView>(ww)).ToList() :
                    originData.OrderByDescending(order).Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToList().Select(ww => ViewModelHelpers.ConvertToViewModel<T, TView>(ww)).ToList();
                }
                else
                {
                    data = originData.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToList().Select(ww => ViewModelHelpers.ConvertToViewModel<T, TView>(ww)).ToList();
                }
            }
            else
            {

                if (order is Object)
                {
                    data = ascending ? originData.OrderBy(order).ToList().Select(ww => ViewModelHelpers.ConvertToViewModel<T, TView>(ww)).ToList() :
                  originData.OrderByDescending(order).ToList().Select(ww => ViewModelHelpers.ConvertToViewModel<T, TView>(ww)).ToList();
                }
                else
                {
                    data = originData.ToList().Select(ww => ViewModelHelpers.ConvertToViewModel<T, TView>(ww)).ToList();
                }
            }
            return new ListResponse<TView> { Data = data, PageIndex = pageIndex, PageSize = pageSize, TotalRecords = count };
        }
        public virtual List<T> GetListEntities(PageCommand pageCommand, Expression<Func<T, Boolean>> expression, Func<T, Object> order = null, Boolean ascending = true)
        {
            List<T> data;
            var pageIndex = pageCommand.PageIndex;
            var pageSize = pageCommand.PageSize;
            var originData = context.Set<T>().Where(expression);
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                if (order is Object)
                {
                    data = ascending ? originData.OrderBy(order).Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToList() :
                    originData.OrderByDescending(order).Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToList();
                }
                else
                {
                    data = originData.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToList();
                }
            }
            else
            {

                if (order is Object)
                {
                    data = ascending ? originData.OrderBy(order).ToList() :
            originData.OrderByDescending(order).ToList();
                }
                else
                {
                    data = originData.ToList();
                }
            }
            return data;
        }
        public virtual Boolean Any(Expression<Func<T, Boolean>> expression)
        {
            return context.Set<T>().Any(expression);
        }
        public virtual ListResponse<T> GetList(PageCommand pageCommand, Expression<Func<T, Boolean>> expression, Func<T, Object> order = null, Boolean ascending = true)
        {
            var count = context.Set<T>().Count(expression);
            List<T> data;
            var pageIndex = pageCommand.PageIndex;
            var pageSize = pageCommand.PageSize;
            var originData = context.Set<T>().Where(expression);
            if (pageIndex.HasValue && pageSize.HasValue)

            {
                if (order is Object)
                {
                    data = ascending ? originData.OrderBy(order).Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToList() :
                    originData.OrderByDescending(order).Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToList();
                }
                else
                {
                    data = originData.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value).ToList();
                }
            }
            else
            {

                if (order is Object)
                {
                    data = ascending ? originData.OrderBy(order).ToList() :
            originData.OrderByDescending(order).ToList();
                }
                else
                {
                    data = originData.ToList();
                }
            }
            return new ListResponse<T> { Data = data, PageIndex = pageIndex, PageSize = pageSize, TotalRecords = count };
        }
        public virtual void Create(T entity)
        {
            context.Add(entity);
            context.SaveChanges();
        }
        public virtual void Update(T entity)
        {
            context.Update(entity);
            context.SaveChanges();
        }
        public virtual void Delete(T entity)
        {
            context.Remove(entity);
            context.SaveChanges();
        }
        public virtual void DeleteRange(List<T> listEntities)
        {
            context.RemoveRange(listEntities);
            context.SaveChanges();
        }
    }
}