using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

namespace HOME.Data {
    public class JSONSaver {

        private static readonly string _fileName = "saveData";
        private static readonly string _fileExtension = ".sav";
        private static int _currentlyActiveSavenumber = 0; // default to 0
        public static int CurrentlyActiveSavenumber { get => _currentlyActiveSavenumber; }
        private static int _selectetSaveNumber = 0;
        public static int SelectetSaveNumber { get => _selectetSaveNumber; set { _selectetSaveNumber = value; } }


        public static string GetSaveFilePathFromDisk(int saveNumber) { // gets file that we want to load!
            return Application.persistentDataPath + "/" + _fileName + saveNumber.ToString() + _fileExtension;
        }

        public void Save(SaveData data) { // save data we pass in
            string jsonGameDataString = JsonUtility.ToJson(data); // write to string
            string saveFilename = GetSaveFilePathFromDisk(_selectetSaveNumber);

            FileStream filestream = new FileStream(saveFilename, FileMode.Create); // creates empty file on disk

            using (StreamWriter writer = new StreamWriter(filestream)) { // use stream writer to open write and close file
                writer.Write(jsonGameDataString); // pass game data to writer
            }
            _currentlyActiveSavenumber = _selectetSaveNumber; // set curr Active Save
        }

        public bool Load(SaveData data) { // bool for was loading succesfull
            string loadFilename = GetSaveFilePathFromDisk(_selectetSaveNumber); // get name
            if (File.Exists(loadFilename)) { // if file exsists
                using (StreamReader reader = new StreamReader(loadFilename)) { // use stream reader for open read close
                    string json = reader.ReadToEnd(); // convert JSON file to string
                    JsonUtility.FromJsonOverwrite(json, data); // replace values with exsisting saveFile
                }
                _currentlyActiveSavenumber = _selectetSaveNumber; // set curr Active Save
                return true; // we loaded!
            }
            return false; // no valid file found
        }

        public void DeleteData() {
            File.Delete(GetSaveFilePathFromDisk(_selectetSaveNumber));
        }

    }
}