using System.Linq; // contains extensions
using HarmonyLib; // contains extensions

namespace MultiLaunchpadMod
{
    [HarmonyLib.HarmonyPatch(typeof( SFS.WorldBase.PlanetLoader), "LoadSolarSystem")]
    class WorldBase_PlanetLoader
    {
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
                    if (filePath.FileExists() && !SFS.Parsers.Json.JsonWrapper.TryLoadJson<System.Collections.Generic.List<MultiLaunchpadMod.SpaceCenterData>>(filePath, out __state))
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
    }
}