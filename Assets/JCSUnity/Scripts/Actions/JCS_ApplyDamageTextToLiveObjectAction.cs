﻿/**
 * $File: JCS_ApplyDamageTextToLiveObjectAction.cs $
 * $Date: $
 * $Revision: $
 * $Creator: Jen-Chieh Shen $
 * $Notice: See LICENSE.txt for modification and distribution information 
 *                   Copyright (c) 2016 by Shen, Jen-Chieh $
 */
using UnityEngine;
using System.Collections;


namespace JCSUnity
{

    /// <summary>
    /// Apply the damage to live object by automatically.
    /// </summary>
    [RequireComponent(typeof(JCS_AttackerInfo))]
    public class JCS_ApplyDamageTextToLiveObjectAction
        : MonoBehaviour
    {

        //----------------------
        // Public Variables

        //----------------------
        // Private Variables

        
        [Header("** Check Variables (JCS_ApplyDamageTextToLiveObjectAction) **")]

        [SerializeField] private bool mInSequence = false;
        [SerializeField] private int mHit = 1;
        
        [SerializeField] private int mMinDamage = 1;
        [SerializeField] private int mMaxDamage = 5;
        [SerializeField] private int mCriticalChance = 10;


        [Header("** Pre Calculate Effect (JCS_ApplyDamageTextToLiveObjectAction) **")]

        [Tooltip("Enable this the attack will be calculate before hit the object.")]
        [SerializeField]
        private bool mPreCalculateEffect = false;

        [Tooltip("Damages that store in here ready to apply to the target.")]
        [SerializeField]
        private int[] mDamageApplying = null;


        [Header("** Lock Effect (JCS_ApplyDamageTextToLiveObjectAction) **")]

        [Tooltip("Enable/Disable the effect.")]
        [SerializeField]
        private bool mOnlyWithTarget = false;

        [Tooltip("Target we lock on!")]
        [SerializeField]
        private Transform mTargetTransform = null;


        [Header("** Runtime Variables (JCS_ApplyDamageTextToLiveObjectAction) **")]

        // Ability Format
        [Tooltip("Ability decide the min and max damage possibility.")]
        [SerializeField]
        private JCS_AbilityFormat mAbilityFormat = null;

        // Offset
        [Tooltip("Position + this.Offset where damage text will spawn.")]
        [SerializeField]
        private Vector3 mDamageTextPositionOffset = Vector3.zero;


        [Header("** Random Effect (JCS_ApplyDamageTextToLiveObjectAction) **")]

        [Tooltip("Enable/Disable Random Position Effect")]
        [SerializeField]
        private bool mRandPos = false;
        
        [Tooltip("Range will be within this negative to positive!")]
        [SerializeField] [Range(0, 10)]
        private float mRandPosRange = 0;


        [Header("** Destroy Setting (JCS_ApplyDamageTextToLiveObjectAction) **")]

        [SerializeField]
        private bool mDestroyByThisAction = true;


        [Header("** AOE Effect (JCS_ApplyDamageTextToLiveObjectAction) **")]

        [Tooltip("Make object un-destroyable, count down by AOECount below.")]
        [SerializeField] private bool mIsAOE = false;

        [Tooltip("Once the object hit a object count one down.")]
        [SerializeField] [Range(1, 15)]
        private int mAOECount = 7;

        // record down the aoe cound!
        private int mRecordAOECount = 0;

        private JCS_AttackerInfo mAttackerInfo = null;

        // every time in a frame, one bullet can shoot 
        // multiple target. To prevent this happen make
        // a boolean handler the detection.
        private bool mIsDestroyed = false;


        [Header("** Sound Settings (JCS_ApplyDamageTextToLiveObjectAction) **")]

        [Tooltip(@"Usage: play this hit sound, while is action happens.")]
        [SerializeField]
        private AudioClip mHitSound = null;


        //----------------------
        // Protected Variables

        //========================================
        //      setter / getter
        //------------------------------
        public Transform TargetTransform { get { return this.mTargetTransform; } set { this.mTargetTransform = value; } }
        public int Hit { get { return this.mHit; } set { this.mHit = value; } }
        public JCS_AbilityFormat AbilityFormat { get { return this.mAbilityFormat; } set { this.mAbilityFormat = value; } }
        public bool InSequence { get { return this.mInSequence; } set { this.mInSequence = value; } }

        public int[] DamageApplying { get { return this.mDamageApplying; } set { this.mDamageApplying = value; } }
        public JCS_AttackerInfo AttackerInfo { get { return this.mAttackerInfo; } set { this.mAttackerInfo = value; } }

        //========================================
        //      Unity's function
        //------------------------------
        private void Awake()
        {
            mAttackerInfo = this.GetComponent<JCS_AttackerInfo>();

            this.mRecordAOECount = this.mAOECount;
        }

        private void OnEnable()
        {
            // relief the aoe count
            mAOECount = mRecordAOECount;

            mIsDestroyed = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (mIsDestroyed)
                return;

            // do not target itself.
            if (other.transform == mAttackerInfo.Attacker)
                return;


            if (mInSequence)
            {
                if (mDestroyByThisAction)
                {
                    if (mTargetTransform == other.transform)
                        Destroy(this.gameObject);
                }

                return;
            }

            JCS_2DLiveObject liveObject = other.GetComponent<JCS_2DLiveObject>();

            // doing the lock on effect
            if (mOnlyWithTarget)
            {
                // if the target isn't what we want ignore than.
                if (mTargetTransform != other.transform)
                    return;
                else {

                    if (liveObject != null)
                    {
                        JCS_2DTrackAction tact = this.GetComponent<JCS_2DTrackAction>();

                        // only the last bullet in sequence will check dead.
                        if (tact != null)
                        {
                            if (tact.OrderIndex == Hit)
                            {
                                liveObject.BeenTarget = false;
                                liveObject.CheckDie();
                            }
                        }
                    }

                    DestroyWithAction();
                }
            }
            // if not in sequence
            else
            {
                if (liveObject == null)
                    return;

                if (!liveObject.CanDamage)
                {
                    return;
                }

                // liveObject is already dead.
                if (liveObject.IsDead())
                {
                    //DestroyWithAction();
                    return;
                }
            }

            Transform attacker = mAttackerInfo.Attacker;
            if (attacker != null)
            {
                JCS_2DLiveObject owenerLiveObject = mAttackerInfo.Attacker.GetComponent<JCS_2DLiveObject>();
                if (owenerLiveObject != null)
                {
                    if (!JCS_GameSettings.instance.TRIBE_DAMAGE_EACH_OTHER)
                    {
                        // if both player does not need to add in to list.
                        // or if both enemy does not need to add in to list.
                        if (liveObject.IsPlayer == owenerLiveObject.IsPlayer)
                            return;
                    }
                }
            }

            if (mAbilityFormat != null)
            {
                mMinDamage = mAbilityFormat.GetMinDamage();
                mMaxDamage = mAbilityFormat.GetMaxDamage();
                mCriticalChance = mAbilityFormat.GetCriticalChance();
            }
            else {
                JCS_Debug.LogReminders(
                    "JCS_ApplyDamageTextToLiveObjectAction", 
                    "You sure to not using any \"JCS_AbilityFormat\"?");
            }

            Vector3 currentPos = liveObject.transform.position + mDamageTextPositionOffset;

            if (mRandPos)
                AddRandomPosition(ref currentPos);

            if (mPreCalculateEffect)
            {
                liveObject.ApplyDamageText(
                    this.mAttackerInfo.Attacker,
                    mDamageApplying,
                    currentPos,
                    mCriticalChance,
                    mHitSound);

                mHit = this.mDamageApplying.Length;
            }
            else
            {
                liveObject.ApplyDamageText(
                    this.mAttackerInfo.Attacker,
                    mMinDamage,
                    mMaxDamage,
                    currentPos,
                    mHit,
                    mCriticalChance, 
                    mHitSound);
            }

            // only if there is only one hit need to do this.
            if (mHit == 1)
                liveObject.BeenTarget = false;

            liveObject.CheckDie();

            DestroyWithAction();
        }

        //========================================
        //      Self-Define
        //------------------------------
        //----------------------
        // Public Functions

        /// <summary>
        /// Copy some information from other.
        /// </summary>
        public void CopyToThis(JCS_ApplyDamageTextToLiveObjectAction copy)
        {
            this.AbilityFormat = copy.AbilityFormat;
            this.Hit = copy.Hit;
            this.AttackerInfo = copy.AttackerInfo;
        }

        //----------------------
        // Protected Functions

        //----------------------
        // Private Functions

        /// <summary>
        /// Randomize the position.
        /// </summary>
        /// <param name="currentPos"> current position to be randomize. </param>
        private void AddRandomPosition(ref Vector3 currentPos)
        {
            float addPos;
            Vector3 newPos = currentPos;

            addPos = Random.Range(-mRandPosRange, mRandPosRange);
            newPos.x += addPos;

            addPos = Random.Range(-mRandPosRange, mRandPosRange);
            newPos.y += addPos;

            currentPos = newPos;
        }

        /// <summary>
        /// Check aoe and decide to destroy 
        /// the game object now or later.
        /// </summary>
        private void DestroyWithAction()
        {
            // return if is aoe continue.
            if (CheckAOE())
                return;

            if (mDestroyByThisAction)
            {
                Destroy(this.gameObject);
            }

            // once is destroy, this action will 
            // not be active anymore.
            mIsDestroyed = true;
        }

        /// <summary>
        /// Check is the attack aoe?
        /// </summary>
        /// <returns> TRUE: is aoe, dont destroy the object
        /// FALSE: is not aoe or aoe count is less/equal to zero, 
        /// destroy the object. </returns>
        private bool CheckAOE()
        {
            // if we have aoe count left.
            if (0 < mAOECount)
            {
                // if aoe effect is on.
                if (mIsAOE)
                {
                    // count down the aoe count, 
                    --mAOECount;

                    // then return, dont destory the object, 
                    // before the aoe count down meet to less 
                    // or equal to zero.
                    return true;
                }
            }

            return false;
        }

    }
}
