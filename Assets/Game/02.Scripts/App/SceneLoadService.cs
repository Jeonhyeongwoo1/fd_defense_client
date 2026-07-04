using Game.Core;
using UnityEngine.SceneManagement;

namespace Game.App
{
    public class SceneLoadService
    {
        public void LoadGameScene()
        {
            SceneManager.LoadScene(Const.GameSceneName);
        }

        public void LoadOutGameScene()
        {
            SceneManager.LoadScene(Const.OutGameSceneName);
        }
    }
}
