using Captr;
using Captr.Extensions;
using Captr.Storage.AzureTableStorage;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SimpleBank.Domain.Aggregates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleBank
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddLogging();

			string connectionString = "<Azure Table Storage Connection string goes here>";
			string eventTableName = "SimpleBankStreams";
			string snapshotTableName = "SimpleBankSnapshots";

			services.AddCaptr(options =>
			{
				options.SnapshotInterval = 10;
				options.AddEventStorage(so => so.UseAzureTableStorageAsEventStore(connectionString, eventTableName));
				options.AddSnapshotStorage(so => so.UseAzureTableStorageAsSnapshotStore(connectionString, snapshotTableName));
			});
			services.AddCaptrDelegates();

			services.AddMediatR(typeof(Startup).Assembly);

			services.AddControllers();
			services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "SimpleBank", Version = "v1" });
			});
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseSwagger();
				app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SimpleBank v1"));
			}

			app.UseRouting();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}

	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection AddCaptrDelegates(this IServiceCollection services)
		{
			services.AddDelegate<CaptrClient, CaptrServices<Account>.LoadEntity>(method => method.LoadEntity<Account>);
			services.AddDelegate<CaptrClient, CaptrServices<Account>.SaveEntityChanges>(method => method.SaveEntityChanges<Account>);
			return services;
		}

		public static IServiceCollection AddDelegate<TService, TDelegate>(this IServiceCollection services, Func<TService, TDelegate> DelegateFromService)
			where TDelegate : Delegate
		{
			return services.AddScoped(sp => DelegateFromService(sp.GetRequiredService<TService>()));
		}
	}

	public class CaptrServices<TEntity>
	{
		public delegate Task<TEntity> LoadEntity(string entityId, CancellationToken cancellationToken);
		public delegate Task<bool> SaveEntityChanges(TEntity entity, CancellationToken cancellationToken);
	}
}
