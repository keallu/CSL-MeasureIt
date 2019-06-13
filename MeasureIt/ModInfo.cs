using Harmony;
using ICities;
using System.Reflection;

namespace MeasureIt
{
    public class ModInfo : IUserMod
    {
        public string Name => "Measure It!";
        public string Description => "Allows to change the zone radius effect when placing roads.";

        public void OnEnabled()
        {
            var harmony = HarmonyInstance.Create("com.github.keallu.csl.measureit");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void OnDisabled()
        {
            var harmony = HarmonyInstance.Create("com.github.keallu.csl.measureit");
            harmony.UnpatchAll();
        }

        public void OnSettingsUI(UIHelperBase helper)
        {
            UIHelperBase group;
            bool selected;

            group = helper.AddGroup(Name);

            selected = ModConfig.Instance.ShowMeasurePanel;
            group.AddCheckbox("Show Measure Panel", selected, sel =>
            {
                ModConfig.Instance.ShowMeasurePanel = sel;
                ModConfig.Instance.Save();
            });

            group.AddSpace(10);

            group.AddButton("Reset Positioning of Measure Panel", () =>
            {
                ToggleProperties.Instance.ResetPanelPosition();
            });
        }
    }
}