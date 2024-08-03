using Kitchen;
using KitchenMods;
using KitchenStartingMealSelector;
using System.Linq;
using Unity.Entities;

namespace StartingMealSelector.maps {

    public class SelectMapAtStartOfGameSystem : FranchiseFirstFrameSystem, IModSystem {

        public static bool isIntegratedHqInstalled => ModPreload.Mods.Any(mod => mod.Name == "Integrated HQ");

        protected override void Initialise() {
            base.Initialise();
        }

        protected override void OnUpdate() {
            var seed = SmsPreferences.getStartWithSeed();

            if (seed != "") {
                using var seedQuery = EntityManager.CreateEntityQuery((ComponentType)typeof(CSeededRunInfo));
                var seededRunInfoEntity = seedQuery.First();
                var seededRunInfo = EntityManager.GetComponentData<CSeededRunInfo>(seededRunInfoEntity);
                seededRunInfo.IsSeedOverride = true;
                seededRunInfo.FixedSeed = new Seed(seed);
                EntityManager.SetComponentData(seededRunInfoEntity, seededRunInfo);

                if (isIntegratedHqInstalled) {
                    Main.Log("Integrated HQ is installed. Skipping loading setting.");
                    return;
                }

                using var settingQuery = EntityManager.CreateEntityQuery((ComponentType)typeof(CSettingSelector));
                var settingEntity = settingQuery.First();
                var settingComponent = EntityManager.GetComponentData<CSettingSelector>(settingEntity);
                settingComponent.SettingID = SmsPreferences.getStartWithSetting();
                EntityManager.SetComponentData(settingEntity, settingComponent);
            }
        }
    }
}
