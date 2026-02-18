using StepanoffGames.Signals;
using System.Collections.Generic;
using UnityEngine;

namespace StepanoffGames.Cameras.Signals
{
	public class AddCamerasSignal : BaseSignal
	{
		public List<Camera> Cameras;

		public AddCamerasSignal(List<Camera> cameras)
		{
			Cameras = cameras;
		}
	}
}

