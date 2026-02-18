using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace StepanoffGames.Initialization
{
	public interface IInitializable
	{
		List<Type> Dependencies { get; }
		float InitializationWeight { get; }
		float InitializationProgress { get; }
		bool IsInitialized { get; }

		UniTask InitializeAsync();
		void MakeInitialised();
		void Dispose();
	}
}
