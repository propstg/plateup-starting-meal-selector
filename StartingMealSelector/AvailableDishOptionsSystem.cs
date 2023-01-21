using Kitchen;
using KitchenData;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using KitchenLib.Utils;

namespace KitchenStartingMealSelector {

    public class AvailableDishOptionsSystem : FranchiseFirstFrameSystem {

        private EntityQuery dishUpgradesQuery;

        protected override void Initialise() {
            base.Initialise();
            dishUpgradesQuery = GetEntityQuery((ComponentType)typeof(CDishUpgrade));
            Debug.Log($"[{Mod.MOD_ID}] AvailableDishOptionsSystem initialized.");
        }

        protected override void OnUpdate()
		{
			Mod.loadedAvailableMenuOptions.Add(0);
			Mod.loadedAvailableMenuOptionNames.Add("Random");

			foreach (int dishID in dishUpgradesQuery
				.ToComponentDataArray<CDishUpgrade>(Unity.Collections.Allocator.Temp)
				.ToList<CDishUpgrade>()
				.Select(item => item.DishID).ToList())
			{
				Dish dish = (Dish)GDOUtils.GetExistingGDO(dishID);
				Mod.loadedAvailableMenuOptions.Add(dishID);
				if (dish == null)
					dish = (Dish)GDOUtils.GetCustomGameDataObject(dishID).GameDataObject;
				if (dish != null)
					Mod.loadedAvailableMenuOptionNames.Add(dish.Name);
				else
					Mod.loadedAvailableMenuOptionNames.Add(dishID.ToString());
			}
			
            Debug.LogWarning($"[{Mod.MOD_ID}] Found dish upgrades: {string.Join(", ", Mod.loadedAvailableMenuOptions.Select(item => item.ToString()))}");
        }
    }
}
