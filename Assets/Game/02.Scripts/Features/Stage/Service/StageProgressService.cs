using Game.Core;

namespace Game.Service
{
    public class StageProgressService
    {
        private readonly UserDataService _userDataService;

        public StageProgressService(UserDataService userDataService)
        {
            _userDataService = userDataService;
        }

        public string GetSelectedStageId()
        {
            var stageId = _userDataService.Data.selectedStageId;
            if (string.IsNullOrEmpty(stageId))
            {
                return Const.DefaultStageId;
            }
            return stageId;
        }

        public void SetSelectedStageId(string stageId)
        {
            _userDataService.Data.selectedStageId = stageId;
            _userDataService.Save();
        }

        public bool IsStageCleared(string stageId)
        {
            return _userDataService.IsStageCleared(stageId);
        }

        public void MarkStageCleared(string stageId)
        {
            _userDataService.MarkStageCleared(stageId);
            _userDataService.Save();
        }
    }
}
