using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit.UI;
using Phonado.Enums;
using Phonado.Logging;
using UnityEngine;
using UnityEngine.Serialization;

namespace Phonado.PhotoEffects
{

	
	public class Holder : MonoBehaviour
	{
		[HideInInspector] public float TimeModifier;

		[HideInInspector] public float RingHeight;

		[HideInInspector] public float CurrentRadius;

		[HideInInspector] public float TimeRate;

		[HideInInspector] public bool ScalePlanesToImageRatios;

		[HideInInspector] public Texture2D Image;

		[HideInInspector] public string NameofImage;
		
		[HideInInspector] public Vector3 ExplosionCenter;

		public GameObject photoHolder;
		
		[HideInInspector]
		public float TravelSpeed;

		[HideInInspector] public Vector3 GrabPoint;

		//Tracks where the plane should go on it's ring
		private Vector3 spinningPosition;

		private float elapsedTime;

		private Vector3 finalScale;

		private float animationProgress;

		private CurrentPlaneState activePlaneState;

		private bool firstExplosion;

		private float travelSpeedMultiplier;

		private float heldTime;
		public bool IsToBeHeld => activePlaneState == CurrentPlaneState.ToBeHeld;

		private ObjectManipulator objectManipulator;

		private float releaseTimer;

		private float releaseTime = .7f;

		private bool isBeingManipulated;

		private bool isHovered;

		private Vector3 startingPosition;

		private GameObject frame;

		/// <summary>
		/// Sets all the starting parameters of the plane and makes sets up it's movement and scale
		/// </summary>
		/// <param name="timeModifier">Where this plane should be relative to the others</param>
		/// <param name="ringHeight">The height of the ring the plane is associated with</param>
		/// <param name="currentRadius">The base radius of the ring the plane is associated with</param>
		/// <param name="timeRate">how much the plane should change each update</param>
		/// <param name="scalePlanesToImageRatios">If the plane should scale to match it's given image</param>
		/// <param name="image">The image displayed on the plane</param>
		/// <param name="nameOfImage">The name associated with the image</param>
		/// <param name="explosionSpeed">How fast the plane travels from the explosion to it's ring position</param>
		/// <param name="holder">The game object to place the image on</param>
		public void SetStartingParameters(
			float timeModifier, float ringHeight, float currentRadius, float timeRate,
			bool scalePlanesToImageRatios, Texture2D image, string nameOfImage,  
			float explosionSpeed, GameObject holder)
		{
			TimeModifier = timeModifier;
			RingHeight = ringHeight;
			CurrentRadius = currentRadius;
			TimeRate = timeRate;
			ScalePlanesToImageRatios = scalePlanesToImageRatios;
			Image = image;
			NameofImage = nameOfImage;
			TravelSpeed = explosionSpeed;
			GrabPoint = Vector3.zero;
			photoHolder = holder;
			spinningPosition = new Vector3(
				Mathf.Sin(TimeModifier) * CurrentRadius,
				RingHeight,
				Mathf.Cos(TimeModifier) * CurrentRadius);

			if (ScalePlanesToImageRatios)
			{
				//scales the image plane to match the ratio of the picture
				float xScalar = 1;
				float yScalar = 1;
				if (Image.width > Image.height)
				{
					yScalar = (float) Image.height / Image.width;
				}
				else
				{
					xScalar = (float) Image.width / Image.height;
				}

				var scale = gameObject.transform.localScale;
				finalScale = new Vector3(xScalar * scale.x, yScalar * scale.y, scale.z);
			}

			transform.localScale = Vector3.zero;
			//setting image from loaded images
			var newHolder = Instantiate(photoHolder, gameObject.transform);
			newHolder.tag = "PhotoPlane";
			foreach (Transform part in newHolder.transform)
			{
				if (part.CompareTag("FrameFront"))
				{
					var x = part.GetComponent<Renderer>().material;
					x.mainTexture = image;
					//x.SetTexture("_MainTex", Image);
					;
					//part.GetComponent<Renderer>().material.SetTexture("_MainTex", Image);
				}
			}

			travelSpeedMultiplier = 1;

			releaseTimer = 0;
			isBeingManipulated = false;
			isHovered = false;
			startingPosition = spinningPosition;
			objectManipulator = newHolder.GetComponent<ObjectManipulator>();
			objectManipulator.OnManipulationStarted.AddListener(ManipulatonStarted);
			objectManipulator.OnManipulationEnded.AddListener(ManipulationEnded);
			objectManipulator.OnHoverEntered.AddListener(HoverEntered);
			objectManipulator.OnHoverExited.AddListener(HoverExited);
			frame = objectManipulator.gameObject;

		}

		// Use this for initialization
		void Start()
		{
			firstExplosion = true;
		}

