using System.Linq; // contains extensions
using HarmonyLib; // contains extensions

namespace MultiLaunchpadMod
{
    [HarmonyLib.HarmonyPatch(typeof( SFS.WorldBase.PlanetLoader), "LoadSolarSystem")]
    class WorldBase_PlanetLoader
    {
         #region "Private classes"
            private class  _TerrainRange
            {
                public double minTerrain = double.MaxValue;
                public double maxTerrain = double.MinValue;
            }
        #endregion

        #region "Private methods"
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
        #endregion

        #region "Harmony methods"
            static void Prefix
                (
                    SFS.WorldBase.WorldSettings settings
                    , SFS.I_MsgLogger log
                    , System.Action<bool> callback
                    , out System.Collections.Generic.List<MultiLaunchpadMod.SpaceCenterData> __state
                )
            {
                SFS.WorldBase.SolarSystemReference solarSystem = settings.solarSystem;
                __state = new  System.Collections.Generic.List<MultiLaunchpadMod.SpaceCenterData>();

                if (solarSystem.name.Length > 0)
                {
                    SFS.IO.FilePath filePath = FileLocations.SolarSystemsFolder.Extend(solarSystem.name).ExtendToFile("Alternate_Space_Center_Data.txt");

                    try
                    {
                        if (!filePath.FileExists())
                        {
                            __state.Add(new MultiLaunchpadMod.SpaceCenterData());
                        }
                        else if (!SFS.Parsers.Json.JsonWrapper.TryLoadJson<System.Collections.Generic.List<MultiLaunchpadMod.SpaceCenterData>>(filePath, out __state))
                        {
                            UnityEngine.Debug.LogError("MultiLaunchpadMod: Solar system \"" + solarSystem.name + "\" has an invalid Alternate_Space_Center_Data.txt file");
                            __state.Clear();
                        }
                    }
                    catch (System.Exception excp)
                    {
                        UnityEngine.Debug.LogError("MultiLaunchpadMod: Solar system \"" + solarSystem.name + "\" has an invalid Alternate_Space_Center_Data.txt file: " + excp.Message);
                        __state.Clear();
                    }
                }
            }

