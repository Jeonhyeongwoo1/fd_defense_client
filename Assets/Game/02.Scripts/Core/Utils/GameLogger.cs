namespace Game.Core
{
    public static class GameLogger
    {
        public static void Log(string message)
        {
            if (!Const.IsLogEnabled)
            {
                return;
            }

            UnityEngine.Debug.Log(message);
        }

        public static void LogWarning(string message)
        {
            if (!Const.IsLogEnabled)
            {
                return;
            }

            UnityEngine.Debug.LogWarning(message);
        }

        public static void LogError(string message)
        {
            if (!Const.IsLogEnabled)
            {
                return;
            }

            UnityEngine.Debug.LogError(message);
        }
    }
}
