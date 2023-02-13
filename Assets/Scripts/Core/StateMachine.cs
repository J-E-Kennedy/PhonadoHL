using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.XR.MagicLeap;
using Phonado.PhotoEffects;
using Phonado.Enums;
using Phonado.Logging;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.XR;

namespace Phonado.Core
{
    public class StateMachine : MonoBehaviour
    {
        /// <summary>
        /// Reference to manager that gets created when orb reaches its location
        /// </summary>
        private RingManager managerRef;

        /// <summary>
        /// Flag to stop explosion from happening multiple times
        /// </summary>
        private bool exploding = true;

        /// <summary>
        /// Storing time as program runs in case it needs to be used at any time
        /// </summary>
        private float currentTime;

        /// <summary>
        /// Reference to the line that is displayed with the controller to be able to set fields as its created
        /// </summary>
        //private DisplayControl displayer;

        /// <summary>
        /// Reference to orb motion script to be able to 
        /// </summary>
        private OrbMotion motionScript;

        /// <summary>
        /// Reference to controller bindings
        /// </summary>
        private ControllerActions controllerActions;

        //private MLSpatialMapper mapper;
        
        [SerializeField] private AnimationStates currentState;

        /// <summary>
        /// Flag used to figure out if clearing of UI assets is done
        /// </summary>
        private bool finishedErrorUISetup = false;

        /// <summary>
        /// Flag to know if user wants to reset experience
        /// </summary>
        private bool hitBumper = false;

        #region Prefabs

        private GameObject orbObject;
        private GameObject collisionSphere;
        private GameObject ringManager;
        private GameObject sphere;

        #endregion Prefabs

        /// <summary>
        /// Song set in editor
        /// </summary>
        public AudioSource Source;

        /// <summary>
        /// HeadposeCanvas set in editor
        /// </summary>
        public Canvas HeadCanvas;

        public float PositionLogFrequency;

        public Highlighter Highlighter;
        
        private float TimeSinceLastLog;

        private bool Log = false;


        void Start()
        {
            //MLInput.OnControllerButtonDown += OnBumperDown;
            //MLInput.OnControllerButtonUp += OnBumperUp;
            if (Highlighter == null)
            {
                Debug.Log("No highlighter attached");
                return;
            }
            
            //displayer = gameObject.AddComponent<DisplayControl>();
            //controllerActions = GetComponent<ControllerActions>();
            //if (controllerActions.RightIsConnected)
            //{
            //    //displayer.controllerRef = controllerActions;
            //    controllerActions.RightTriggerFullPress.AddListener(OnTriggerPress);
            //    controllerActions.RightBumperPressed.AddListener(OnBumperDown);
            //    controllerActions.RightBumperRelease.AddListener(OnBumperUp);
            //}

            orbObject = Resources.Load<GameObject>("Book");
            collisionSphere = Resources.Load<GameObject>("Sphere");
            ringManager = Resources.Load<GameObject>("RingManager");
            //data = Camera.main.GetComponentInChildren<FrequencyData>();
            //mapper = gameObject.GetComponentInChildren<MLSpatialMapper>();
            if (ResourceLoader.ImageCount <= 0)
            {
                currentState = AnimationStates.CantStart;
            }
            else
            {
                currentState = AnimationStates.OrbStart;
            }
            OnTriggerPress();
            TimeSinceLastLog = 0;
            GetSessionInfo();
        }

        /// <summary>
        /// Currently using trigger to begin experience, can also be changed to use a gesture
        /// </summary>
        private void OnTriggerPress()
        {
            if (currentState == AnimationStates.OrbStart)
            {
                Debug.Log("Pulled trigger");
                SetupPhonado(true);
            }
        }


        /// <summary>
        /// Bumper is currently being used to change states in state machine as a temporary
        /// restart, eventually this will not be used anymore.
        /// </summary>
        private void OnBumperUp()
        {
            if (hitBumper)
            {
                Source.Stop();
                currentState = AnimationStates.OrbStart;
                Highlighter.UnsetTargets();
                Destroy(managerRef.gameObject);
                Destroy(motionScript.gameObject);
                Destroy(sphere.gameObject);
                //displayer.lineRend.enabled = true;
                HeadCanvas.gameObject.SetActive(true);

                hitBumper = false;
                Logger.EndTime = DateTime.UtcNow;
                Logger.EndReason = "Session restarted";
                Logger.writeToFile();
                Logger.StartTime = DateTime.UtcNow;

                //Application.Quit();
            }
        }

        /// <summary>
        /// Storing state of bumper press to register on bumper release
        /// </summary>
        private void OnBumperDown()
        {
            if (currentState == AnimationStates.RingMotion)
            {
                hitBumper = true;
            }
        }

        /// <summary>
        /// Going through the canvas and disabling any elements that are there so only the error text is shown
        /// </summary>
        /// <returns>Count of UI elements to use in spawning new UI element on last element</returns>
        private int ClearHeadPoseElements()
        {
            int uiItemCount = HeadCanvas.transform.childCount;
            for (int i = 0; i < uiItemCount; i++)
            {
                var currentParentObject = HeadCanvas.transform.GetChild(i);
                int uiElementsCount = currentParentObject.childCount;
                for (int j = 0; j < uiElementsCount; j++)
                {
                    currentParentObject.GetChild(j).gameObject.SetActive(false);
                }
            }

            return uiItemCount;
        }