		// Update is called once per frame
		void Update()
		{
			//updates the time to be used to finding the plane's location on the ring
			elapsedTime += TimeRate * Time.deltaTime;
			switch (activePlaneState)
			{
				//The state where the plane moves from the center of the explosion to it's position on the ring
				//Also increases the image's scale from zero to it's final scale
				//ends when the plane gets to it's ring position
				case CurrentPlaneState.Exploding:
					animationProgress += TravelSpeed * Time.deltaTime;
					gameObject.transform.position = Vector3.Slerp(ExplosionCenter, spinningPosition, animationProgress);
					if (firstExplosion)
					{
						transform.localScale = finalScale * animationProgress;
					}

					if (animationProgress >= 1)
					{
						activePlaneState = CurrentPlaneState.Spinning;
						firstExplosion = false;
						transform.localScale = finalScale;
						if (Logger.IndividualImageHeldTimes.ContainsKey(NameofImage))
						{
							Logger.IndividualImageHeldTimes[NameofImage].SecondsGrabbed = heldTime;
							Logger.IndividualImageHeldTimes[NameofImage].TimesGrabbed++;
						}

					}

					goto case CurrentPlaneState.UpdatePlanePosition;
				//The state where the plane is spinning around the ring, facing the player
				case CurrentPlaneState.Spinning:
					gameObject.transform.position = spinningPosition;
					goto case CurrentPlaneState.UpdatePlanePosition;
				case CurrentPlaneState.ToBeHeld:
					animationProgress += TravelSpeed * Time.deltaTime * travelSpeedMultiplier;
					transform.position = Vector3.Slerp(transform.position, GrabPoint, animationProgress);
					if (animationProgress >= 1)
					{
						activePlaneState = CurrentPlaneState.Held;
						//checking if the name of the image inside the dictionary does not exist yet
						//then will add a new timespan to record how long an image is being held
						if (Logger.IndividualImageHeldTimes.ContainsKey(NameofImage))
						{
							//setting currenttime to already pre-existing time
							heldTime = Logger.IndividualImageHeldTimes[NameofImage].SecondsGrabbed;
						}
						else
						{
							Logger.IndividualImageHeldTimes.Add(NameofImage, 
								new SessionPhotoLogDto()
								{
									PhotoName = NameofImage,
									SecondsGrabbed = 0,
									TimesGrabbed = 1
								});
						}
						//check name of image against name of image in db to see if it already exists?
						//considering storing a local tuple of name + time held, but may not need that if we check against db
					}

					break;
				case CurrentPlaneState.Held:
					if (!isBeingManipulated && isHovered)
					{
						releaseTimer += Time.deltaTime;
						if(releaseTimer > releaseTime)
                        {
							GrabPoint = Vector3.zero;
                        }
					}
					if (GrabPoint == Vector3.zero)
					{
						Explode(frame.transform.position, false);
					}
					heldTime += Time.fixedDeltaTime;
					break;
					//goto case CurrentPlaneState.UpdatePlanePosition;
				//This state is used by the others when they need the plane's ring position to be updated
				case CurrentPlaneState.UpdatePlanePosition:
					// Takes in radius and places cube based on its current time and moves it to new position
					frame.transform.localPosition = photoHolder.transform.position;
					frame.transform.localRotation = photoHolder.transform.rotation;
					spinningPosition = new Vector3(
						                   Mathf.Sin(elapsedTime + TimeModifier) * CurrentRadius,
						                   RingHeight,
						                   Mathf.Cos(elapsedTime + TimeModifier) * CurrentRadius)
					                   + transform.parent.transform.position;
					break;
			}

			gameObject.transform.LookAt(Camera.main.transform);
			//Adjusts the planes by 90 degrees in the x direction to make the plane's image face the viewer
			//rather than the bottom of each plane
			gameObject.transform.Rotate(0, 90, 0);
		}

		/// <summary>
		/// Changes the object into an exploding state, sent outward from a given center
		/// </summary>
		/// <param name="explosionCenter">Where the plane should start from</param>
		/// <param name="mustBeHeld">If it needs to be in the held state to explode</param>
		public void Explode(Vector3 explosionCenter, bool mustBeHeld = false)
		{
			if (!mustBeHeld || activePlaneState == CurrentPlaneState.Held)
			{
				activePlaneState = CurrentPlaneState.Exploding;
				ExplosionCenter = explosionCenter;
				animationProgress = 0;
			}
		}

		/// <summary>
		/// Gives information about the image plane's current state
		/// </summary>
		public void SayState()
		{
			Debug.Log(activePlaneState + " " + animationProgress + " " + GrabPoint);
		}

		/// <summary>
		/// Sets a point for the plane to travel to and stay at until pushed away
		/// </summary>
		/// <param name="newPoint">The point where the user grabbed</param>
		public void Grab(Vector3 newPoint)
		{
			if (newPoint == Vector3.zero)
			{
				return;
			}

			//If the position it is travelling to is closer the plane travels faster
			travelSpeedMultiplier = Mathf.Max(1, 1 / (transform.position - newPoint).magnitude);
			//Can only grab the plane in one of three states, while it's spinning, travelling to a held position or held
			if (activePlaneState == CurrentPlaneState.Spinning
			    || activePlaneState == CurrentPlaneState.ToBeHeld
			    || activePlaneState == CurrentPlaneState.Held)
			{
				//resets animation progress at sets the plane to the "To be held" state
				animationProgress = 0;
				GrabPoint = newPoint;
				activePlaneState = CurrentPlaneState.ToBeHeld;
			}
		}

		public void ManipulatonStarted(ManipulationEventData manipulationEventData)
        {
			activePlaneState = CurrentPlaneState.Held;
			GrabPoint = manipulationEventData.PointerCentroid;
			isBeingManipulated = true;
			releaseTimer = 0;
        }

		public void ManipulationEnded(ManipulationEventData manipulationEventData)
        {
			isBeingManipulated = false;
        }

		public void HoverEntered(ManipulationEventData manipulationEventData)
        {
			isHovered = true;
			GrabPoint = manipulationEventData.ManipulationSource.transform.position;
        }

		public void HoverExited(ManipulationEventData manipulationEventData)
        {
			isHovered = false;
			releaseTimer = 0;

        }
	}
}