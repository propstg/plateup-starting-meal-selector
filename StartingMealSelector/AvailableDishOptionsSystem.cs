﻿using Kitchen;
using KitchenData;
using KitchenMods;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;

namespace KitchenStartingMealSelector {

    public class AvailableDishOptionsSystem : FranchiseSystem, IModSystem {

        private static bool readyToRunOnce = false;

        protected override void OnUpdate() {
            bool firstFrame = Has<CSceneFirstFrame>();
            if (Has<CSceneFirstFrame>() || Main.refreshOptions) {
                readyToRunOnce = true;
                return;
            }

            if (!readyToRunOnce) {
                return;
            }

            using var dishUpgradesQuery = EntityManager.CreateEntityQuery((ComponentType)typeof(CDishUpgrade));
            if (dishUpgradesQuery.CalculateEntityCount() == 0) {
                Main.Log("No CDishUpgrades found");
                return;
            }
            readyToRunOnce = false;

            Main.Log($"Loading dish upgrades...");

            Main.loadedAvailableMenuOptions.Clear();
            Main.loadedAvailableMenuOptionNames.Clear();
            Main.availableMenuOptions.Clear();

            addAlwaysAvailableOption();
            addOptionsThatUserHasUnlocked(dishUpgradesQuery);
            addIdNamePair(536093200, "Nut Roast");
            addIdNamePair(373996608, "Ice Cream");
            Main.Log(Main.availableMenuOptions.Keys.ToList());
            Main.Log(Main.availableMenuOptions.Values.ToList());
            Main.availableMenuOptions = Main.availableMenuOptions.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            Main.availableMenuOptions = Main.availableMenuOptions.Prepend(new KeyValuePair<int, string>(0, "Random")).ToDictionary(x => x.Key, x => x.Value); ;
            Main.availableMenuOptions = Main.availableMenuOptions.Prepend(new KeyValuePair<int, string>(-1, "Surprise me")).ToDictionary(x => x.Key, x => x.Value); ;
            
            Main.Log($"Found dish upgrades: {string.Join(", ", Main.loadedAvailableMenuOptions.Select(item => item.ToString()))}");
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
        
        private void addOptionsThatUserHasUnlocked(EntityQuery dishUpgradesQuery) {
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

            Main.Log($"Attempting to load {dishId}");
            Dish dish = getGdo(dishId);

            string dishName = dish != null ? dish.Name : dishId.ToString();
            Main.Log($"Loaded {dishId}, {dishName}");

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
