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
        var connectionString = namedConfigurationSection.GetValue<string>("ConnectionString");
        var connectorName = namedConfigurationSection.GetValue<string>("Connector")
            ?? namedConfigurationSection.GetValue<string>("ConnectorType");

        Assembly asm = Assembly.Load($"SequelNet.Connector.{connectorName}");
        Type factoryType = asm.GetType($"SequelNet.Connector.{connectorName}Factory");

        IConnectorFactory factoryInstance = (IConnectorFactory)Activator.CreateInstance(factoryType, new string[] { connectionString });

        if (!string.IsNullOrEmpty(connectionString) && factoryType != null)
        {
            ConnectorBase.SetDefaultConnectionString(connectionString);
            ConnectorBase.SetDefaultConnectorTypeByName(connectorName);
        }

        services.AddSingleton(factoryType, factoryInstance);

        return services;
    }
}
