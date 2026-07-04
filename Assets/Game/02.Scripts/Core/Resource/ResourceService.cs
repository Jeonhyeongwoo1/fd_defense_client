namespace Game.Core
{
    public class ResourceService
    {
        public T Load<T>(string path) where T : UnityEngine.Object
        {
            var resource = UnityEngine.Resources.Load<T>(path);

            if (resource == null)
            {
                GameLogger.LogError($"[ResourceService] Failed to load: {path}");
            }

            return resource;
        }
    }
}
