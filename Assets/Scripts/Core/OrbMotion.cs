using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phonado.Core
{
	public class OrbMotion : MonoBehaviour
	{
		/// <summary>
		/// Used for lerp to represent the current time position
		/// </summary>
		private float currentTime;
		/// <summary>
		/// Where orb starts moving from
		/// </summary>
		private Vector3 startingPos;
		/// <summary>
		/// Point where orb will travel to, set externally
		/// </summary>
		[HideInInspector] public Vector3 TargetPos;
		/// <summary>
		/// Flag to let external entity know when orb has reached its destination
		/// </summary>
		[HideInInspector] public bool Moving;
		
		void Start()
		{
			currentTime = 0f;
			startingPos = gameObject.transform.position;
			Moving = true;
		}

		void Update()
		{
			//Checking when orb gets to be a certain distance away from the target position
			if (Moving && (gameObject.transform.position - TargetPos).magnitude > 0.2f)
			{
				gameObject.transform.position = Vector3.Slerp(startingPos, TargetPos, currentTime);
				currentTime += 0.005f;
			}
			else
			{
				Moving = false;
			}
		}
	}
}
