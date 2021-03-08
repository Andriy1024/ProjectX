using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace ProjectX.Realtime.Infrastructure.PublicContract
{
    public class RealtimeContractsMiddleware
    {
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new CamelCasePropertyNamesContractResolver(),
            Converters = new List<JsonConverter>
            {
                new StringEnumConverter(new CamelCaseNamingStrategy())
            },
            Formatting = Formatting.Indented
        };

        private static readonly ContractTypes Contracts = new ContractTypes();
        private static int _initialized;
        private static string _serializedContracts = "{}";

        public RealtimeContractsMiddleware(RequestDelegate next)
        {
            if (_initialized == 1) return;
            
            Load();
        }

        public Task InvokeAsync(HttpContext context)
        {
            context.Response.ContentType = "application/json";
            context.Response.WriteAsync(_serializedContracts);
            return Task.CompletedTask;
        }

        private void Load()
        {
            if (Interlocked.Exchange(ref _initialized, 1) == 1)
            {
                return;
            }

            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            
            var realTimeType = typeof(IRealtimeMessage);

            var realTimecontracts = assemblies
                                      .SelectMany(a => a.GetTypes())
                                      .Where(t => !t.IsInterface && realTimeType.IsAssignableFrom(t))
                                      .ToArray();

            foreach (var realtimeMessage in realTimecontracts)
            {
                var instance = FormatterServices.GetUninitializedObject(realtimeMessage);
                var name = instance.GetType().Name;
                
                if (Contracts.RealtimeMessages.ContainsKey(name))
                {
                    throw new InvalidOperationException($"RealtimeMessage: '{name}' already exists.");
                }

                instance.SetDefaultInstanceProperties();
                Contracts.RealtimeMessages[name] = instance;
            }

            _serializedContracts = JsonConvert.SerializeObject(Contracts, SerializerSettings);
        }

        private class ContractTypes
        {
            public Dictionary<string, object> RealtimeMessages { get; } = new Dictionary<string, object>();
        }
    }
}
