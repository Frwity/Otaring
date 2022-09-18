using Com.RandomDudes.CryptoGraphy;
using Com.RandomDudes.Debug;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Com.RandomDudes.Saves
{
    public class SavesManager
    {
        private const string AESKey = "Ze+H8bqe7apcTYz4RGvsKA==";
        private const string saveName = "projectName";

        public static Save CurrentSave { get; private set; }

        private static string persistentDataPath;

        public static void LoadSave()
        {
            persistentDataPath = Application.persistentDataPath;

            CurrentSave = GetSave();
        }

        public static void UnloadSave()
        {
            CurrentSave = null;
        }

        public static void StoreLoadedSave()
        {
            if (CurrentSave == null)
                throw new System.Exception("No save loaded!");

            StoreSave(CurrentSave);
        }

        public static void StoreSave(Save save)
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Create(GetFullSavePath());

            string saveToStore = AES.Cipher(AES.ObjectToBytes(save), AESKey);

            bf.Serialize(file, saveToStore);
            file.Close();

            DevLog.Message("===Save Manager===\nLocal Save Updated!");
        }

        public static Save GetSave()
        {
            Save save;
            string path = GetFullSavePath();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file;

            if (CheckIfSaveExists())
            {
                file = File.Open(path, FileMode.Open);

                try
                {
                    save = AES.DecipherToObject<Save>((string)bf.Deserialize(file), AESKey);

                    file.Close();

                    DevLog.Message("===Save Manager===\nLoaded save successfully!");

                    return save;

                }
                catch
                {
                    file.Close();
                    File.Delete(path);

                    DevLog.Warning("===Save Manager===\nSave was corrupted!");

                    return GetSave();
                }
            }
            else
            {
                DevLog.Warning("===Save Manager===\nResquested save not found on local storage! Creating new one");

                save = new Save();
                StoreSave(save);

                return save;
            }
        }

        public static string GetFullSavePath()
        {
            return persistentDataPath + "/" + saveName + ".save";
        }

        public static bool CheckIfSaveExists()
        {
            return File.Exists(GetFullSavePath());
        }

        public static void ResetSave()
        {
            if (CheckIfSaveExists())
            {
                File.Delete(GetFullSavePath());

                LoadSave();
            }
        }
    }
}