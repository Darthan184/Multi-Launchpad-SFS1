using System.Linq; // contains extensions

namespace MultiLaunchpadMod
{
    public class UI
    {
        #region "Private classes"
        #endregion

        #region "Private fields"
            // Create a GameObject for your window to attach to.
            private static UnityEngine.GameObject windowHolder;

            // Random window ID to avoid conflicts with other mods.
            private static readonly int MainWindowID = SFS.UI.ModGUI.Builder.GetRandomID();

            private static bool _debug=false;
            private static bool _isActive=false;
            private static SFS.UI.ModGUI.TextInput _planetName_TextInput;
        #endregion

        #region "Private methods"
            public static void SwitchTo_Button_Click()
            {
                string planetName = PlanetName;

                if (!SFS.Base.planetLoader.planets.ContainsKey(planetName))
                {
                    SFS.UI.MsgDrawer.main.Log(string.Format("\"{0}\" Not Found", planetName));
                }
                else
                {
                    SFS.Base.planetLoader.spaceCenter.address = planetName;
//~                     typeof(SFS.World.SpaceCenter).GetMethod("Start", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(SFS.Base.planetLoader.spaceCenter, null);
                    SFS.Base.sceneLoader.LoadHubScene();
                }
            }

        #endregion

        #region "Public properties"
            public static bool Debug
            {
                get
                {
                    return _debug;
                }
                set
                {
                    _debug=value;
                   SettingsManager.settings.debug = _debug;
                    MultiLaunchpadMod.SettingsManager.Save();
                }
            }

            public static string PlanetName
            {
                get
                {
                    return _planetName_TextInput.Text;
                }
                set
                {
                    _planetName_TextInput.Text=value;
                }
            }
            public static bool IsActive
            { get { return _isActive;}}

        #endregion

        #region "Public methods"
            public static void ShowGUI()
            {
                _isActive = false;
                // Create the window holder, attach it to the currently active scene so it's removed when the scene changes.
                windowHolder = SFS.UI.ModGUI.Builder.CreateHolder(SFS.UI.ModGUI.Builder.SceneToAttach.CurrentScene, "MultiLaunchpadMod GUI Holder");
                UnityEngine.Vector2Int pos = SettingsManager.settings.windowPosition;
                _debug =  SettingsManager.settings.debug;
                SFS.UI.ModGUI.Window window = SFS.UI.ModGUI.Builder.CreateWindow(windowHolder.transform, MainWindowID, 360, 100, pos.x, pos.y, true, true, 0.95f, "Select Launchpad");

                // Create a layout group for the window. This will tell the GUI builder how it should position elements of your UI.
                window.CreateLayoutGroup(SFS.UI.ModGUI.Type.Horizontal, UnityEngine.TextAnchor.MiddleCenter,10f);

                window.gameObject.GetComponent<SFS.UI.DraggableWindowModule>().OnDropAction += () =>
                {
                    MultiLaunchpadMod.SettingsManager.settings.windowPosition = UnityEngine.Vector2Int.RoundToInt(window.Position);
                    MultiLaunchpadMod.SettingsManager.Save();
                };
                _planetName_TextInput = SFS.UI.ModGUI.Builder.CreateTextInput(window, 180,30,0,0,SFS.Base.planetLoader.spaceCenter.address);
                SFS.UI.ModGUI.Builder.CreateButton(window,120,30,0,0,SwitchTo_Button_Click,"Switch To");
                _isActive = true;
            }

            public static void GUIInActive()
            {
                _isActive = false;
            }
        #endregion
    }
}
