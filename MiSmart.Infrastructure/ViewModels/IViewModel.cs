using System;
namespace MiSmart.Infrastructure.ViewModels
{
    public interface IViewModel<T>
    {
        void LoadFrom(T entity);
    }
    public static class ViewModelHelpers
    {
        public static TView ConvertToViewModel<T, TView>(T entity) where T : class where TView : IViewModel<T>, new()
        {
            TView view = new TView();
            view.LoadFrom(entity);
            return view;
        }
    }
}