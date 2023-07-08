using Kitchen;
using KitchenData;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;

namespace KitchenStartingMealSelector {

    public class AvailableDishOptionsSystem : GameSystemBase {

        private EntityQuery dishUpgradesQuery;

        protected override void Initialise() {
            base.Initialise();
            dishUpgradesQuery = GetEntityQuery((ComponentType)typeof(CDishUpgrade));
            Debug.Log($"[{Main.MOD_ID}] AvailableDishOptionsSystem initialized.");
        }

        protected override void OnUpdate() {
            if (!(Has<CSceneFirstFrame>() || Main.refreshOptions)) {
                return;
            }

            Debug.Log($"[{Main.MOD_ID}] Loading dish upgrades...");

            Main.loadedAvailableMenuOptions.Clear();
            Main.loadedAvailableMenuOptionNames.Clear();
            Main.availableMenuOptions.Clear();

            addAlwaysAvailableOption();
            addOptionsThatUserHasUnlocked();
            addIdNamePair(536093200, "Nut Roast");
            Debug.Log(Main.availableMenuOptions.Keys.ToList());
            Debug.Log(Main.availableMenuOptions.Values.ToList());
            Main.availableMenuOptions = Main.availableMenuOptions.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            Main.availableMenuOptions = Main.availableMenuOptions.Prepend(new KeyValuePair<int, string>(0, "Random")).ToDictionary(x => x.Key, x => x.Value); ;
            Main.availableMenuOptions = Main.availableMenuOptions.Prepend(new KeyValuePair<int, string>(-1, "Surprise me")).ToDictionary(x => x.Key, x => x.Value); ;
            
            Debug.Log($"[{Main.MOD_ID}] Found dish upgrades: {string.Join(", ", Main.loadedAvailableMenuOptions.Select(item => item.ToString()))}");
            Main.refreshOptions = false;
        }

        private void addIdNamePair(int id, string name) {
            Main.loadedAvailableMenuOptions.Add(id);
            Main.loadedAvailableMenuOptionNames.Add(name);
            Main.availableMenuOptions.Add(id, name);
        }

        private void addAlwaysAvailableOption() {
            addDishOption(AssetReference.AlwaysAvailableDish);
        }
        
        private void addOptionsThatUserHasUnlocked() {
            dishUpgradesQuery
                .ToComponentDataArray<CDishUpgrade>(Unity.Collections.Allocator.Temp)
                .ToList<CDishUpgrade>()
                .Select(item => item.DishID).ToList()
                .ForEach(addDishOption);
        }

        private void addDishOption(int dishId) {
            if (Main.loadedAvailableMenuOptions.Contains(dishId) || dishId == 0) {
                return;
            }

            Debug.Log("Looking up " + dishId);
            Dish dish = getGdo(dishId);

            string dishName = dish != null ? dish.Name : dishId.ToString();

            Main.loadedAvailableMenuOptions.Add(dishId);
            Main.loadedAvailableMenuOptionNames.Add(dishName);
            Main.availableMenuOptions.Add(dishId, dishName);
        }

        private Dish getGdo(int id) {
            return GameData.Main.Get<GameDataObject>()
                .Where(gdo => gdo.ID == id)
                .Select(gdo => (Dish)gdo)
                .First();
        }
    }
}
