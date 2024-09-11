
using System.Collections.Generic;

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
        public int Scores;
        public List<ushort> PurchasedAnimals = new List<ushort>();
        
        public long RewardCollectedAt;
        
        public int RewardCount;
        public ushort SelectedAnimalID;
    }
}
