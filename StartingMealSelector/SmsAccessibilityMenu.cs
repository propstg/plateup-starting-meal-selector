using UnityEngine;
using Kitchen.Modules;
using Kitchen;
using HarmonyLib;
using System.Reflection;
using System.Collections.Generic;

namespace KitchenStartingMealSelector {

    public class SmsAccessibilityMenu : Menu<MenuAction> {

        public SmsAccessibilityMenu(Transform container, ModuleList module_list) : base(container, module_list) { }

        public override void Setup(int player_id) {
            AddLabel("Menu Size");
            Add(new Option<float>(new List<float> { 1.0f, 1.3f, 1.5f, 1.7f }, SmsPreferences.getMenuSize(), new List<string> { "1.0x", "1.3x", "1.5x", "1.7x"}))
                .OnChanged += delegate (object _, float value) {
                    SmsPreferences.setMenuSize(value);
                };
            New<SpacerElement>();
            AddButton(Localisation["MENU_BACK_SETTINGS"], delegate { RequestPreviousMenu(); });
        }
    }

    [HarmonyPatch(typeof(AccessibilityMenu<MenuAction>), "Setup")]
    class AccessibilityMenu_Patch {

        public static bool Prefix(MainMenu __instance) {
            MethodInfo addSubmenu = __instance.GetType().GetMethod("AddSubmenuButton", BindingFlags.NonPublic | BindingFlags.Instance);
            addSubmenu.Invoke(__instance, new object[] { "Dish Selection Cabinet", typeof(SmsAccessibilityMenu), false });
            return true;
        }
    }

    [HarmonyPatch(typeof(PlayerPauseView), "SetupMenus")]
    class SmsAccessibilityPauseMenu_Patch {

        public static bool Prefix(PlayerPauseView __instance) {
            ModuleList moduleList = (ModuleList)__instance.GetType().GetField("ModuleList", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(__instance);
            MethodInfo addMenu = __instance.GetType().GetMethod("AddMenu", BindingFlags.NonPublic | BindingFlags.Instance);
            addMenu.Invoke(__instance, new object[] { typeof(SmsAccessibilityMenu), new SmsAccessibilityMenu(__instance.ButtonContainer, moduleList) });
            return true;
        }
    }

    [HarmonyPatch(typeof(DishSelectionIndicator), "SetNewMenu")]
    class DishSelectionIndicator_SetNewMenu_Patch {

        public static void Postfix(Transform ___Container) {
            float menuSize = SmsPreferences.getMenuSize();
            ___Container.localScale = new Vector3(menuSize, menuSize, 1f);
        }
    }
}