            static void Postfix(System.Collections.Generic.List<MultiLaunchpadMod.SpaceCenterData> __state)
            {
                MultiLaunchpadMod.SpaceCenterData.alternates.Clear();
                System.Collections.Generic.List<MultiLaunchpadMod.SpaceCenterData> expandableSpaceCenters=
                    new System.Collections.Generic.List<MultiLaunchpadMod.SpaceCenterData>();

                // create list of expandable entries
                foreach (MultiLaunchpadMod.SpaceCenterData oneSpaceCenter in __state)
                    if (oneSpaceCenter.address=="")
                    {
                        expandableSpaceCenters.Add(oneSpaceCenter);
                    }

                 // expand each entry
                foreach (MultiLaunchpadMod.SpaceCenterData oneSpaceCenter in expandableSpaceCenters)
                {
                    __state.Remove(oneSpaceCenter);

                    foreach (SFS.WorldBase.Planet onePlanet in SFS.Base.planetLoader.planets.Values)
                        if
                            (
                                onePlanet.data.hasTerrain
                                && (oneSpaceCenter.inclInsignificant || onePlanet.data.basics.significant)
                                && !oneSpaceCenter.exclude.Contains(onePlanet.codeName)
                                &&
                                    (
                                        oneSpaceCenter.primaries.Length==0
                                        || (onePlanet.parentBody!=null && oneSpaceCenter.primaries.Contains(onePlanet.parentBody.codeName))
                                    )
                            )
                        {
                            MultiLaunchpadMod.SpaceCenterData newSpaceCenter = new MultiLaunchpadMod.SpaceCenterData();
                            newSpaceCenter.address = onePlanet.codeName;
                            newSpaceCenter.enabled = oneSpaceCenter.enabled;
                            newSpaceCenter.challenge_id = oneSpaceCenter.challenge_id.Replace("{planet}",onePlanet.codeName);
                            newSpaceCenter.difficulty=oneSpaceCenter.difficulty;
                            newSpaceCenter.angle=oneSpaceCenter.angle;

                            if (oneSpaceCenter.position_LaunchPad==null)
                            {
                                newSpaceCenter.position_LaunchPad=null;
                            }
                            else
                            {
                                newSpaceCenter.position_LaunchPad=new MultiLaunchpadMod.SpaceCenterData.BuildingPosition(oneSpaceCenter.position_LaunchPad);
                            }
                            __state.Add(newSpaceCenter);
                        }
                }

                // copy the list as read into a dictionary, ignoring unknown planets
                foreach (MultiLaunchpadMod.SpaceCenterData oneSpaceCenter in __state)
                {
                    if (SFS.Base.planetLoader.planets.ContainsKey(oneSpaceCenter.address))
                    {
                        bool canAdd=false;

                        if (SFS.Base.worldBase!=null && SFS.Base.worldBase.settings!=null && SFS.Base.worldBase.settings.difficulty!=null)
                        {
                            switch (SFS.Base.worldBase.settings.difficulty.difficulty)
                            {
                                case SFS.WorldBase.Difficulty.DifficultyType.Normal:
                                    canAdd=(oneSpaceCenter.difficulty.ToLower()=="all" || oneSpaceCenter.difficulty.ToLower()=="normal" );
                                break;

                                case SFS.WorldBase.Difficulty.DifficultyType.Hard:
                                    canAdd=(oneSpaceCenter.difficulty.ToLower()=="all" || oneSpaceCenter.difficulty.ToLower()=="hard" );
                                break;

                                case SFS.WorldBase.Difficulty.DifficultyType.Realistic:
                                    canAdd=(oneSpaceCenter.difficulty.ToLower()=="all" || oneSpaceCenter.difficulty.ToLower()=="realistic" );
                                break;

                                default:
                                    canAdd=(oneSpaceCenter.difficulty.ToLower()=="all");
                                break;
                            }
                        }
                        else
                        {
                            canAdd=(oneSpaceCenter.difficulty.ToLower()=="all");
                        }
                        if (canAdd)
                        {
                            if (oneSpaceCenter.position_LaunchPad==null)
                                oneSpaceCenter.position_LaunchPad = new MultiLaunchpadMod.SpaceCenterData.BuildingPosition();

                            if (oneSpaceCenter.position_LaunchPad.height==null)
                            {
                                SFS.WorldBase.Planet planet = SFS.Base.planetLoader.planets[oneSpaceCenter.address];
                                double edgeOffset = 50 / planet.Radius;
                                double hubOffset = oneSpaceCenter.position_LaunchPad.horizontalPosition / planet.Radius;
                                _TerrainRange range=null;

                                if (oneSpaceCenter.angle==null)
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
                                            oneSpaceCenter.angle=angleRadians*180.0/System.Math.PI;
                                            isFound=true;
                                            break;
                                        }

                                        if (angleOffset!=0.0)
                                        {
                                            angleRadians = System.Math.PI/2.0-angleOffset;
                                            range=GetTerrainRange(planet, angleRadians, hubOffset, edgeOffset);

                                            if (range.maxTerrain-range.minTerrain<8)
                                            {
                                                oneSpaceCenter.angle=angleRadians*180.0/System.Math.PI;
                                                isFound=true;
                                                break;
                                            }
                                        }
                                    }

                                    if (!isFound)
                                    {
                                        oneSpaceCenter.angle=90.0;
                                        range=GetTerrainRange(planet, System.Math.PI/2.0,hubOffset,edgeOffset);
                                    }
                                }
                                else
                                {
                                    double angleRadians =  ((double)oneSpaceCenter.angle*System.Math.PI / 180.0);
                                    range=GetTerrainRange(planet, angleRadians, hubOffset, edgeOffset);
                                    oneSpaceCenter.angle = (double)oneSpaceCenter.angle;
                                }

                                oneSpaceCenter.position_LaunchPad.height = System.Math.Max(range.minTerrain+8,range.maxTerrain);
                            }
                            else
                            {
                                if (oneSpaceCenter.angle==null) oneSpaceCenter.angle=90;
                                oneSpaceCenter.position_LaunchPad.height = (double)oneSpaceCenter.position_LaunchPad.height;
                            }
                            if (!MultiLaunchpadMod.SpaceCenterData.alternates.ContainsKey(oneSpaceCenter.address))
                            {
                                MultiLaunchpadMod.SpaceCenterData.alternates[oneSpaceCenter.address] =
                                    new  System.Collections.Generic.SortedDictionary<string, MultiLaunchpadMod.SpaceCenterData>();
                            }
                            MultiLaunchpadMod.SpaceCenterData.alternates[oneSpaceCenter.address][oneSpaceCenter.location]=oneSpaceCenter;
                        }
                    }
                }

                // add or update the default space center and the current space center
                if (!MultiLaunchpadMod.SpaceCenterData.alternates.ContainsKey(SFS.Base.planetLoader.spaceCenter.address))
                {
                    MultiLaunchpadMod.SpaceCenterData.current = new MultiLaunchpadMod.SpaceCenterData();
                    MultiLaunchpadMod.SpaceCenterData.current.address =SFS.Base.planetLoader.spaceCenter.address;
                    MultiLaunchpadMod.SpaceCenterData.current.location = "(default)";
                    MultiLaunchpadMod.SpaceCenterData.alternates[MultiLaunchpadMod.SpaceCenterData.current.address] =
                        new  System.Collections.Generic.SortedDictionary<string, MultiLaunchpadMod.SpaceCenterData>();
                    MultiLaunchpadMod.SpaceCenterData.alternates[MultiLaunchpadMod.SpaceCenterData.current.address][MultiLaunchpadMod.SpaceCenterData.current.location]=MultiLaunchpadMod.SpaceCenterData.current;
                }
                else if (! MultiLaunchpadMod.SpaceCenterData.alternates[SFS.Base.planetLoader.spaceCenter.address].ContainsKey("(default)"))
                {
                    MultiLaunchpadMod.SpaceCenterData.current = new MultiLaunchpadMod.SpaceCenterData();
                    MultiLaunchpadMod.SpaceCenterData.current.address =SFS.Base.planetLoader.spaceCenter.address;
                    MultiLaunchpadMod.SpaceCenterData.current.location = "(default)";
                    MultiLaunchpadMod.SpaceCenterData.alternates[MultiLaunchpadMod.SpaceCenterData.current.address][MultiLaunchpadMod.SpaceCenterData.current.location]=MultiLaunchpadMod.SpaceCenterData.current;
                }
                else
                {
                    MultiLaunchpadMod.SpaceCenterData.current = MultiLaunchpadMod.SpaceCenterData.alternates[SFS.Base.planetLoader.spaceCenter.address]["(default)"];
                }
                MultiLaunchpadMod.SpaceCenterData.current.enabled=1;
                MultiLaunchpadMod.SpaceCenterData.current.angle=SFS.Base.planetLoader.spaceCenter.angle;
                MultiLaunchpadMod.SpaceCenterData.current.position_LaunchPad.horizontalPosition= SFS.Base.planetLoader.spaceCenter.position_LaunchPad.horizontalPosition;
                MultiLaunchpadMod.SpaceCenterData.current.position_LaunchPad.height= SFS.Base.planetLoader.spaceCenter.position_LaunchPad.height;
            }
        #endregion
    }
}