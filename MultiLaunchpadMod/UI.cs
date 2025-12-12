using System.Linq; // contains extensions

namespace MultiLaunchpadMod
{
    public class UI
    {
        #region "Private classes"
            private class  _TerrainRange
            {
                public double minTerrain = double.MaxValue;
                public double maxTerrain = double.MinValue;
            }
        #endregion

        #region "Private fields"
            // Create a GameObject for your window to attach to.
            private static UnityEngine.GameObject windowHolder;

            // Random window ID to avoid conflicts with other mods.
            private static readonly int MainWindowID = SFS.UI.ModGUI.Builder.GetRandomID();

            private static bool _debug=false;
            private static bool _isActive=false;

            private static System.Collections.Generic.List<string> _locationNames = null;
            private static System.Collections.Generic.List<string> _planetNames = new System.Collections.Generic.List<string>();

            private static System.Collections.Generic.Dictionary<string,System.Collections.Generic.List<string>> _spaceCenterAddresses =
                new  System.Collections.Generic.Dictionary<string,System.Collections.Generic.List<string>>();

            private static int _locationIndex=0;
            private static SFS.UI.ModGUI.Label _locationName_Label;
            private static SFS.UI.ModGUI.Button _nextLocation_Button;
            private static SFS.UI.ModGUI.Button _previousLocation_Button;

            private static int _planetIndex=0;
            private static SFS.UI.ModGUI.Label _planetName_Label;
        #endregion

        #region "Private methods"
            private static void NextPlanet_Button_Click()
            {
                if (++_planetIndex>=_planetNames.Count)  _planetIndex=0;
                _locationNames=_spaceCenterAddresses[_planetNames[_planetIndex]];
                _planetName_Label.Text= _planetNames[_planetIndex];
                _locationIndex=0;
                _locationName_Label.Text= _locationNames[_locationIndex];
                _nextLocation_Button.Active=(_locationNames.Count>1);
                _previousLocation_Button.Active=(_locationNames.Count>1);
                SwitchTo();
            }

            private static void PreviousPlanet_Button_Click()
            {
                if (_planetIndex-- <=0)  _planetIndex=_planetNames.Count-1;
                _locationNames=_spaceCenterAddresses[_planetNames[_planetIndex]];
                _planetName_Label.Text= _planetNames[_planetIndex];
                _locationIndex=_locationNames.Count-1;
                _locationName_Label.Text= _locationNames[_locationIndex];
                _nextLocation_Button.Active=(_locationNames.Count>1);
                _previousLocation_Button.Active=(_locationNames.Count>1);
                SwitchTo();
            }

            private static void NextLocation_Button_Click()
            {
                if (++_locationIndex>=_locationNames.Count)  _locationIndex=0;
                _locationName_Label.Text= _locationNames[_locationIndex];
                SwitchTo();
            }

            private static void PreviousLocation_Button_Click()
            {
                if (_locationIndex-- <=0)  _locationIndex=_locationNames.Count-1;
                _locationName_Label.Text= _locationNames[_locationIndex];
                SwitchTo();
            }

            private static _TerrainRange GetTerrainRange(SFS.WorldBase.Planet planet, double angleRadians, double hubOffset, double edgeOffset)
            {
                _TerrainRange result = new _TerrainRange();

                for (double angle= angleRadians - edgeOffset; angle< angleRadians + edgeOffset*1.05; angle+=edgeOffset*0.1)
                {
                    double terrain= planet.GetTerrainHeightAtAngle(angle - hubOffset);
                    if (result.minTerrain>terrain) result.minTerrain=terrain;
                    if (result.maxTerrain<terrain) result.maxTerrain=terrain;
                }
                return result;
            }