        /// <summary>
        /// Setting up application depending on whether we are on the headset, or running in the editor for testing
        /// </summary>
        /// <param name="onHeadset">Set true if on the headset, set false if not</param>
        private void SetupPhonado(bool onHeadset)
        {
            Vector3 cameraPos = Camera.main.transform.position;
            //motionScript = Instantiate(orbObject, controllerActions.RightControllerPosition, Quaternion.identity).GetComponent<OrbMotion>();
            //motionScript.gameObject.transform.LookAt(controllerActions.RightControllerPosition);
            motionScript = Instantiate(orbObject, cameraPos, Quaternion.identity).GetComponent<OrbMotion>();
            motionScript.gameObject.transform.LookAt(cameraPos);
            Vector3 endPoint = new Vector3();
            if (onHeadset)
            {
                //endPoint = displayer.selectedEndPoint;
                //displayer.lineRend.enabled = false;
            }
            else
            {
                endPoint = new Vector3(cameraPos.x, cameraPos.y, cameraPos.z + 1 * 5);
            }

            //end point is set based on whether or not we are on the headset or not
            
            sphere = Instantiate(collisionSphere, endPoint,
                Quaternion.identity);
            var spherePosition = sphere.transform.position;
            motionScript.TargetPos = spherePosition;
            Logger.Ball = new BallLogDto()
            {
                LaunchTime = DateTime.UtcNow,
                XLaunch = spherePosition.x,
                YLaunch = spherePosition.y,
                ZLaunch = spherePosition.z
            };

            HeadCanvas.gameObject.SetActive(false);
            currentState = AnimationStates.OrbMoving;
        }

        /// <summary>
        /// Checking different states to see where we currently are in application
        /// </summary>
        void Update()
        {
            var headPose = Camera.main.transform.position;
            if (Log)
            {
                TimeSinceLastLog += Time.fixedDeltaTime;
                if (TimeSinceLastLog >= PositionLogFrequency)
                {
                    TimeSinceLastLog = 0;
                    Logger.Positions.Add(new PositionLogDto()
                    {
                        X = headPose.x,
                        Y = headPose.y,
                        Z = headPose.z,
                        Time = DateTime.UtcNow
                    });
                }
            }

            //if (Input.GetKeyUp(KeyCode.Space))
            //{
            //    OnBumperDown();
            //    hitBumper = true;
            //    OnBumperUp();
            //}

            switch (currentState)
            {
                case AnimationStates.CantStart:
                    //state will be set to this if no images are found in the resources
                    if (!finishedErrorUISetup)
                    {
                        //keeping track of how many items were disabled
                        int uiItemCount = ClearHeadPoseElements();
                        //getting last object and using it to put the errortext on
                        GameObject borrowedParentObject = HeadCanvas.transform.GetChild(uiItemCount - 1).gameObject;
                        var errorObject =
                            Instantiate(Resources.Load("ErrorText"), borrowedParentObject.transform) as GameObject;
                        errorObject.SetActive(true);
                        finishedErrorUISetup = true;
                    }

                    break;
                case AnimationStates.OrbStart:
                    //checking if controller trigger is pressed, will switch to orb moving and send orb forwards
                    //if (!controllerActions.LeftIsConnected && !controllerActions.RightIsConnected)
                    //{
                    //    if (Input.GetMouseButtonDown(0))
                    //    {
                    //        SetupPhonado(false);
                    //    }
                    //}

                    break;
                case AnimationStates.OrbMoving:
                    //check for when orb gets to desired position
                   
                    if (!motionScript.Moving)
                    {
                        Source.Play();
                        //motionScript.gameObject.SetActive(false);
                        //sphere.gameObject.SetActive(false);
                        var bookOpening = motionScript.gameObject.GetComponent<BookOpening>();
                        if (bookOpening != null)
                        {
                            bookOpening.OpenBook = true;
                        }
                        currentState = AnimationStates.RingTransition;
                    }

                    break;
                /*
                case AnimationStates.WaitingToPlay:
                    //waiting for user to move closer to orb to explode and play song

                    break;
                case AnimationStates.OrbExplosions:
                    //will most likely switch states based on amount of time passed
                    break;
                */
                case AnimationStates.RingTransition:
                    //pulling pictures to ring holder to start ring holder spawning
                    GameObject manager = Instantiate(ringManager);
                    manager.transform.position = motionScript.gameObject.transform.position;
                    managerRef = manager.GetComponent<RingManager>();
                    //FreqDataSetter setter = manager.GetComponent<FreqDataSetter>();
                    //setter.FreqData = data;
                    currentState = AnimationStates.RingMotion;
                    break;
                case AnimationStates.RingMotion:
                    //will most likely finish when program ends?
                    var spherePosition = sphere.transform.position;
                    Logger.Ball.LandTime = DateTime.UtcNow;
                    Logger.Ball.XLand = spherePosition.x;
                    Logger.Ball.YLand = spherePosition.y;
                    Logger.Ball.ZLand = spherePosition.z;
                    managerRef.Explode();
                    break;
            }

            currentTime += Time.deltaTime;
        }

        public void GetSessionInfo()
        {
            Logger.StartTime = DateTime.UtcNow;
            Logger.Location = "Par-D Lab";
            Logger.UserID = 1;
            Logger.ExperienceID = 1;
            Logger.Ball = new BallLogDto();
            Logger.EyeTrackings = new HashSet<EyeTrackingLogDto>();
            Logger.Positions = new List<PositionLogDto>();
            Logger.SessionPhotos = new List<SessionPhotoLogDto>();
            Logger.Grabs = new List<GrabLogDto>();
        }
    }
}