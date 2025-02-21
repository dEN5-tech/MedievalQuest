using Microsoft.Extensions.DependencyInjection;
using System;
using MedivalQuest.DI.Interfaces;
using MedivalQuest.DI.Services;

namespace MedivalQuest.DI
{
    public static class DIContainer
    {
        private static IServiceProvider _serviceProvider;
        private static readonly IServiceCollection _services = new ServiceCollection();

        public static void Initialize()
        {
            _services.AddGameServices();
            _serviceProvider = _services.BuildServiceProvider();
        }

        public static T Resolve<T>() where T : class
        {
            if (_serviceProvider == null)
            {
                Initialize();
            }
            return _serviceProvider.GetService<T>();
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGameServices(this IServiceCollection services)
        {
            // Register singleton services (shared across all entities)
            services.AddSingleton<ICameraService, CameraService>();
            services.AddSingleton<IGUIService, GUIService>();
            
            // Register health service as transient since each entity needs its own instance
            services.AddTransient<IHealthService, HealthService>();
            
            // Register player services as transient
            services.AddTransient<IPlayerMovementService, PlayerMovementService>();
            services.AddTransient<IPlayerStateService, PlayerStateService>();
            
            // Register enemy services as transient
            services.AddTransient<IEnemyMovementService, EnemyMovementService>();
            services.AddTransient<IEnemyStateService, EnemyStateService>();
            
            // Register item services as transient
            services.AddTransient<ISoulItemService, SoulItemService>();
            
            return services;
        }
    }
} 