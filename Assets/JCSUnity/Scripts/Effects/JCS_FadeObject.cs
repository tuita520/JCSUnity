﻿/**
 * $File: JCS_FadeObject.cs $
 * $Date: $
 * $Revision: $
 * $Creator: Jen-Chieh Shen $
 * $Notice: See LICENSE.txt for modification and distribution information 
 *                   Copyright (c) 2016 by Shen, Jen-Chieh $
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;


namespace JCSUnity
{
    public delegate void IsFadeOutCallback();
    public delegate void IsFadeInCallback();

    /// <summary>
    /// Fade alpha the object in particular value.
    /// </summary>
    public class JCS_FadeObject
        : JCS_UnityObject
    {

        //----------------------
        // Public Variables

        //----------------------
        // Private Variables
        private JCS_FadeType mFadeType = JCS_FadeType.FADE_IN;  // defaul as visible

        private float mAlpha = 1.0f;

        [Header("** Runtime Variables (JCS_FadeObject) **")]

        [Tooltip("How long it fade?")]
        [SerializeField]
        [Range(0, 60.0f)]
        private float mFadeTime = 1.0f;

        [Tooltip("Override the action before it complete the action.")]
        [SerializeField]
        private bool mOverriteFade = false;

        private bool mEffect = false;
        private bool mVisible = true;

        // callback after fadeout if complete
        private IsFadeOutCallback mIsFadeOutCallback = DefaultFadeCallback;

        // callback after fade in is complete.
        private IsFadeInCallback mIsFadeInCallback = DefaultFadeCallback;

        [Tooltip("Maxinum of fade amount of value.")]
        [SerializeField]
        [Range(0, 1)]
        private float mFadeInAmount = 1;

        [Tooltip("Mininum of fade amount of value.")]
        [SerializeField]
        [Range(0, 1)]
        private float mFadeOutAmount = 0;

        //----------------------
        // Protected Variables

        //========================================
        //      setter / getter
        //------------------------------
        public float FadeTime { get { return this.mFadeTime; } set { this.mFadeTime = value; } }
        public float Alpha { get { return this.mAlpha; } set { this.mAlpha = value; } }
        public void SetIsFadeOutCallback(IsFadeOutCallback func) { this.mIsFadeOutCallback = func; }
        public void SetIsFadeInCallback(IsFadeInCallback func) { this.mIsFadeInCallback = func; }

        //========================================
        //      Unity's function
        //------------------------------
        private void Awake()
        {
            UpdateUnityData();
        }

        private void Update()
        {
#if (UNITY_EDITOR)
            //Test();
#endif

            if (GetObjectType() == JCS_UnityObjectType.GAME_OBJECT &&
                JCS_GameManager.instance.GAME_PAUSE)
                return;

            if (!mEffect)
                return;


            switch (mFadeType)
            {
                case JCS_FadeType.FADE_OUT:
                    {
                        // Fade out effect complete
                        if (mAlpha < mFadeOutAmount)
                        {
                            switch (GetObjectType())
                            {
                                case JCS_UnityObjectType.GAME_OBJECT:
                                    {
                                        //this.gameObject.SetActive(false);
                                    }
                                    break;
                                case JCS_UnityObjectType.UI:
                                    {
                                        if (mImage != null)
                                            mImage.enabled = false;
                                    }
                                    break;
                                case JCS_UnityObjectType.SPRITE:
                                    {
                                        if (mSpriteRenderer != null)
                                            mSpriteRenderer.enabled = false;
                                    }
                                    break;
                                case JCS_UnityObjectType.TEXT:
                                    {
                                        if (mText != null)
                                            mText.enabled = false;
                                    }
                                    break;
                            }

                            // do fade out callback
                            mIsFadeOutCallback.Invoke();

                            mEffect = false;
                            return;
                        }

                        mAlpha -= Time.deltaTime / mFadeTime;
                    }
                    break;

                case JCS_FadeType.FADE_IN:
                    {
                        // Fade in effect complete
                        if (mAlpha > mFadeInAmount)
                        {
                            // do fade in callback
                            mIsFadeInCallback.Invoke();

                            mEffect = false;
                            return;
                        }

                        mAlpha += Time.deltaTime / mFadeTime;
                    }
                    break;
            }

            Color screenColor = this.LocalColor;
            screenColor.a = mAlpha;
            this.LocalColor = screenColor;

        }

#if (UNITY_EDITOR)
        private void Test()
        {
            if (Input.GetKeyDown(KeyCode.N))
                FadeOut();
            if (Input.GetKeyDown(KeyCode.M))
                FadeIn();
        }
#endif

        //========================================
        //      Self-Define
        //------------------------------
        //----------------------
        // Public Functions

            /// <summary>
            /// Is the fade object fade in?
            /// </summary>
            /// <returns>
            /// true : is fade in.
            /// false : not fade in yet.
            /// </returns>
        public bool IsFadeIn()
        {
            return (this.mAlpha >= mFadeInAmount);
        }

        /// <summary>
        /// Is the fade object fade out?
        /// </summary>
        /// <returns>
        /// true : is fade out.
        /// false : not fade out yet.
        /// </returns>
        public bool IsFadeOut()
        {
            return (this.mAlpha <= mFadeOutAmount);
        }

        public void FadeOut() { FadeOut(mFadeTime); }
        public void FadeIn() { FadeIn(mFadeTime); }
        public void FadeOut(float time) { FadeEffect(JCS_FadeType.FADE_OUT, time); }
        public void FadeIn(float time) { this.FadeEffect(JCS_FadeType.FADE_IN, time); }

        /// <summary>
        /// Default function to point to, 
        /// prevent null reference exception.
        /// to save check null pointer performance.
        /// </summary>
        public static void DefaultFadeCallback()
        {
            // empty.
        }

        //----------------------
        // Protected Functions

        //----------------------
        // Private Functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"> Type to fade </param>
        /// <param name="time"> time to fade in/out, in seconds </param>
        private void FadeEffect(JCS_FadeType type, float time)
        {
            if (!mOverriteFade)
            {
                // Check is already fade out or fade in!
                if ((mVisible && type == JCS_FadeType.FADE_IN) ||
                    (!mVisible && type == JCS_FadeType.FADE_OUT))
                    return;
            }

            // enable the effect component
            switch (GetObjectType())
            {
                // enable the shader
                case JCS_UnityObjectType.GAME_OBJECT:
                    {
                        //this.gameObject.SetActive(true);
                    }
                    break;
                // enable "Image" component
                case JCS_UnityObjectType.UI:
                    {
                        if (mImage != null)
                            mImage.enabled = true;
                    }
                    break;
                case JCS_UnityObjectType.SPRITE:
                    {
                        if (mSpriteRenderer != null)
                            mSpriteRenderer.enabled = true;
                    }
                    break;
                case JCS_UnityObjectType.TEXT:
                    {
                        if (mText != null)
                            mText.enabled = true;
                    }
                    break;
            }

            switch (type)
            {
                case JCS_FadeType.FADE_OUT:
                    {
                        mAlpha = mFadeInAmount;
                        this.mVisible = false;
                    }
                    break;
                case JCS_FadeType.FADE_IN:
                    {
                        mAlpha = mFadeOutAmount;
                        this.mVisible = true;
                    }
                    break;
            }

            this.mFadeTime = time;
            this.mFadeType = type;
            this.mEffect = true;
        }

    }
}
