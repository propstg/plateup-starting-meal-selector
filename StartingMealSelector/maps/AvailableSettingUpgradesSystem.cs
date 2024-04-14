using Kitchen;
using KitchenData;
using KitchenMods;
using System.Linq;
using Unity.Entities;

namespace KitchenStartingMealSelector.maps {

    public class AvailableSettingUpgradesSystem : FranchiseFirstFrameSystem, IModSystem {

        private EntityQuery settingUpgradesQuery;

        protected override void Initialise() {
            base.Initialise();
            settingUpgradesQuery = GetEntityQuery((ComponentType)typeof(CSettingUpgrade));
            Main.Log($"AvailableSettingUpgradesSystem initialized.");
        }

        protected override void OnUpdate() {
            Main.Log($"Loading setting upgrades...");
            Main.availableSettingOptions.Clear();

            settingUpgradesQuery
                .ToComponentDataArray<CSettingUpgrade>(Unity.Collections.Allocator.Temp)
                .ToList<CSettingUpgrade>()
                .Select(item => item.SettingID).ToList()
                .ForEach(addDishOption);

            Main.Log($"Found setting upgrades: {string.Join(", ", Main.availableSettingOptions.Values)}");
        }

        private void addDishOption(int settingId) {
            RestaurantSetting setting = getGdo(settingId);
            string settingName = setting != null ? setting.Name : settingId.ToString();
            Main.availableSettingOptions.Add(settingId, settingName);
        }

        private RestaurantSetting getGdo(int id) {
            return GameData.Main.Get<GameDataObject>()
                .Where(gdo => gdo.ID == id)
                .Select(gdo => (RestaurantSetting)gdo)
                .First();
        }
    }
}
