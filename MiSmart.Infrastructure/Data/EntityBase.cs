using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Infrastructure;
namespace MiSmart.Infrastructure.Data
{
    [Serializable]
    public abstract class EntityBase<T> 
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public T ID { get; set; }
        protected ILazyLoader lazyLoader;
        protected EntityBase(ILazyLoader lazyLoader)
        {
            this.lazyLoader = lazyLoader;
        }
        protected EntityBase() { }
    }
}