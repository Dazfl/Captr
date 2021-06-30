using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Captr
{
	public class CaptrOptionsBuilder
	{
		private readonly CaptrOptions _options;

		private readonly Queue<ServiceDescriptor> _serviceDescriptors;
		public CaptrOptionsBuilder()
		{
			_options = new();
			_serviceDescriptors = new(0);
		}

		public void AddService(ServiceDescriptor service)
		{
			_serviceDescriptors.Enqueue(service);
		}

		public ServiceDescriptor RemoveService()
		{
			if (_serviceDescriptors.Count > 0)
				return _serviceDescriptors.Dequeue();

			return null;
		}

		public IEnumerable<ServiceDescriptor> GetServices()
		{
			return _serviceDescriptors.ToList();
		}

		public int SnapshotInterval
		{
			get => int.Parse(_options.GetConfigItem("SnapshotInterval"));
			set => _options.SetConfigItem("SnapshotInterval", $"{value}");
		}

		public void SetOption(string itemName, string itemValue) => _options.SetConfigItem(itemName, itemValue);

		public CaptrOptions GetOptions() => _options;
	}
}
