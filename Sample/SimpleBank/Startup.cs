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

			// Depending on the storage being used, these details can be stored
			// and retrieved from appsettings.json, etc.  The connection string
			// here is for a locally running Storage Emulator
			/// (See <see cref="https://docs.microsoft.com/en-us/azure/storage/common/storage-use-emulator"/> for more details about the Azure Storage Emulator)
			string connectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;";
			string eventTableName = "SimpleBankStreams";
			string snapshotTableName = "SimpleBankSnapshots";

			services.AddCaptr(options =>
			{
				options.SnapshotInterval = 10;
				options.AddEventStorage(cob => cob.UseAzureTableStorageAsEventStore(connectionString, eventTableName));
				options.AddSnapshotStorage(cob => cob.UseAzureTableStorageAsSnapshotStore(connectionString, snapshotTableName));
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
			services.AddDelegate<CaptrClient, CaptrClientServices<Account>.LoadEntity>(method => method.LoadEntity<Account>);
			services.AddDelegate<CaptrClient, CaptrClientServices<Account>.SaveEntityChanges>(method => method.SaveEntityChanges<Account>);
			return services;
		}

		public static IServiceCollection AddDelegate<TService, TDelegate>(this IServiceCollection services, Func<TService, TDelegate> DelegateFromService)
			where TDelegate : Delegate
		{
			return services.AddScoped(sp => DelegateFromService(sp.GetRequiredService<TService>()));
		}
	}
}
