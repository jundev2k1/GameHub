using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace game_x.infrastructure.Extensions;

public static class SettingsBindingExtensions
{
    public static IServiceCollection AutoBindSettings(this IServiceCollection services, IConfiguration configuration, Assembly assembly)
    {
        var settingsTypes = assembly
            .GetTypes()
            .Where(t =>
                t.IsClass &&
                !t.IsAbstract &&
                t.Name.EndsWith("Settings", StringComparison.OrdinalIgnoreCase));

        foreach (var type in settingsTypes)
        {
            var section = configuration.GetSection(type.Name);
            var method = typeof(OptionsConfigurationServiceCollectionExtensions)
                .GetMethods(BindingFlags.Static | BindingFlags.Public)
                .First(m =>
                    m.Name == nameof(OptionsConfigurationServiceCollectionExtensions.Configure) &&
                    m.IsGenericMethodDefinition &&
                    m.GetParameters().Length == 2);

            var genericMethod = method.MakeGenericMethod(type);
            genericMethod.Invoke(null, new object[] { services, section });
        }

        return services;
    }
}
