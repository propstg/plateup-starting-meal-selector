using Kitchen;

namespace KitchenStartingMealSelector {

    public class SmsPreferences {

        public static readonly Pref StartWithMeal = new Pref(Main.MOD_ID, nameof(StartWithMeal));

        public static bool preferencesLoaded = false;

        public static void registerPreferences() {
            if (!preferencesLoaded) {
                preferencesLoaded = true;
                Preferences.AddPreference<int>(new IntPreference(StartWithMeal, 0));
                Preferences.Load();
            }
        }

        public static int getStartWithMeal() {
            return Preferences.Get<int>(StartWithMeal);
        }

        public static void setStartWithMeal(int value) {
            Preferences.Set<int>(StartWithMeal, value);
        }
    }
}
