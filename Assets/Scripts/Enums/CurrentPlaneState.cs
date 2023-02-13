using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phonado.Enums
{
	/// <summary>
	/// Organizes the different states the plane can be in
	/// </summary>
	public enum CurrentPlaneState
	{
		AtCenter,
		Exploding,
		Spinning,
		ToBeHeld,
		Held,
		UpdatePlanePosition,
		Placed
	}
}
