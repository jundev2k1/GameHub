namespace game_x.application.Common.Context;

public static class MapsterContext
{
    public static IDisposable Use(IDictionary<string, object>? parameters)
        => new MapsterScope(parameters);

    private sealed class MapsterScope : IDisposable
    {
        private readonly MapContextScope _internalScope;

        public MapsterScope(IDictionary<string, object>? parameters)
        {
            _internalScope = new MapContextScope();

            if (parameters != null && MapContext.Current != null)
            {
                foreach (var param in parameters)
                {
                    MapContext.Current.Parameters[param.Key] = param.Value;
                }
            }
        }

        public void Dispose()
        {
            _internalScope.Dispose();
        }
    }
}
