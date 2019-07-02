using ICities;

namespace MeasureIt
{
    public class ModInfo : IUserMod
    {
        public string Name => "Measure It!";
        public string Description => "Helps with measurements.";

        public static readonly string[] UnitOfDistanceLabels =
        {
            "Unit",
            "Metre",
            "Yard",
            "Foot"
        };

        public static readonly int[] UnitOfDistanceValues =
        {
            0,
            1,
            2,
            3
        };

        public static readonly string[] UnitOfSlopeLabels =
        {
            "Degree",
            "Percentage"
        };

        public static readonly int[] UnitOfSlopeValues =
        {
            0,
            1
        };

        public static readonly string[] UnitOfDirectionLabels =
        {
            "Degree",
            "Point"
        };

        public static readonly int[] UnitOfDirectionValues =
        {
            0,
            1
        };

        public void OnSettingsUI(UIHelperBase helper)
        {
            UIHelperBase group;
            bool selected;
            int selectedValue;

            group = helper.AddGroup(Name);

            selected = ModConfig.Instance.ShowControlPanel;
            group.AddCheckbox("Show Control Panel", selected, sel =>
            {
                ModConfig.Instance.ShowControlPanel = sel;
                ModConfig.Instance.Save();
            });

            group.AddSpace(10);

            group.AddButton("Reset Positioning of Control Panel", () =>
            {
                ModProperties.Instance.ResetControlPanelPosition();
            });

            group = helper.AddGroup("Measurements");

            selected = ModConfig.Instance.ShowInfoPanel;
            group.AddCheckbox("Show Info Panel", selected, sel =>
            {
                ModConfig.Instance.ShowInfoPanel = sel;
                ModConfig.Instance.Save();
            });

            selectedValue = ModConfig.Instance.UnitOfDistance;
            group.AddDropdown("Unit of Distance", UnitOfDistanceLabels, selectedValue, sel =>
            {
                ModConfig.Instance.UnitOfDistance = UnitOfDistanceValues[sel];
                ModConfig.Instance.Save();
            });

            selectedValue = ModConfig.Instance.UnitOfSlope;
            group.AddDropdown("Unit of Slope", UnitOfSlopeLabels, selectedValue, sel =>
            {
                ModConfig.Instance.UnitOfSlope = UnitOfSlopeValues[sel];
                ModConfig.Instance.Save();
            });

            selectedValue = ModConfig.Instance.UnitOfDirection;
            group.AddDropdown("Unit of Direction", UnitOfDirectionLabels, selectedValue, sel =>
            {
                ModConfig.Instance.UnitOfDirection = UnitOfDirectionValues[sel];
                ModConfig.Instance.Save();
            });

            group.AddSpace(10);

            group.AddButton("Reset Positioning of Info Panel", () =>
            {
                ModProperties.Instance.ResetInfoPanelPosition();
            });
        }
    }
}