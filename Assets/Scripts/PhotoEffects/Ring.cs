using System;
using System.Collections.Generic;
using Phonado.Core;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
//using UnityEngine.XR.MagicLeap;

namespace Phonado.PhotoEffects
{
    public class Ring : MonoBehaviour
    {

        [HideInInspector]
        public RingManager Parent;

        //Storing all cubes int collection to be able to manipulate all of them
        [HideInInspector]
        [FormerlySerializedAs("cubes")]
        public List<Holder> PhotoHolders;

        [FormerlySerializedAs("CubeAmount")]
        public int PlaneAmount;

        [HideInInspector] 
        public float RingHeight;
        /// <summary>
        /// Should only be used to initialize, changing this value after rings are created will not update rings
        /// </summary>
        public float StartingRadius = 5f;

        /// <summary>
        /// How much the ring's time value should update by for each update frame
        /// </summary>
        public float TimeRate;
        /// <summary>
        /// Field to adjust ring radius during runtime
        /// </summary>
        public float CurrentRadius;

        /// <summary>
        /// If the planes should be scaled to match the ratio of the picture they are showing
        /// </summary>
        public bool ScalePlanesToImageRatios;

        /// <summary>
        /// How fast the planes should explode out from the center
        /// </summary>
        public float ExplosionSpeed;

        /// <summary>
        /// The current value for time to be used to calculate the positions of the cube
        /// </summary>
        private float currentTime = 0f;

        /// <summary>
        /// The previous value of time, if there is no change in time we don't need to recalcuate the cube postions.
        /// </summary>
        private float lastTime = 0f;

        //made to check if radius has been updated, if this is the same as current radius, no update was made
        private float lastRadius;

        private GameObject holderPrefab;

        public GameObject HolderObject;

        public bool DefaultRing;

        public Texture2D DefaultTexture;

        public void Start()
        {
            holderPrefab = Resources.Load<GameObject>("PhotoHolder");
            lastRadius = CurrentRadius;
            //spawning as many planes as set
            for (int i = 0; i < PlaneAmount; i++)
            {
                var planeToAdd = Instantiate(holderPrefab, this.transform);

                var holderScript = planeToAdd.GetComponent<Holder>();

                Texture2D image;
                string nameOfImage = "";
                if (DefaultRing)
                {
                    image = DefaultTexture;
                }
                else
                {
                    var sprite = ResourceLoader.SelectNextImage(); 
                    image = sprite.texture;
                    nameOfImage = sprite.name;
                }

                //getting ratio of time starting from beginning of sin/cos wave
                //that will adjust each cubes spawn position based on which cube its currently on
                float timeModifier = (Mathf.PI * 2 * i) / PlaneAmount;

                //Sets all values of the plane
                holderScript.SetStartingParameters(timeModifier, RingHeight, CurrentRadius,
                    TimeRate, ScalePlanesToImageRatios, image, nameOfImage, 
                    ExplosionSpeed, HolderObject);

                PhotoHolders.Add(holderScript);
                if (DefaultRing)
                {
                    holderScript.Explode(transform.position);
                }
            }
        }

        /// <summary>
        /// For each plane on the ring, calls it's explosion function, which will send it from it's center
        /// to it's position on the ring
        /// </summary>
        public void Explode()
        {
            foreach (var holder in PhotoHolders)
            {
                holder.Explode(transform.position);
            }
        }
    }
}