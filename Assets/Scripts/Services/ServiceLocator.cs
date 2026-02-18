using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.Services
{
	public static class ServiceLocator
	{
		private static Dictionary<string, IService> _services;

		static ServiceLocator()
		{
			_services = new Dictionary<string, IService>();
		}

		public static T Get<T>() where T : IService
		{
			string key = typeof(T).Name;
			if (!_services.ContainsKey(key))
			{
				return default(T);

				//Debug.LogError($"{key} not registered.");
				//throw new InvalidOperationException();
			}

			return (T)_services[key];
		}

		public static void Register<T>(T service) where T : IService
		{
			string key = typeof(T).Name;
			if (_services.ContainsKey(key))
			{
				Debug.LogError($"Attempted to register service of type {key} which is already registered.");
				return;
			}

			_services.Add(key, service);
		}

		public static void Unregister<T>() where T : IService
		{
			string key = typeof(T).Name;
			if (!_services.ContainsKey(key))
			{
				Debug.LogError($"Attempted to unregister service of type {key} which is not registered.");
				return;
			}

			_services.Remove(key);
		}
	}
}
