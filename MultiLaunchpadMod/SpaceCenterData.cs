namespace MultiLaunchpadMod
{
    [System.Serializable]
    public class SpaceCenterData
    {
        [System.Serializable]
        public class BuildingPosition
        {
            /// <summary>Horizontal position the launch pad relative to the space center (meters) +ve right</summary>
            public double horizontalPosition=365.0;

            /// <summary>Vertical position the launch pad relative to the space center (meters) +ve up</summary>
            public double height=26.2;

            public BuildingPosition(double horizontalPosition_P, double height_P)
            {
                height=height_P;
                horizontalPosition=horizontalPosition_P;
            }
        }

        /// <summary>
        /// The current enable status:
        /// 0 - disabled (inteded to be set by other mods)
        /// 1 - enabled
        /// 2 - enabled when landed on planet challenge accomplished
        /// ... others may follow
        /// </summary>
        public int enabled=1;

        /// <summary>The difficulty that this space center is enabled for: "all", "normal", "hard", "realistic"</summary>
        public string difficulty = "all";

        /// <summary>The name of the planet where the space center is</summary>
        public string address = "Earth";

        /// <summary>The name of the location on the planet where the space center is</summary>
        public string location = "(default)";

        /// <summary>The location on the planet (in degrees) of the space center (not fully implemented by SFS, location is, orientation is not)</summary>
        public double angle = 90.0;

        /// <summary>Position of the launch pad relative to the space center (meters)</summary>
        public BuildingPosition position_LaunchPad = new BuildingPosition(365.0, 26.2);

        /// <summary>The current space center </summary>
        public static SpaceCenterData current;

        /// <summary>All the possible space centers, indexed by planet name, location name</summary>
        public static System.Collections.Generic.SortedDictionary
            <
                string
                , System.Collections.Generic.SortedDictionary
                    <
                        string
                        , MultiLaunchpadMod.SpaceCenterData
                    >
            > alternates =
            new  System.Collections.Generic.SortedDictionary
                <
                    string
                    , System.Collections.Generic.SortedDictionary
                        <
                            string
                            , MultiLaunchpadMod.SpaceCenterData
                        >
                >();
    }
}
