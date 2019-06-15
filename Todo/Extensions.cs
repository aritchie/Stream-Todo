using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using ReactiveUI;


namespace Todo
{
    public static class Extensions
    {
        public static IDisposable SubOnMainThread<T>(this IObservable<T> obs, Action<T> onNext)
            => obs
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext);


        public static IDisposable SubOnMainThread<T>(this IObservable<T> obs, Action<T> onNext, Action<Exception> onError)
            => obs
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext, onError);


        public static IDisposable SubOnMainThread<T>(this IObservable<T> obs, Action<T> onNext, Action<Exception> onError, Action onComplete)
            => obs
                .ObserveOn(RxApp.MainThreadScheduler)
                .Subscribe(onNext, onError, onComplete);


        public static async Task Navigate(this INavigationService navigation, string uri, params (string, object)[] parameters)
        {
            var result = await navigation.NavigateAsync(uri, parameters.ToNavParams());
            if (!result.Success)
                Console.WriteLine("[NAV FAIL] " + result.Exception);
            //throw new ArgumentException("Failed to navigate", result.Exception);
        }


        public static ICommand NavigateCommand(this INavigationService navigation, string uri, Func<(string, object)[]> getParams = null)
            => ReactiveCommand.CreateFromTask(() => navigation.Navigate(uri, getParams?.Invoke()));


        public static async Task GoBack(this INavigationService navigation, params (string, object)[] parameters)
        {
            var result = await navigation.GoBackAsync(parameters.ToNavParams());
            if (!result.Success)
                Console.WriteLine("[NAV FAIL] " + result.Exception);
            //throw new ArgumentException("Failed to navigate", result.Exception);
        }


        public static ICommand GoBackCommand(this INavigationService navigation, Func<(string, object)[]> getParams = null)
            => ReactiveCommand.CreateFromTask(() => navigation.GoBack(getParams?.Invoke()));


        static NavigationParameters ToNavParams(this (string, object)[] parameters)
        {
            if (parameters?.Any() ?? true)
                return null;

            var navParams = new NavigationParameters();
            foreach (var p in parameters)
                navParams.Add(p.Item1, p.Item2);

            return navParams;
        }
    }
}