            private static void SwitchTo()
            {
                string planetName = _planetName_Label.Text;
                string locationName = _locationName_Label.Text;

                if (!SFS.Base.planetLoader.planets.ContainsKey(planetName))
                {
                    SFS.UI.MsgDrawer.main.Log(string.Format("Planet \"{0}\" Not Found", planetName));
                }
                else
                {
                    MultiLaunchpadMod.SpaceCenterData selectedSpaceCenter =  MultiLaunchpadMod.SpaceCenterData.alternates[planetName][locationName];
                    MultiLaunchpadMod.SpaceCenterData.current = selectedSpaceCenter;
                    SFS.Base.planetLoader.spaceCenter.address = selectedSpaceCenter.address;
                    SFS.Base.planetLoader.spaceCenter.position_LaunchPad.horizontalPosition = selectedSpaceCenter.position_LaunchPad.horizontalPosition;

                    if (selectedSpaceCenter.position_LaunchPad.height==null)
                    {
                        SFS.WorldBase.Planet planet = SFS.Base.planetLoader.planets[planetName];
                        double edgeOffset = 50 / planet.Radius;
                        double hubOffset = selectedSpaceCenter.position_LaunchPad.horizontalPosition / planet.Radius;
                        _TerrainRange range=null;

                        if (selectedSpaceCenter.angle==null)
                        {
                            double angleOffset;
                            int counter=0;
                            bool isFound=false;

                            for (angleOffset=0.0;angleOffset<System.Math.PI / 180.0 && counter<1000; angleOffset+=edgeOffset, counter++)
                            {
                                double angleRadians = System.Math.PI/2.0+angleOffset;
                                range=GetTerrainRange(planet, angleRadians, hubOffset, edgeOffset);

                                if (range.maxTerrain-range.minTerrain<8)
                                {
                                    SFS.Base.planetLoader.spaceCenter.angle=angleRadians*180.0/System.Math.PI;
                                    isFound=true;
                                    break;
                                }

                                if (angleOffset!=0.0)
                                {
                                    angleRadians = System.Math.PI/2.0-angleOffset;
                                    range=GetTerrainRange(planet, angleRadians, hubOffset, edgeOffset);

                                    if (range.maxTerrain-range.minTerrain<8)
                                    {
                                        SFS.Base.planetLoader.spaceCenter.angle=angleRadians*180.0/System.Math.PI;
                                        isFound=true;
                                        break;
                                    }
                                }
                            }

                            if (!isFound)
                            {
                                SFS.Base.planetLoader.spaceCenter.angle=90.0;
                                range=GetTerrainRange(planet, System.Math.PI/2.0,hubOffset,edgeOffset);
                            }
                        }
                        else
                        {
                            double angleRadians =  ((double)selectedSpaceCenter.angle*System.Math.PI / 180.0);
                            range=GetTerrainRange(planet, angleRadians, hubOffset, edgeOffset);
                            SFS.Base.planetLoader.spaceCenter.angle = (double)selectedSpaceCenter.angle;
                        }

                        SFS.Base.planetLoader.spaceCenter.position_LaunchPad.height = System.Math.Max(range.minTerrain+8,range.maxTerrain);
                    }
                    else
                    {
                        SFS.Base.planetLoader.spaceCenter.position_LaunchPad.height = (double)selectedSpaceCenter.position_LaunchPad.height;
                    }
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
                        _planetIndex=0;
                        _spaceCenterAddresses.Clear();
                        _planetNames.Clear();

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
                                    filePath.FileExists()
                                    && !SFS.Parsers.Json.JsonWrapper.TryLoadJson<System.Collections.Generic.List<string>>(filePath, out completeChallenges_List)
                                )
                            {
                                UnityEngine.Debug.LogError("[MultiLaunchpadMod.UI.LoadPlanetsList] Invalid Challenges.txt file");
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

                        foreach (string onePlanetName in MultiLaunchpadMod.SpaceCenterData.alternates.Keys)
                        {
                            foreach (string oneLocationName in MultiLaunchpadMod.SpaceCenterData.alternates[onePlanetName].Keys)
                            {
                                tracePoint="D-02";
                                if
                                    (
                                        MultiLaunchpadMod.SpaceCenterData.alternates[onePlanetName][oneLocationName].enabled>=1
                                    )
                                {
                                    tracePoint="D-03";
                                    bool isEnabled=false;
                                    MultiLaunchpadMod.SpaceCenterData spaceCenter = MultiLaunchpadMod.SpaceCenterData.alternates[onePlanetName][oneLocationName];

                                    switch (spaceCenter.enabled)
                                    {
                                        case 1:
                                            isEnabled=true;
                                        break;

                                        case 2:
                                        {
                                            if (completeChallenges == null)
                                            {
                                                isEnabled=false;
                                            }
                                            else if (string.IsNullOrWhiteSpace(spaceCenter.challenge_id))
                                            {
                                                if (onePlanetName=="Venus" || onePlanetName=="Mercury")
                                                {
                                                    isEnabled=completeChallenges.Contains(onePlanetName + "_Landing");
                                                }
                                                else
                                                {
                                                    isEnabled=completeChallenges.Contains("Land_" + onePlanetName);
                                                }
                                            }
                                            else
                                            {
                                                isEnabled=completeChallenges.Contains(spaceCenter.challenge_id);
                                            }
                                        }
                                        break;

                                        default:
                                            isEnabled=false;
                                        break;
                                    }
                                    tracePoint="D-04";

                                    if (isEnabled)
                                    {
                                        if (!_spaceCenterAddresses.ContainsKey(onePlanetName))
                                        {
                                            _spaceCenterAddresses[onePlanetName]=new System.Collections.Generic.List<string>();
                                            _planetNames.Add(onePlanetName);

                                            if (onePlanetName==MultiLaunchpadMod.SpaceCenterData.current.address)
                                            {
                                                _planetIndex=_spaceCenterAddresses.Count-1;
                                                _locationNames= _spaceCenterAddresses[onePlanetName];
                                            }
                                        }
                                        _spaceCenterAddresses[onePlanetName].Add(oneLocationName);
                                    }
                                }
                            }
                        }
                        _locationIndex = _locationNames.IndexOf(MultiLaunchpadMod.SpaceCenterData.current.location);
                        _nextLocation_Button.Active=(_locationNames.Count>1);
                        _previousLocation_Button.Active=(_locationNames.Count>1);
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
                SFS.UI.ModGUI.Window window = SFS.UI.ModGUI.Builder.CreateWindow(windowHolder.transform, MainWindowID, 360, 130, pos.x, pos.y, true, true, 0.95f, "Select Launchpad");

                // Create a layout group for the window. This will tell the GUI builder how it should position elements of your UI.
                window.CreateLayoutGroup(SFS.UI.ModGUI.Type.Vertical, UnityEngine.TextAnchor.MiddleCenter,10f);

                window.gameObject.GetComponent<SFS.UI.DraggableWindowModule>().OnDropAction += () =>
                {
                    MultiLaunchpadMod.SettingsManager.settings.windowPosition = UnityEngine.Vector2Int.RoundToInt(window.Position);
                    MultiLaunchpadMod.SettingsManager.Save();
                };

                SFS.UI.ModGUI.Container planet_Container =  SFS.UI.ModGUI.Builder.CreateContainer(window);
                planet_Container.CreateLayoutGroup(SFS.UI.ModGUI.Type.Horizontal, UnityEngine.TextAnchor.MiddleCenter,10f);
                SFS.UI.ModGUI.Builder.CreateButton(planet_Container,30,30,0,0,PreviousPlanet_Button_Click,"<");
                _planetName_Label = SFS.UI.ModGUI.Builder.CreateLabel(planet_Container, 270,30,0,0,MultiLaunchpadMod.SpaceCenterData.current.address);
                SFS.UI.ModGUI.Builder.CreateButton(planet_Container,30,30,0,0,NextPlanet_Button_Click,">");

                SFS.UI.ModGUI.Container location_Container = SFS.UI.ModGUI.Builder.CreateContainer(window);
                location_Container.CreateLayoutGroup(SFS.UI.ModGUI.Type.Horizontal, UnityEngine.TextAnchor.MiddleCenter,10f);
                _previousLocation_Button= SFS.UI.ModGUI.Builder.CreateButton(location_Container,30,30,0,0,PreviousLocation_Button_Click,"<");
                _locationName_Label = SFS.UI.ModGUI.Builder.CreateLabel(location_Container, 270,30,0,0,MultiLaunchpadMod.SpaceCenterData.current.location);
                _nextLocation_Button= SFS.UI.ModGUI.Builder.CreateButton(location_Container,30,30,0,0,NextLocation_Button_Click,">");

                _isActive = true;
                LoadPlanetsList();
            }

            /// <summary>Note that the GUI is no longer active</summary>
            public static void GUIInActive()
            {
                _isActive = false;
            }
        #endregion
    }
}
