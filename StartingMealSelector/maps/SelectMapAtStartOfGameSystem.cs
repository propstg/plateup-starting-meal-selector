using Kitchen;
using KitchenMods;
using KitchenStartingMealSelector;
using System.Linq;
using Unity.Entities;

namespace StartingMealSelector.maps {

    public class SelectMapAtStartOfGameSystem : FranchiseSystem, IModSystem {

        static bool readyToRunOnce = false;

        protected override void OnUpdate() {
            if (Has<CSceneFirstFrame>()) {
                readyToRunOnce = true;
                return;
            }

            if (!readyToRunOnce) {
                return;
            }

            if (Main.isIntegratedHqInstalled) {
                Main.Log("Integrated HQ is installed. Skipping loading seed.");
                return;
            }

            var seed = SmsPreferences.getStartWithSeed();

            if (seed != "") {
                Main.Log($"Attempting to set seed to ${seed} and setting to ${SmsPreferences.getStartWithSetting()}");
                using var seedQuery = EntityManager.CreateEntityQuery((ComponentType)typeof(CSeededRunInfo));
                if (!seedQuery.IsEmpty) {
                    var seededRunInfoEntity = seedQuery.First();
                    var seededRunInfo = EntityManager.GetComponentData<CSeededRunInfo>(seededRunInfoEntity);
                    seededRunInfo.IsSeedOverride = true;
                    seededRunInfo.FixedSeed = new Seed(seed);
                    EntityManager.SetComponentData(seededRunInfoEntity, seededRunInfo);
                    Main.Log("Seed set.");
                } else {
                    Main.Log("Unable to set seed, because seedQuery returned no results");
                }

                using var settingQuery = EntityManager.CreateEntityQuery(typeof(CSettingSelector));
                if (!settingQuery.IsEmpty) {
                    var settingEntity = settingQuery.First();
                    var settingComponent = EntityManager.GetComponentData<CSettingSelector>(settingEntity);
                    settingComponent.SettingID = SmsPreferences.getStartWithSetting();
                    EntityManager.SetComponentData(settingEntity, settingComponent);
                    Main.Log("Setting set.");
                } else {
                    Main.Log("Unable to set setting, because settingQuery returned no results");
                }
            }

            readyToRunOnce = false;
        }
    }
}
