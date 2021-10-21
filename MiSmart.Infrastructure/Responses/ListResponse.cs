using System;
using System.Collections.Generic;
using System.Linq;
using MiSmart.Infrastructure.ViewModels;
namespace MiSmart.Infrastructure.Responses
{
    public class ListResponse<T> where T : class
    {
        public List<T> Data { get; set; }
        public Int32 TotalRecords { get; set; }
        public Int32? PageIndex { get; set; }
        public Int32? PageSize { get; set; }
        public ActionResponse SetResponse(ActionResponse response)
        {
            response.TotalItems = this.TotalRecords;
            response.Data = Data;
            return response;
        }
        public static ListResponse<TView> LoadViewFromList<TView>(List<T> list) where TView : class, IViewModel<T>, new()
        {
            return new ListResponse<TView>
            {
                Data = list.Select(ww => ViewModelHelpers.ConvertToViewModel<T, TView>(ww)).ToList(),
                TotalRecords = list.Count,
                PageIndex = null,
                PageSize = null
            };
        }
        public static ListResponse<T> LoadFromList(List<T> list)
        {
            return new ListResponse<T>
            {
                Data = list,
                TotalRecords = list.Count,
                PageIndex = null,
                PageSize = null
            };
        }
    }
}