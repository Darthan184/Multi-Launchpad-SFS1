using System.Linq; // contains extensions
using HarmonyLib; // contains extensions

namespace MultiLaunchpadMod
{
    [HarmonyLib.HarmonyPatch(typeof( SFS.WorldBase.PlanetLoader), "LoadSolarSystem")]
    class WorldBase_PlanetLoader
    {
        static void Prefix
            (
                SFS.WorldBase.WorldSettings settings, SFS.I_MsgLogger log
                , System.Action<bool> callback
                , out System.Collections.Generic.List<MultiLaunchpadMod.SpaceCenterData> __state
            )
        {
            SFS.WorldBase.SolarSystemReference solarSystem = settings.solarSystem;
            __state = new  System.Collections.Generic.List<MultiLaunchpadMod.SpaceCenterData>();

            if (solarSystem.name.Length > 0)
            {
                SFS.IO.FilePath filePath = FileLocations.SolarSystemsFolder.Extend(solarSystem.name).ExtendToFile("Alternate_Space_Center_Data.txt");

                if (filePath.FileExists() && !SFS.Parsers.Json.JsonWrapper.TryLoadJson<System.Collections.Generic.List<MultiLaunchpadMod.SpaceCenterData>>(filePath, out __state))
                {
                    log.Log("MultiLaunchpadMod: Solar system \"" + solarSystem.name + "\" has an invalid Alternate_Space_Center_Data.txt file");
                    __state.Clear();
                }
            }
        }

        static void Postfix(System.Collections.Generic.List<MultiLaunchpadMod.SpaceCenterData> __state)
        {
            // copy the list as read into a dictionary, ignoring unknown planets
            foreach (MultiLaunchpadMod.SpaceCenterData oneSpaceCenter in __state)
            {
                if (SFS.Base.planetLoader.planets.ContainsKey(oneSpaceCenter.address))
                {
                    MultiLaunchpadMod.SpaceCenterData.alternates[oneSpaceCenter.address]=oneSpaceCenter;
                }
            }

            // add or update the default space center
            MultiLaunchpadMod.SpaceCenterData thisSpaceCenter;
            if (!MultiLaunchpadMod.SpaceCenterData.alternates.ContainsKey(SFS.Base.planetLoader.spaceCenter.address))
            {
                thisSpaceCenter = new MultiLaunchpadMod.SpaceCenterData();
                thisSpaceCenter.address =SFS.Base.planetLoader.spaceCenter.address;
                MultiLaunchpadMod.SpaceCenterData.alternates[SFS.Base.planetLoader.spaceCenter.address]=thisSpaceCenter;
            }
            else
            {
                thisSpaceCenter = MultiLaunchpadMod.SpaceCenterData.alternates[SFS.Base.planetLoader.spaceCenter.address];
            }
            thisSpaceCenter.enabled=1;
            thisSpaceCenter.angle=SFS.Base.planetLoader.spaceCenter.angle;
            thisSpaceCenter.position_LaunchPad.horizontalPosition= SFS.Base.planetLoader.spaceCenter.position_LaunchPad.horizontalPosition;
            thisSpaceCenter.position_LaunchPad.height= SFS.Base.planetLoader.spaceCenter.position_LaunchPad.height;
        }
    }
}