using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace YG
{
    [Serializable]
    public class SavesYG
    {
        public int IDSave;
        public bool IsFirstSession = true;
        public string Language = "ru";
        public bool PromptDone;

        public bool CheckFirstSession() => 
            IDSave == 0;

        [SerializeField]
        private Data _savings = new Data
        {
            Audio = new Data.SaveAudio(musicIsOn: true, soundIsOn: true),
            PurchasedAnimals = new List<ushort>
            {
               //default animal
               0
            }
        };

        public ref Data Savings
        {
            get
            {
//                 if (IsFirstSession)
//                 {
// #if DEBUG
//                     Debug.Log("IsFirstSession");
// #endif
//
//                     _savings = new Data
//                     {
//                         Audio = new Data.SaveAudio(musicIsOn: true, soundIsOn: true),
//                         PurchasedAnimals = new List<ushort>()
//                     };
//                 }

                return ref _savings;
            }
        }

        [Serializable]
        public struct Data
        {
            public int Coins;
            public int BestScores;
            public List<ushort> PurchasedAnimals;

            [FormerlySerializedAs("RewardCollectedAt")]
            public long RewardCollectionTime;

            public int RewardCount;
            public ushort SelectedAnimalID;
            public SaveAudio Audio;

            [Serializable]
            public struct SaveAudio
            {
                public bool MusicIsOn;
                public bool SoundIsOn;

                public SaveAudio(bool musicIsOn, bool soundIsOn)
                {
                    MusicIsOn = musicIsOn;
                    SoundIsOn = soundIsOn;
                }
            }
        }
    }
}