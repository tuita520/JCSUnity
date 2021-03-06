﻿/**
 * $File: JCS_IGLogSystem.cs $
 * $Date: $
 * $Revision: $
 * $Creator: Jen-Chieh Shen $
 * $Notice: See LICENSE.txt for modification and distribution information 
 *	                 Copyright (c) 2016 by Shen, Jen-Chieh $
 */
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace JCSUnity
{
    /// <summary>
    /// In Game Log System (IGL).
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(JCS_GUITextPool))]
    public class JCS_IGLogSystem
        : JCS_UnityObject
    {
        /* Variables */

        private JCS_GUITextPool mLogTextPool = null;

        [Header("** Initialize Variables (JCS_IGLogSystem) **")]

        [Tooltip("Space between each log message.")]
        [SerializeField] [Range(-300.0f, 300.0f)]
        private float mLogSpacing = 1;

        // vector of log text rendering on the screen.
        private JCS_Vector<JCS_LogText> mRenderLogText = null;

        
        /* Setter & Getter */

        /* Functions */

        protected override void Awake()
        {
            base.Awake();

            this.mLogTextPool = this.GetComponent<JCS_GUITextPool>();

            mRenderLogText = new JCS_Vector<JCS_LogText>();
        }

        private void Start()
        {
            // set to utility manager.
            JCS_UtilitiesManager.instance.SetIGLogSystem(this);
        }

#if (UNITY_EDITOR)
        private void Update()
        {
            Test();
        }

        private void Test()
        {
            if (JCS_Input.GetKeyDown(KeyCode.Q))
                SendLogMessage("Hello World!");
            if (JCS_Input.GetKeyDown(KeyCode.W))
                SendLogMessage("EXP: 10");
            if (JCS_Input.GetKeyDown(KeyCode.E))
            {
                string[] msgs = { "Ok", "Cool?", "Awesome!" };

                SendLogMessages(msgs);
            }
        }
#endif

        /// <summary>
        /// Make single log message on the screen.
        /// </summary>
        /// <param name="message"></param>
        public void SendLogMessage(string message)
        {
            // get one log text from the pool
            JCS_LogText logText = mLogTextPool.ExecuteOneFromPool();

            // all pool active?
            if (logText == null)
                return;

            UpdateSpace();

            mRenderLogText.push(logText);

            Vector3 newPos = logText.SimpleTrackAction.TargetPosition;

            // set back to position.
            // NOTE(JenChieh): 不太懂這邊的原理...
            newPos.y = 0;

            logText.SimpleTrackAction.TargetPosition = newPos;
            logText.SimpleTrackAction.LocalPosition = newPos;

            // this will set the log text active, 
            // so it wont be reuse until is fade out.
            logText.Execute(message);
        }

        /// <summary>
        /// Make multiple log messages on the screen.
        /// </summary>
        /// <param name="messages"></param>
        public void SendLogMessages(string[] messages)
        {
            foreach (string msg in messages)
            {
                SendLogMessage(msg);
            }
        }

        /// <summary>
        /// Remove the log message that are outdated.
        /// </summary>
        /// <param name="txt"></param>
        public void RemoveFromRenderVec(JCS_LogText txt)
        {
            mRenderLogText.slice(txt);
        }

        /// <summary>
        /// Update all current active log messages' spacing.
        /// </summary>
        /// <param name="spaces"></param>
        public void UpdateSpace(int spaces = 1)
        {
            JCS_LogText logText = null;

            for (int index = 0;
                index < mRenderLogText.length;
                ++index)
            {
                logText = mRenderLogText.at(index);

                logText.SimpleTrackAction.DeltaTargetPosY(mLogSpacing * spaces);
            }
        }

        /// <summary>
        /// move all the log text up one space.
        /// </summary>
        private void IncSpacing()
        {
            for (int index =0;
                index < mRenderLogText.length;
                ++index)
            {
                mRenderLogText.at(index);
            }
        }

        /// <summary>
        /// Active one log text object.
        /// </summary>
        private void ActiveOneText()
        {
            // get one log text from the pool
            mLogTextPool.ExecuteOneFromPool();
        }
    }
}
