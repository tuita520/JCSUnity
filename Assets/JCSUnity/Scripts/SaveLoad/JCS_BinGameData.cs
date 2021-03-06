/**
 * $File: JCS_BinGameData.cs $
 * $Date: 2018-06-27 11:03:19 $
 * $Revision: $
 * $Creator: Jen-Chieh Shen $
 * $Notice: See LICENSE.txt for modification and distribution information 
 *	                 Copyright © 2018 by Shen, Jen-Chieh $
 */
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace JCSUnity
{
    /// <summary>
    /// Interface of storing game data as binary format.
    /// </summary>
    [System.Serializable]
    public class JCS_BinGameData
    {
        public string Copyright = "";
        public string Version = "";

        private void InitJCSFile()
        {
            if (JCS_PackageDataSettings.instance == null)
            {
#if (UNITY_EDITOR)
                JCS_Debug.LogError(
                    "Failed to load the copyright and version information text...");
#endif 
                return;
            }

            Copyright = JCS_PackageDataSettings.instance.CopyrightString;
            Version = JCS_PackageDataSettings.instance.VersionString;
        }

        /// <summary>
        /// Save the game data into binary file format.
        /// </summary>
        /// <typeparam name="T"> type of the data save. </typeparam>
        /// <param name="filePath"> where to save. </param>
        /// <param name="fileName"> name of the file u want to save. </param>
        public void Save<T>(string filePath, string fileName)
        {
            string fullFilePath = filePath + fileName;

            Save<T>(fullFilePath);
        }

        /// <summary>
        /// Save the game data into binary file format.
        /// </summary>
        /// <typeparam name="T"> type of the data save. </typeparam>
        /// <param name="fullFilePath"> file path direct where to save. </param>
        public void Save<T>(string fullFilePath)
        {
            string filePath = Path.GetDirectoryName(fullFilePath);

            // if Directory does not exits, create it prevent error!
            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            InitJCSFile();

            using (var stream = new FileStream(fullFilePath, FileMode.Create))
            {
                var binFmt = new BinaryFormatter();
                binFmt.Serialize(stream, this);
            }
        }

        /// <summary>
        /// Load the game data from a directory file path.
        /// </summary>
        /// <typeparam name="T"> type of the game data u want to load. </typeparam>
        /// <param name="filePath"> file directory, location, path. </param>
        /// <param name="fileName"> name of the file u want to load. </param>
        /// <returns>
        /// Full game data. If the file does not exists returns 
        /// null references.
        /// </returns>
        public static T LoadFromFile<T>(string filePath, string fileName)
        {
            string fullFilePath = filePath + fileName;

            return LoadFromFile<T>(fullFilePath);
        }

        /// <summary>
        /// Load the game data from a directory file path.
        /// </summary>
        /// <typeparam name="T"> type of the game data u want to load. </typeparam>
        /// <param name="filePath"> file directory, location, path. </param>
        /// <param name="fileName"> name of the file u want to load. </param>
        /// <returns>
        /// Full game data. If the file does not exists returns 
        /// null references.
        /// </returns>
        public static T LoadFromFile<T>(string fullFilePath)
        {
            if (!File.Exists(fullFilePath))
                return default(T);

            using (var stream = new FileStream(fullFilePath, FileMode.Open))
            {
                var binFmt = new BinaryFormatter();
                return (T)binFmt.Deserialize(stream);
            }
        }
    }
}
