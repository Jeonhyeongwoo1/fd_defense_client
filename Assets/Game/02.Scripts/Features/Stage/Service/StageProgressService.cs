using Game.Core;
using UnityEngine;

namespace Game.Service
{
    public class StageProgressService
    {
        private const string SelectedStageIdKey = "SelectedStageId";
        private const string StageClearedPrefix = "StageCleared_";

        public string GetSelectedStageId()
        {
            var stageId = PlayerPrefs.GetString(SelectedStageIdKey, string.Empty);
            if (string.IsNullOrEmpty(stageId))
            {
                return Const.DefaultStageId;
            }
            return stageId;
        }

        public void SetSelectedStageId(string stageId)
        {
            PlayerPrefs.SetString(SelectedStageIdKey, stageId);
            PlayerPrefs.Save();
        }

        public bool IsStageCleared(string stageId)
        {
            var key = StageClearedPrefix + stageId;
            return PlayerPrefs.GetInt(key, 0) == 1;
        }

        public void MarkStageCleared(string stageId)
        {
            var key = StageClearedPrefix + stageId;
            PlayerPrefs.SetInt(key, 1);
            PlayerPrefs.Save();
        }
    }
}
