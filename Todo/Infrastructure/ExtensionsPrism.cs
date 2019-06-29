using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Navigation;
using ReactiveUI;


namespace Todo
{
    public static class ExtensionsPrism
    {
        public static Task Navigate(this INavigationService navigation, string uri, params (string, object)[] parameters)
            => navigation.Navigate(uri, parameters.ToNavParams());


        public static async Task Navigate(this INavigationService navigation, string uri, INavigationParameters parameters)
        {
            var result = await navigation.NavigateAsync(uri, parameters);
            if (!result.Success)
                Console.WriteLine("[NAV FAIL] " + result.Exception);
            //throw new ArgumentException("Failed to navigate", result.Exception);
        }


        public static ICommand NavigateCommand(this INavigationService navigation, string uri, Action<INavigationParameters> getParams = null, IObservable<bool> canExecute = null)
            => ReactiveCommand.CreateFromTask(async () =>
            {
                var p = new NavigationParameters();
                getParams?.Invoke(p);
                await navigation.Navigate(uri, p);
            }, canExecute);


        public static ICommand NavigateCommand<T>(this INavigationService navigation, string uri, Action<T, INavigationParameters> getParams = null, IObservable<bool> canExecute = null)
               => ReactiveCommand.CreateFromTask<T>(async arg =>
               {
                   var p = new NavigationParameters();
                   getParams?.Invoke(arg, p);
                   await navigation.Navigate(uri, p);
               }, canExecute);



        public static async Task GoBack(this INavigationService navigation, bool toRoot = false, params (string, object)[] parameters)
        {
            var task = toRoot
                ? navigation.GoBackToRootAsync(parameters.ToNavParams())
                : navigation.GoBackAsync(parameters.ToNavParams());

            var result = await task.ConfigureAwait(false);
            if (!result.Success)
                Console.WriteLine("[NAV FAIL] " + result.Exception);
        }


        public static ICommand GoBackCommand(this INavigationService navigation, IObservable<bool> canExecute, bool toRoot = false, Func<(string, object)[]> getParams = null)
            => ReactiveCommand.CreateFromTask(() => navigation.GoBack(toRoot, getParams?.Invoke()), canExecute);


        public static ICommand GoBackCommand(this INavigationService navigation, bool toRoot = false, Func<(string, object)[]> getParams = null)
            => ReactiveCommand.CreateFromTask(() => navigation.GoBack(toRoot, getParams?.Invoke()));


        public static INavigationParameters Set(this INavigationParameters parameters, string key, object value)
        {
            parameters.Add(key, value);
            return parameters;
        }


        public static INavigationParameters AddRange(this INavigationParameters parameters, params (string Key, object Value)[] args)
        {
            foreach (var arg in args)
                parameters.Add(arg.Key, arg.Value);

            return parameters;
        }


        static NavigationParameters ToNavParams(this (string Key, object Value)[] parameters)
        {
            if (parameters?.Any() ?? true)
                return null;

            var navParams = new NavigationParameters();
            navParams.AddRange(parameters);

            return navParams;
        }
    }
}
