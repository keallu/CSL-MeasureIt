namespace MeasureIt
{
    [ConfigurationPath("MeasureItConfig.xml")]
    public class ModConfig
    {
        public bool ConfigUpdated { get; set; }
        public bool ShowMeasurePanel { get; set; } = true;
        public float PositionX { get; set; }
        public float PositionY { get; set; }
        public int Cells { get; set; } = 4;

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