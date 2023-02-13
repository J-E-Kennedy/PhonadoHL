using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Phonado.Core
{
    public class BookOpening : MonoBehaviour
    {
        public bool OpenBook = false;
        
        private bool startOpen = false;
        private float animationProgress = 0;
        private float progressionAmount = 0.01f;
        private GameObject firstPiece;
        private Vector3 spinePoint = new Vector3(0.0f,0.01f,0f);
        private List<GameObject> parts;
        void Start()
        {
            bool first = true;
            parts = new List<GameObject>();
            foreach (Transform t in transform)
            {
                parts.Add(t.gameObject);
                if (first)
                {
                    first = false;
                    firstPiece = t.gameObject;
                }
            }

            Debug.Log(parts.Count);
            ;
        }

        // Update is called once per frame
        void Update()
        {
            if (OpenBook)
            {
                if (!startOpen)
                {
                    startOpen = true;
                    var t = firstPiece.transform;
                    spinePoint = new Vector3(t.position.x + t.localScale.x / 2 , t.position.y, t.position.z);
                }
                if (animationProgress < 2)
                {

                    if (animationProgress < 1)
                    {
                        int partNumber = 0;
                        foreach (GameObject part in parts)
                        {
                            part.transform.RotateAround(spinePoint, Vector3.forward, progressionAmount * -180f * partNumber / (parts.Count - 1));
                            partNumber++;
                        }

                    }
                
                    animationProgress += progressionAmount;
                
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
            
            
        }
    }
}