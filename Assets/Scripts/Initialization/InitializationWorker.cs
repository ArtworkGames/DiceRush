using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace StepanoffGames.Initialization
{
	public class InitializationWorker
	{
		private enum InitializationState
		{
			Wait,
			Initializing,
			Initialized,
			Error
		}

		private class InitializableItem
		{
			public Type Type;
			public IInitializable Initializable;
			public List<Type> Dependencies;
			public InitializationState State;
			public float InitializationWeight;
		}

		private List<InitializableItem> _initializableItems;

		public InitializationWorker()
		{
			_initializableItems = new List<InitializableItem>();
		}

		public void Register(IInitializable initializable)
		{
			InitializationState state = InitializationState.Wait;
			if (initializable.Dependencies != null)
			{
				foreach (Type type in initializable.Dependencies)
				{
					if (!typeof(IInitializable).IsAssignableFrom(type))
					{
						Debug.LogError($"{initializable.GetType()} has wrong dependency from '{type}'. Must be inherited from '{typeof(IInitializable)}'");
						state = InitializationState.Error;
					}
				}
			}

			InitializableItem item = new InitializableItem()
			{
				Type = initializable.GetType(),
				Initializable = initializable,
				Dependencies = initializable.Dependencies,
				State = state,
				InitializationWeight = initializable.InitializationWeight
			};
			_initializableItems.Add(item);
		}

		public async UniTask InitializeAllAsync()
		{
			IEnumerable<InitializableItem> next = _initializableItems.Where(item => item.State == InitializationState.Wait && IsAllInitialized(item.Dependencies));
			do
			{
				if (next.Any())
				{
					UniTask[] tasks = next.Select(x => InitializeAsync(x)).ToArray();
					await UniTask.WhenAll(tasks);
				}
				next = _initializableItems.Where(item => item.State == InitializationState.Wait && IsAllInitialized(item.Dependencies));
			}
			while (next.Any());
		}

		private async UniTask InitializeAsync(InitializableItem item)
		{
			if (item.State == InitializationState.Wait)
			{
				//Debug.Log(Time.realtimeSinceStartup + " Initialize " + item.Type);

				item.State = InitializationState.Initializing;
				try
				{
					await item.Initializable.InitializeAsync();
					item.Initializable.MakeInitialised();
					item.State = InitializationState.Initialized;
					item.Initializable = null;

					//Debug.Log(Time.realtimeSinceStartup + " " + item.Type + " initialized");
				}
				catch (Exception exception)
				{
					item.State = InitializationState.Error;
					Debug.LogException(exception);
				}
			}
		}

		public float GetInitializationProgress()
		{
			float fullWeight = 0.0f;
			float completed = 0.0f;
			for (int i = 0; i < _initializableItems.Count; i++)
			{
				fullWeight += _initializableItems[i].InitializationWeight;
				switch (_initializableItems[i].State)
				{
					case InitializationState.Wait:
						break;
					case InitializationState.Initializing:
						completed += _initializableItems[i].Initializable.InitializationProgress * _initializableItems[i].InitializationWeight;
						break;
					case InitializationState.Initialized:
						completed += _initializableItems[i].InitializationWeight;
						break;
					case InitializationState.Error:
						completed += _initializableItems[i].InitializationWeight;
						break;
				}
			}
			return (completed / fullWeight);
		}

		private bool IsAllInitialized(List<Type> dependencies)
		{
			if (dependencies == null || dependencies.Count == 0)
				return true;

			return dependencies.All(dependecy => _initializableItems.Any(
				item => item.State == InitializationState.Initialized && dependecy.IsAssignableFrom(item.Type))
			);
		}
	}
}
