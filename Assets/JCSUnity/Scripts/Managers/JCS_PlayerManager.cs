﻿/**
 * $File: JCS_PlayerManager.cs $
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
    /// Take care of all the player in the scene.
    /// </summary>
    public class JCS_PlayerManager 
        : MonoBehaviour
    {

        //----------------------
        // Public Variables
        public static JCS_PlayerManager instance = null;

        //----------------------
        // Private Variables

        [Header("** Check Variables (JCS_PlayerManager) **")]

        [Tooltip("current player that are active")]
        [SerializeField]
        private JCS_Player mActivePlayer = null;

        [Tooltip("List of all the player in the game.")]
        [SerializeField]
        private List<JCS_Player> mPlayers = null;


        //----------------------
        // Protected Variables

        //========================================
        //      setter / getter
        //------------------------------
        public JCS_Player GetActivePlayer() { return this.mActivePlayer; }
        public List<JCS_Player> GetJCSPlayerList() { return this.mPlayers; }
        public JCS_Player GetJCSPlayerAt(int index) { return this.mPlayers[index]; }

        //========================================
        //      Unity's function
        //------------------------------
        private void Awake()
        {
            instance = this;
            mPlayers = new List<JCS_Player>();
        }

        private void Start()
        {
            DoIgnorePlayersToEachOthers();

            // if the game only allow one play do the function
            // in order to take the effect.
            if (JCS_GameSettings.instance.ACTIVE_ONE_PLAYER)
                ActiveOnePlayer(JCS_GameManager.instance.GetJCSPlayer());
        }

#if (UNITY_EDITOR)
        private void Update()
        {
            if (JCS_GameSettings.instance.GAME_TYPE == JCS_GameType.GAME_2D)
                PlayerManageTest();
        }
#endif

#if (UNITY_EDITOR)
        private void PlayerManageTest()
        {
            //if (JCS_Input.GetKeyDown(KeyCode.L))
            //{
            //    JCS_2DCamera cam = (JCS_2DCamera)JCS_Camera.main;
            //    cam.SetFollowTarget(mPlayers[0].transform);
            //    JCS_GameManager.instance.SetJCSPlayer(mPlayers[0]);
            //    ActiveOnePlayer(0);
            //}
            //if (JCS_Input.GetKeyDown(KeyCode.K))
            //{
            //    JCS_2DCamera cam = (JCS_2DCamera)JCS_Camera.main;
            //    cam.SetFollowTarget(mPlayers[1].transform);
            //    JCS_GameManager.instance.SetJCSPlayer(mPlayers[1]);
            //    ActiveOnePlayer(1);
            //}
        }

#endif

        //========================================
        //      Self-Define
        //------------------------------
        //----------------------
        // Public Functions
        
        /// <summary>
        /// Add the player to the list, in order to get manage 
        /// by this manager.
        /// </summary>
        /// <param name="player"> Player to add to the list. </param>
        public void AddPlayerToManage(JCS_Player player)
        {
            if (player == null)
                return;

            mPlayers.Add(player);
        }

        /// <summary>
        /// This player will be in the active player slot.
        /// </summary>
        /// <param name="index"> index in the player list. </param>
        public void ActiveOnePlayer(int index)
        {
            ActiveOnePlayer(mPlayers[index]);
        }
        /// <summary>
        /// This player will be in the active player slot.
        /// </summary>
        /// <param name="player"> player to be active. </param>
        public void ActiveOnePlayer(JCS_Player player)
        {
            foreach (JCS_Player p in mPlayers)
            {
                if (p == player)
                {
                    p.ControlEnable(true);
                    mActivePlayer = p;
                }
                else
                    p.ControlEnable(false);
            }
        }

        /// <summary>
        /// Ignore the player's physcis to each other player?
        /// </summary>
        public void DoIgnorePlayersToEachOthers()
        {
            if (JCS_GameSettings.instance.PLAYER_IGNORE_EACH_OTHER)
            {
                // Make all the player ignore each other
                for (int index = 0;
                    index < mPlayers.Count;
                    ++index)
                {
                    for (int pairIndex = index + 1;
                        pairIndex < mPlayers.Count;
                        ++pairIndex)
                    {

                        Physics.IgnoreCollision(
                                mPlayers[index].GetCharacterController(),
                                mPlayers[pairIndex].GetCharacterController(), true);
                    }
                }
            }
        }

        /// <summary>
        /// Ignore a collider with all the player in the list.
        /// </summary>
        /// <param name="cc"> Collider to ignore. </param>
        /// <param name="act"> 
        /// true : ignore it.
        /// false : don't ignore it.
        /// </param>
        public void IgnorePhysicsToAllPlayer(Collider cc, bool act = true)
        {
            // Make all the player ignore each other
            for (int index = 0;
                index < mPlayers.Count;
                ++index)
            {
                Physics.IgnoreCollision(
                            mPlayers[index].GetCharacterController(), 
                            cc, act);
            }
        }

        /// <summary>
        /// Comparing to the transform see if the same
        /// transform as player did. (typeid method)
        /// </summary>
        /// <param name="trans"></param>
        /// <returns>
        /// true : is one of the player's transfrom.
        /// false : vice versa.
        /// </returns>
        public bool IsPlayerTransform(Transform trans)
        {
            if (trans == null)
                return false;

            foreach (JCS_Player player in mPlayers)
            {
                if (trans == player.transform)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Check weather the transfrom is active player's transform.
        /// </summary>
        /// <param name="tran"> Transform to test. </param>
        /// <returns>
        /// true : is active player's transform.
        /// false : vice versa.
        /// </returns>
        public bool IsActivePlayerTransform(Transform tran)
        {
            if (mActivePlayer != null)
            {
                if (tran == mActivePlayer.transform)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Ignore physics to all the active player except 
        /// the active player.
        /// </summary>
        /// <param name="cc"> Collider want to ignore with. </param>
        public void IgnorePhysicsToAllExceptActivePlayer(Collider cc)
        {
            // ignore all
            IgnorePhysicsToAllPlayer(cc);

            if (mActivePlayer == null)
                return;

            // active
            Physics.IgnoreCollision(
                            mActivePlayer.GetCharacterController(),
                            cc, true);
        }

        /// <summary>
        /// Return the player on the scene, according to 
        /// the direction.
        /// </summary>
        /// <param name="direction"> position u want to find the player </param>
        /// <returns> player with the position </returns>
        public JCS_Player FindPlayerByDirection(JCS_2D4Direction direction)
        {
            if (GetJCSPlayerList().Count == 0)
            {
                JCS_Debug.LogError(
                    "JCS_PlayerManager", 
                      
                    "Cannot use the current function cuz the player list in the manager is lower than 0...");

                return null;
            }

            Vector3 foundPos = GetJCSPlayerAt(0).transform.position;
            int foundIndex = 0;

            JCS_Player currentPlayer = null;

            switch (direction)
            {
                case JCS_2D4Direction.TOP:
                    {
                        for (int index = 1;
                            index < GetJCSPlayerList().Count;
                            ++index)
                        {
                            currentPlayer = GetJCSPlayerAt(index);

                            if (foundPos.y < currentPlayer.transform.position.y)
                            {
                                // update position if found the highest position
                                foundPos = currentPlayer.transform.position;
                                // update found index.
                                foundIndex = index;
                            }
                        }
                    }
                    break;
                case JCS_2D4Direction.BOTTOM:
                    {
                        for (int index = 1;
                            index < GetJCSPlayerList().Count;
                            ++index)
                        {
                            currentPlayer = GetJCSPlayerAt(index);

                            if (foundPos.y > currentPlayer.transform.position.y)
                            {
                                // update position if found the highest position
                                foundPos = currentPlayer.transform.position;
                                // update found index.
                                foundIndex = index;
                            }
                        }
                    }
                    break;
                case JCS_2D4Direction.RIGHT:
                    {
                        for (int index = 1;
                            index < GetJCSPlayerList().Count;
                            ++index)
                        {
                            currentPlayer = GetJCSPlayerAt(index);

                            if (foundPos.x < currentPlayer.transform.position.x)
                            {
                                // update position if found the highest position
                                foundPos = currentPlayer.transform.position;
                                // update found index.
                                foundIndex = index;
                            }
                        }
                    }
                    break;
                case JCS_2D4Direction.LEFT:
                    {
                        for (int index = 1;
                            index < GetJCSPlayerList().Count;
                            ++index)
                        {
                            currentPlayer = GetJCSPlayerAt(index);

                            if (foundPos.x > currentPlayer.transform.position.x)
                            {
                                // update position if found the highest position
                                foundPos = currentPlayer.transform.position;
                                // update found index.
                                foundIndex = index;
                            }
                        }
                    }
                    break;
            }

            return GetJCSPlayerAt(foundIndex);
        }

        /// <summary>
        /// Finding Player but using the list 
        /// instead of the list of player
        /// current manager handled.
        /// </summary>
        /// <param name="direction"> direction u want to search </param>
        /// <param name="players"> player u want to search </param>
        /// <returns> player with that direction. </returns>
        public JCS_Player FindPlayerByDirectionUsingList(JCS_2D4Direction direction, List<JCS_Player> players)
        {

            if (players.Count == 0)
            {
                JCS_Debug.LogError(
                    "JCS_PlayerManager",
                     
                    "Cannot use the current function cuz the player list in the manager is lower than 0...");

                return null;
            }

            Vector3 foundPos = players[0].transform.position;
            int foundIndex = 0;

            JCS_Player currentPlayer = null;

            switch (direction)
            {
                case JCS_2D4Direction.TOP:
                    {
                        for (int index = 1;
                            index < players.Count;
                            ++index)
                        {
                            currentPlayer = players[index];

                            if (foundPos.y < currentPlayer.transform.position.y)
                            {
                                // update position if found the highest position
                                foundPos = currentPlayer.transform.position;
                                // update found index.
                                foundIndex = index;
                            }
                        }
                    }
                    break;
                case JCS_2D4Direction.BOTTOM:
                    {
                        for (int index = 1;
                            index < players.Count;
                            ++index)
                        {
                            currentPlayer = players[index];

                            if (foundPos.y > currentPlayer.transform.position.y)
                            {
                                // update position if found the highest position
                                foundPos = currentPlayer.transform.position;
                                // update found index.
                                foundIndex = index;
                            }
                        }
                    }
                    break;
                case JCS_2D4Direction.RIGHT:
                    {
                        for (int index = 1;
                            index < players.Count;
                            ++index)
                        {
                            currentPlayer = players[index];

                            if (foundPos.x < currentPlayer.transform.position.x)
                            {
                                // update position if found the highest position
                                foundPos = currentPlayer.transform.position;
                                // update found index.
                                foundIndex = index;
                            }
                        }
                    }
                    break;
                case JCS_2D4Direction.LEFT:
                    {
                        for (int index = 1;
                            index < players.Count;
                            ++index)
                        {
                            currentPlayer = players[index];

                            if (foundPos.x > currentPlayer.transform.position.x)
                            {
                                // update position if found the highest position
                                foundPos = currentPlayer.transform.position;
                                // update found index.
                                foundIndex = index;
                            }
                        }
                    }
                    break;
            }

            return players[foundIndex];
        }

        /// <summary>
        /// Add all the player to multi track list.
        /// </summary>
        public void AddAllPlayerToMultiTrack()
        {
            // find the object in the scene.
            JCS_2DMultiTrackCamera jcs2dmtc = (JCS_2DMultiTrackCamera)FindObjectOfType(typeof(JCS_2DMultiTrackCamera));

            foreach (JCS_Player p in mPlayers)
            {
                jcs2dmtc.AddTargetToTrackList(p);
            }
        }

        /// <summary>
        /// Remove all the player from multi track list.
        /// </summary>
        public void RemoveAllPlayerFromMultiTrack()
        {
            // find the object in the scene.
            JCS_2DMultiTrackCamera jcs2dmtc = (JCS_2DMultiTrackCamera)FindObjectOfType(typeof(JCS_2DMultiTrackCamera));

            foreach (JCS_Player p in mPlayers)
            {
                jcs2dmtc.RemoveTargetFromTrackList(p);
            }
        }

        //----------------------
        // Protected Functions

        //----------------------
        // Private Functions


    }
}
