namespace MultiLaunchpadMod
{
    [System.Serializable]
    public class UserSettings
    {
        public bool debug = false;
        public UnityEngine.Vector2Int windowPosition = new UnityEngine.Vector2Int( -850, -500 );
    }

    public static class SettingsManager
    {
        public static readonly SFS.IO.FilePath Path = MultiLaunchpadMod.Main.modFolder.ExtendToFile("settings.txt");
        public static UserSettings settings;

        public static void Load()
        {
            if (!SFS.Parsers.Json.JsonWrapper.TryLoadJson(Path, out settings) && Path.FileExists())
            {
                UnityEngine.Debug.Log("Multi Launchpad: Settings file invalid or not found, using defaults.");
            }

            settings = settings ?? new UserSettings();
            Save();
        }

        public static void Save()
        {
            if (settings == null)
                Load();
            Path.WriteText(SFS.Parsers.Json.JsonWrapper.ToJson(settings, true));
        }
    }
}
