using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace YG
{
    [System.Serializable]
    public class SavesYG
    {
        public int IDSave;
        public bool IsFirstSession = true;
        public string Language = "ru";
        public bool PromptDone;

        public int Coins;
        public int BestScores;
        public List<ushort> PurchasedAnimals = new List<ushort>();

        public long RewardCollectedAt;

        public int RewardCount;
        public ushort SelectedAnimalID;
        public SaveAudio Audio = new SaveAudio(musicIsOn: true, soundIsOn: true);

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