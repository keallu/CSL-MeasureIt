namespace MeasureIt
{
    [ConfigurationPath("MeasureItConfig.xml")]
    public class ModConfig
    {
        public bool ConfigUpdated { get; set; }
        public bool ShowControlPanel { get; set; } = true;
        public float ControlPanelPositionX { get; set; }
        public float ControlPanelPositionY { get; set; }
        public int Cells { get; set; } = 4;
        public bool ShowInfoPanel { get; set; } = true;
        public float InfoPanelPositionX { get; set; }
        public float InfoPanelPositionY { get; set; }
        public int UnitOfLength { get; set; } = 1;
        public int UnitOfSlope { get; set; } = 1;
        public int UnitOfDirection { get; set; } = 1;

        private static ModConfig instance;

        public static ModConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Configuration<ModConfig>.Load();
                }

                return instance;
            }
        }

        public void Save()
        {
            Configuration<ModConfig>.Save();
            ConfigUpdated = true;
        }
    }
}