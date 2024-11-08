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
            private static System.Collections.Generic.List<string> _spaceCenterAddresses =
                new  System.Collections.Generic.List<string>();
            private static int _spaceCenterIndex=0;
            private static SFS.UI.ModGUI.Label _planetName_Label;
        #endregion

        #region "Private methods"
            private static void Next_Button_Click()
            {
                if (++_spaceCenterIndex>=_spaceCenterAddresses.Count)  _spaceCenterIndex=0;
                _planetName_Label.Text= _spaceCenterAddresses[_spaceCenterIndex];
                SwitchTo();
            }

            private static void Previous_Button_Click()
            {
                if (_spaceCenterIndex-- <=0)  _spaceCenterIndex=_spaceCenterAddresses.Count-1;
                _planetName_Label.Text= _spaceCenterAddresses[_spaceCenterIndex];
                SwitchTo();
            }

            private static void SwitchTo()
            {
                string planetName = _planetName_Label.Text;

                if (!SFS.Base.planetLoader.planets.ContainsKey(planetName))
                {
                    SFS.UI.MsgDrawer.main.Log(string.Format("Planet \"{0}\" Not Found", planetName));
                }
                else
                {
                    MultiLaunchpadMod.SpaceCenterData selectedSpaceCenter =  MultiLaunchpadMod.SpaceCenterData.alternates[planetName];
                    SFS.Base.planetLoader.spaceCenter.address = selectedSpaceCenter.address;
                    SFS.Base.planetLoader.spaceCenter.angle = selectedSpaceCenter.angle;
                    SFS.Base.planetLoader.spaceCenter.position_LaunchPad.horizontalPosition = selectedSpaceCenter.position_LaunchPad.horizontalPosition;
                    SFS.Base.planetLoader.spaceCenter.position_LaunchPad.height = selectedSpaceCenter.position_LaunchPad.height;
                    SFS.Base.sceneLoader.LoadHubScene();
                }
            }

        #endregion

        #region "Public properties"
            /// <summary>True if debugging mode is on</summary>
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

            /// <summary>True if the GUI is currently active</summary>
            public static bool IsActive
            { get { return _isActive;}}

        #endregion

        #region "Public methods"
            /// <summary>(re-)Load the list of planets with space centers</summary>
            public static void LoadPlanetsList()
            {
                if (_isActive)
                {
                    string tracePoint="D-01";
                    try
                    {
                        // determine the list of available space center names and the index corresponding to the current one
                        System.Collections.Generic.HashSet<string> completeChallenges = null;
                        _spaceCenterIndex=0;
                        _spaceCenterAddresses.Clear();

                        if
                            (
                                SFS.Base.worldBase!=null
                                && SFS.Base.worldBase.paths!=null
                                && SFS.Base.worldBase.paths.worldPersistentPath!=null
                            )
                        {
                            SFS.IO.FilePath filePath = SFS.Base.worldBase.paths.worldPersistentPath.ExtendToFile("Challenges.txt");
                            System.Collections.Generic.List<string> completeChallenges_List = new System.Collections.Generic.List<string>();
                            if
                                (
                                    !filePath.FileExists()
                                    || !SFS.Parsers.Json.JsonWrapper.TryLoadJson<System.Collections.Generic.List<string>>(filePath, out completeChallenges_List)
                                )
                            {
                                UnityEngine.Debug.LogError("[MultiLaunchpadMod.UI.LoadPlanetsList] Failed to load Challenges.txt");
                            }
                            else
                            {
                                completeChallenges = completeChallenges_List.ToHashSet();
                            }
                        }
                        else
                        {
                            UnityEngine.Debug.LogError("[MultiLaunchpadMod.UI.LoadPlanetsList] Failed to determine SFS.Base.worldBase.paths.worldPersistentPath");
                        }

                        foreach (string oneSpaceCenterAddress in MultiLaunchpadMod.SpaceCenterData.alternates.Keys)
                        {
                            tracePoint="D-02";
                            if
                                (
                                    MultiLaunchpadMod.SpaceCenterData.alternates.ContainsKey(oneSpaceCenterAddress)
                                    && MultiLaunchpadMod.SpaceCenterData.alternates[oneSpaceCenterAddress].enabled>=1
                                )
                            {
                                tracePoint="D-03";
                                bool isEnabled=false;

                                switch (MultiLaunchpadMod.SpaceCenterData.alternates[oneSpaceCenterAddress].enabled)
                                {
                                    case 1:
                                        isEnabled=true;
                                    break;

                                    case 2:
                                    {
                                        if (completeChallenges != null)
                                        {
                                            isEnabled=completeChallenges.Contains("Land_" + oneSpaceCenterAddress);
                                        }
                                        else
                                        {
                                            isEnabled=false;
                                        }
                                    }
                                    break;

                                    default:
                                        isEnabled=false;
                                    break;
                                }
                                if (isEnabled)
                                {
                                    _spaceCenterAddresses.Add(oneSpaceCenterAddress);
                                    if (oneSpaceCenterAddress==SFS.Base.planetLoader.spaceCenter.address) _spaceCenterIndex=_spaceCenterAddresses.Count-1;
                                }
                            }
                        }
                        _planetName_Label.Text=SFS.Base.planetLoader.spaceCenter.address;
                    }
                    catch (System.Exception excp)
                    {
                        UnityEngine.Debug.LogErrorFormat("[MultiLaunchpadMod.UI.LoadPlanetsList-{0}] {1}", tracePoint ,excp.ToString());
                    }
                }
            }

            /// <summary>Show the GUI</summary>
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

                SFS.UI.ModGUI.Builder.CreateButton(window,30,30,0,0,Previous_Button_Click,"<");
                _planetName_Label = SFS.UI.ModGUI.Builder.CreateLabel(window, 270,30,0,0,SFS.Base.planetLoader.spaceCenter.address);
                SFS.UI.ModGUI.Builder.CreateButton(window,30,30,0,0,Next_Button_Click,">");

                _isActive = true;
                LoadPlanetsList();
            }

            /// <summary>Note that the GIU is no longer active</summary>
            public static void GUIInActive()
            {
                _isActive = false;
            }
        #endregion
    }
}
