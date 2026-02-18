using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace StepanoffGames.Initialization
{
	public abstract class BaseInitializable : IInitializable
	{
		virtual public List<Type> Dependencies => new List<Type> { };
		virtual public float InitializationWeight => 1.0f;
		virtual public float InitializationProgress { get; protected set; }

		protected bool _isInitialized = false;
		public bool IsInitialized => _isInitialized;

		abstract public UniTask InitializeAsync();

		public void MakeInitialised()
		{
			_isInitialized = true;
		}

		virtual public void Dispose()
		{
		}
	}
}
