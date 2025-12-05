namespace MultiLaunchpadMod
{
    [System.Serializable]
    public class SpaceCenterData
    {
        [System.Serializable]
        public class BuildingPosition
        {
            /// <summary>Horizontal position the launch pad relative to the space center (meters) +ve right, default 365</summary>
            public double horizontalPosition=365.0;

            /// <summary>Vertical position the launch pad relative to the planet radius +ve up, if omitted attempt to find the best value</summary>
            public double? height=null;
        }

        /// <summary>
        /// The current enable status:
        /// 0 - disabled (intended to be set by other mods)
        /// 1 - enabled
        /// 2 - enabled when challenge accomplished
        /// ... others may follow
        /// default 1
        /// </summary>
        public int enabled=1;

        /// <summary>the challenge id to be used with enabled=2, if omitted will be the "landed on planet and returned safely" challenge</summary>
        public string challenge_id = "";

        /// <summary>The difficulty that this space center is enabled for: "all", "normal", "hard", "realistic"</summary>
        /// default "all"
        public string difficulty = "all";

        /// <summary>The name of the planet where the space center is, default "Earth"</summary>
        public string address = "Earth";

        /// <summary>The name of the location on the planet where the space center is</summary>
        public string location = "(default)";

        /// <summary>The location on the planet (in degrees) of the space center (not fully implemented by SFS, location is, orientation is not), default 90</summary>
        public double angle = 90.0;

        /// <summary>Position of the launch pad relative to the space center (meters)</summary>
        public BuildingPosition position_LaunchPad = new BuildingPosition();

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
