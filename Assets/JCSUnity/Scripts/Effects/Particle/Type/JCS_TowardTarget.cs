/**
 * $File: JCS_TowardTarget.cs $
 * $Date: 2016-11-12 21:27:25 $
 * $Revision: $
 * $Creator: Jen-Chieh Shen $
 * $Notice: See LICENSE.txt for modification and distribution information 
 *	                 Copyright (c) 2016 by Shen, Jen-Chieh $
 */
using UnityEngine;
using System.Collections;


namespace JCSUnity
{

    /// <summary>
    /// Particle will lerp to the target position.
    /// </summary>
    [RequireComponent(typeof(JCS_TransfromTweener))]
    [RequireComponent(typeof(JCS_DisableWithCertainRangeEvent))]
    public class JCS_TowardTarget
        : JCS_Particle
    {

        //----------------------
        // Public Variables

        //----------------------
        // Private Variables

        // tweener effect to the object.
        private JCS_TransfromTweener mJCSTweener = null;

        // when reach the certain range disable it.
        private JCS_DisableWithCertainRangeEvent mDisableWidthCertainRangeEvent = null;

        

        [Header("** Runtime Variables (JCS_TowardTarget) **")]

        [Tooltip("Toward this target.")]
        [SerializeField]
        private Transform mTargetTransform = null;

        [Tooltip("Random Position when active.")]
        [SerializeField] [Range(0, 1000)]
        private float mRange = 10;

        [Tooltip("Range will be adjustable.")]
        [SerializeField] [Range(0, 1000)]
        private float mAdjustRange = 0;

        [Tooltip("Is 3D particle?")]
        [SerializeField]
        private bool mIncludeDepth = false;

        //----------------------
        // Protected Variables

        //========================================
        //      setter / getter
        //------------------------------
        public void SetTargetTransfrom(Transform trans)
        {
            // update target position.
            this.mTargetTransform = trans;
        }
        public float Range { get { return this.mRange; } set { this.mRange = value; } }
        public float AdjustRange { get { return this.mAdjustRange; } set { this.mAdjustRange = value; } }
        public bool IncludeDepth { get { return this.mIncludeDepth; } set { this.mIncludeDepth = value; } }

        //========================================
        //      Unity's function
        //------------------------------
        private void Awake()
        {
            this.mJCSTweener = this.GetComponent<JCS_TransfromTweener>();
            this.mDisableWidthCertainRangeEvent = this.GetComponent<JCS_DisableWithCertainRangeEvent>();

            // set destination callback.
            mJCSTweener.SetCallback(DestinationCallback);
        }

        private void Start()
        {
            // set the target transform.
            this.mJCSTweener.SetTargetTransform(this.mTargetTransform);
            this.mDisableWidthCertainRangeEvent.SetTargetTransfrom(this.mTargetTransform);
        }

        private void OnEnable()
        {
            if (mTargetTransform == null)
            {
                JCS_Debug.LogError(
                    this, "Cannot set the calculate circle position with null target transform...");

                return;
            }

            // on enable set the random position
            // with in the circle range.

            // get the position.
            Vector3 newPos = CalculateCirclePosition();

            // set to the position.
            SetPosition(newPos);

            mJCSTweener.UpdateUnityData();

            // reset alpha change.
            mJCSTweener.LocalAlpha = 1.0f;

            // enable the sprite renderer component.
            mJCSTweener.LocalEnabled = true;

            // reset tweener
            mJCSTweener.ResetTweener();

            // update the unity data first.
            mJCSTweener.DoTweenContinue(this.mTargetTransform);
        }

        //========================================
        //      Self-Define
        //------------------------------
        //----------------------
        // Public Functions

        //----------------------
        // Protected Functions

        //----------------------
        // Private Functions

        /// <summary>
        /// Get the random position within the 
        /// certain range. (Circle)
        /// </summary>
        private Vector3 CalculateCirclePosition()
        {
            return CalculateCirclePosition(
                mTargetTransform.position, 
                mRange, 
                mAdjustRange);
        }

        /// <summary>
        /// Get the random position within the 
        /// certain range. (Circle)
        /// </summary>
        /// <param name="targetPos"> target position </param>
        /// <param name="range"> radius of circle </param>
        /// <param name="adjRange"> adjustable value? </param>
        /// <returns></returns>
        private Vector3 CalculateCirclePosition(Vector3 targetPos, float range, float adjRange = 0)
        {
            Vector3 newPos = mTargetTransform.position;

            // set up the unknown angle
            // �H��"����"
            float angle = JCS_Random.Range(0, 360);

            // define offset
            float hypOffset = JCS_Random.Range(
                    -adjRange,
                    adjRange);

            // add offset to current distance (hyp)
            float hyp = range + hypOffset;

            float opp = Mathf.Sin(angle) * hyp;

            float adj = JCS_Mathf.PythagoreanTheorem(hyp, opp, JCS_Mathf.TriSides.adj);
            

            bool flipX = JCS_Mathf.IsPossible(50);
            if (flipX)
                newPos.x -= adj;
            else
                newPos.x += adj;


            bool flipY = JCS_Mathf.IsPossible(50);
            if (flipY)
                newPos.y -= opp;
            else
                newPos.y += opp;


            if (mIncludeDepth)
            {
                bool flipZ = JCS_Mathf.IsPossible(50);

                if (flipZ)
                    newPos.z -= opp;
                else
                    newPos.z += opp;
            }

            return newPos;
        }

        /// <summary>
        /// Callback when reach the destination.
        /// </summary>
        private void DestinationCallback()
        {
            this.gameObject.SetActive(false);
        }

    }
}
