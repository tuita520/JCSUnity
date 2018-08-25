﻿/**
 * $File: JCS_UIManager.cs $
 * $Date: $
 * $Revision: $
 * $Creator: Jen-Chieh Shen $
 * $Notice: See LICENSE.txt for modification and distribution information 
 *                   Copyright (c) 2016 by Shen, Jen-Chieh $
 */
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace JCSUnity
{
    /// <summary>
    /// Manage all the dialogue in the scene.
    /// </summary>
    public class JCS_UIManager 
        : JCS_Managers<JCS_UIManager>
    {

        //----------------------
        // Public Variables

        //----------------------
        // Private Variables

        [Header("** Check Variables (JCS_UIManager) **")]

        [Tooltip("Global undo redo system.")]
        [SerializeField]
        private JCS_UndoRedoSystem mGlobalUndoRedoSystem = null;


        [Header("** Initialize Variables (JCS_UIManager) **")]

        [Tooltip("Game Play UI (Game Layer - Only One)")]
        [SerializeField]
        private JCS_DialogueObject mGameUI = null;          // the most common one!

        [Tooltip("System dialogue (Application Layer - Only One)")]
        [SerializeField]
        private JCS_DialogueObject mForceDialogue = null;

        // Game Dialogue (Game Layer - could have multiple one)
        [Tooltip("Dialogue player are focusing")]
        [SerializeField]
        private JCS_DialogueObject mFocusGameDialogue = null;

        // List of all the window that are opened!
        private LinkedList<JCS_DialogueObject> mOpenWindow = null;

        [Header("** General Screen Settings (JCS_UIManager) **")]

        [Tooltip("Panel that could do the fade loose focus effect.")]
        [SerializeField]
        private JCS_FadeScreen mFadeScreen = null;


        //----------------------
        // Protected Variables

        //========================================
        //      setter / getter
        //------------------------------
        public void SetJCSDialogue(JCS_DialogueType type, JCS_DialogueObject jdo)
        {
            switch (type)
            {
                case JCS_DialogueType.GAME_UI:
                    {
                        if (mGameUI != null)
                        {
                            //JCS_Debug.LogError("Failed to set \"In Game Dialogue\"...");
                            return;
                        }

                        this.mGameUI = jdo;
                    }
                    break;
                case JCS_DialogueType.PLAYER_DIALOGUE:
                    {
                        //if (mFocusGameDialogue != null)
                        //{
                        //    JCS_Debug.LogError("JCS_UIManager",   "Failed to set \"In Game Dialogue\"...");
                        //    return;
                        //}

                        this.mFocusGameDialogue = jdo;
                    }
                    break;
                case JCS_DialogueType.SYSTEM_DIALOGUE:
                    {
                        if (mForceDialogue != null)
                        {
                            //JCS_Debug.LogError("JCS_UIManager",   "Failed to set \"Force Dialogue\"...");
                            return;
                        }

                        mForceDialogue = jdo;
                    }
                    break;
            }
        }
        public JCS_DialogueObject GetJCSDialogue(JCS_DialogueType type)
        {
            switch (type)
            {
                // Game UI
                case JCS_DialogueType.GAME_UI:
                    return this.mGameUI;

                // Dialogue Box
                case JCS_DialogueType.PLAYER_DIALOGUE:
                    return this.mFocusGameDialogue;
                case JCS_DialogueType.SYSTEM_DIALOGUE:
                    return this.mForceDialogue;
            }

            JCS_Debug.LogError("Failed to get Dialogue -> " + type);
            return null;
        }
        public LinkedList<JCS_DialogueObject> GetOpenWindow() { return this.mOpenWindow; }
        public JCS_FadeScreen FadeScreen { get { return this.mFadeScreen; } set { this.mFadeScreen = value; } }
        public JCS_UndoRedoSystem GetGlobalUndoRedoSystem() { return this.mGlobalUndoRedoSystem; }

        //========================================
        //      Unity's function
        //------------------------------
        private void Awake()
        {
            instance = this;

            this.mOpenWindow = new LinkedList<JCS_DialogueObject>();

            this.mGlobalUndoRedoSystem = this.gameObject.AddComponent<JCS_UndoRedoSystem>();
        }
        private void Start()
        {
            // pop the fade screen.
            string path = JCS_GameSettings.FADE_SCREEN_PATH;
            this.mFadeScreen = JCS_Utility.SpawnGameObject(path).GetComponent<JCS_FadeScreen>();
        }

        private void Update()
        {
#if (UNITY_EDITOR)
            //Test();
#endif


#if (UNITY_EDITOR || UNITY_STANDALONE)

            // Exit in game diagloue not in game UI!!
            // (Admin input)
            if (Input.GetKeyDown(KeyCode.Escape))
                JCS_UtilityFunctions.DestoryCurrentDialogue(JCS_DialogueType.PLAYER_DIALOGUE);
#endif
        }

#if (UNITY_EDITOR)
        private void Test()
        {
            //if (JCS_Input.GetKeyDown(KeyCode.A))
            //    JCS_UtilityFunctions.PopIsConnectDialogue();
            //if (JCS_Input.GetKeyDown(KeyCode.S))
            //    JCS_UtilityFunctions.PopSettingDialogue();
            //if (JCS_Input.GetKeyDown(KeyCode.D))
            //    JCS_UtilityFunctions.PopInGameUI();
            //if (JCS_Input.GetKeyDown(KeyCode.F))
            //    JCS_UtilityFunctions.PopTalkDialogue();

            if (JCS_Input.GetKeyDown(KeyCode.A))
            {
                Color col = Color.red;
                col.a = 0.6f;

                Focus(col);
            }

            if (JCS_Input.GetKeyDown(KeyCode.S))
            {
                UnFocus();
            }
        }
#endif

        //========================================
        //      Self-Define
        //------------------------------
        //----------------------
        // Public Functions

        /// <summary>
        /// Hide the last open dialogue in the current scene.
        /// </summary>
        public void HideTheLastOpenDialogue()
        {
            // return if nothing in the list
            if (GetOpenWindow().Count == 0)
                return;

            JCS_DialogueObject jdo = GetOpenWindow().Last.Value;

            // once it hide it will remove from the list it self!
            jdo.HideDialogue();
        }

        /// <summary>
        /// Hide all the open dialgoue in the current scene.
        /// </summary>
        public void HideAllOpenDialogue()
        {
            // return if nothing in the list
            if (GetOpenWindow().Count == 0)
                return;

            while (GetOpenWindow().Count != 0)
            {
                GetOpenWindow().First.Value.HideDialogueWithoutSound();
            }

        }

        /// <summary>
        /// This will run the default value by the inspector's setting.
        /// </summary>
        /// <param name="time"> time to fade. </param>
        public void Focus()
        {
            JCS_FadeObject fadeObj = this.mFadeScreen.FadeObject;
            fadeObj.FadeIn();
        }

        /// <summary>
        /// This will run the default value by the inspector's setting.
        /// </summary>
        /// <param name="time"> time to fade. </param>
        public void Focus(float time)
        {
            JCS_FadeObject fadeObj = this.mFadeScreen.FadeObject;
            fadeObj.FadeIn(time);
        }

        /// <summary>
        /// This will run the default value by the inspector's setting.
        /// </summary>
        /// <param name="time"> time to fade. </param>
        public void Focus(Color color)
        {
            float alpha = color.a;

            JCS_FadeObject fadeObj = this.mFadeScreen.FadeObject;

            Color fakeColor = color;
            fakeColor.a = 0;

            fadeObj.LocalColor = fakeColor;
            fadeObj.FadeInAmount = alpha;
            fadeObj.FadeIn();
        }

        /// <summary>
        /// Fade a screen to cetain amount of value.
        /// </summary>
        /// <param name="time"></param>
        /// <param name="color"></param>
        public void Focus(float time, Color color)
        {
            float alpha = color.a;

            JCS_FadeObject fadeObj = this.mFadeScreen.FadeObject;

            Color fakeColor = color;
            fakeColor.a = 0;

            fadeObj.LocalColor = fakeColor;
            fadeObj.FadeInAmount = alpha;
            fadeObj.FadeIn(time);
        }

        /// <summary>
        /// Fade out the screen, back to original amount of value.
        /// </summary>
        public void UnFocus()
        {
            JCS_FadeObject fadeObj = this.mFadeScreen.FadeObject;
            fadeObj.FadeOut();
        }

        /// <summary>
        /// Fade out the screen, back to original amount of value.
        /// </summary>
        public void UnFocus(float time)
        {
            JCS_FadeObject fadeObj = this.mFadeScreen.FadeObject;
            fadeObj.FadeOut(time);
        }

        //----------------------
        // Protected Functions

        //----------------------
        // Private Functions

    }
}
