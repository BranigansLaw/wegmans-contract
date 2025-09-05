namespace INN.JobRunner.Extensions
{
    public static class IServiceProviderExtensions
    {
        /// <summary>
        /// Gets the service of type <typeparamref name="T"/> from <paramref name="serviceProvider"/> if it is registered, or throws a <see cref="Exception"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceProvider"></param>
        /// <returns>The dependency <typeparamref name="T"/></returns>
        /// <exception cref="Exception">If the type <typeparamref name="T"/> is not registered</exception>
        public static T GetServiceOrThrowException<T>(this IServiceProvider serviceProvider) where T : class
        {
            T? service = (T?)serviceProvider.GetService(typeof(T));

            return service is null ? throw new Exception($"Configuration setup failed. Could not initialize {typeof(T)}") : service;
        }
    }
}
