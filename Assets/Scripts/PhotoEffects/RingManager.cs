using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.XR.MagicLeap;

namespace Phonado.PhotoEffects
{
	public class RingManager : MonoBehaviour
	{
		//Cubes will still be spaced out if radius isn't too small regardless of number of cubes
		public int PlanesPerRing;

		//Height between each ring of cubes
		public float HeightGap;

		//Should be larger with larger amount of cubes
		public float RingRadius;

		//Can be changed, but set to 8 due to amount of bands for FFT data
		public int AmountOfRings = 8;

		//The default rate each ring moves by, with 2*pi having the unit move once around the circle per update
		public float RingSpinRate;

		public float ExplosionSpeed;

		/// <summary>
		/// If the planes should be scaled to match the ratio of the picture they are showing
		/// </summary>
		public bool ScalePlanesToImageRatios;

		public GameObject HolderObject;
		
		[HideInInspector] public Vector3 HeadPosition;

		private bool hasExploded;

		//private MLResult eyeResult;

		private GameObject ringPrefab;

		[HideInInspector] public List<Ring> Rings;

		//public Canvas StartScreen;
		private bool forceStart = true;

		void Start()
		{
			//transform.position = Camera.main.transform.position;
			//transform.Translate(0, 0, 10);
			//eyeResult = MLEyes.Start();
			ringPrefab = Resources.Load<GameObject>("ring");

			Rings = new List<Ring>();
			hasExploded = false;

			//will spawn each ring and move each ring up depending on the current ring number
			for (int i = 0; i < AmountOfRings; i++)
			{
				var ringObj = Instantiate(ringPrefab, this.transform);
				var ringScript = ringObj.GetComponent<Ring>();
				ringScript.PlaneAmount = PlanesPerRing;
				ringScript.RingHeight = (i - AmountOfRings / 2) * HeightGap;
				ringScript.CurrentRadius = RingRadius;
				ringScript.ScalePlanesToImageRatios = ScalePlanesToImageRatios;
				ringScript.Parent = this;
				ringScript.ExplosionSpeed = ExplosionSpeed;
				//sets each ring's spin, the lower half will be spinning in reverse while the upper half spins forward
				//rings further toward the bottom and top will spin faster than rings towards the center
				ringScript.TimeRate = RingSpinRate * (i - (AmountOfRings - 1) / 8f);
				//taking each time rate and dividing by 8 so each speed is slowed a large amount
				ringScript.TimeRate /= 8f;
				ringScript.HolderObject = HolderObject;
				

				Rings.Add(ringScript);
			}
		}
		/// <summary>
		/// Calls each ring's Explode function, sending their planes onto the ring
		/// </summary>
		public void Explode()
		{

			if (!hasExploded)
			{
				hasExploded = true;
				foreach (var ring in Rings)
				{
					ring.Explode();
				}
			}
		}

	}
}