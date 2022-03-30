using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SequelNet.Connector;
using System;
using System.Reflection;

namespace SequelNet.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSequelNet(this IServiceCollection services, IConfiguration namedConfigurationSection)
    {
        var connectionString = namedConfigurationSection.GetValue<string>("Type");
        var connectorName = namedConfigurationSection.GetValue<string>("Connector");

        Assembly asm = Assembly.Load($"SequelNet.Connector.{connectorName}");
        Type factoryType = asm.GetType($"SequelNet.Connector.{connectorName}Factory");

        IConnectorFactory factoryInstance = (IConnectorFactory)Activator.CreateInstance(factoryType, new string[] { connectionString });

        ConnectorBase.SetDefaultConnectionString(connectionString);
        ConnectorBase.SetDefaultConnectorTypeByName(connectorName);

        services.AddSingleton(factoryType, factoryInstance);

        return services;
    }
}
