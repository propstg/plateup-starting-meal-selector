using Kitchen;
using System;
using UnityEngine;

namespace KitchenStartingMealSelector {

    public class SmsPreferences {

        public static readonly Pref StartWithMeal = new Pref(Main.MOD_ID, nameof(StartWithMeal));
        public static readonly Pref StartWithSeed = new Pref(Main.MOD_ID, nameof(StartWithSeed));
        public static readonly Pref StartWithSetting = new Pref(Main.MOD_ID, nameof(StartWithSetting));

        public static bool preferencesLoaded = false;

        public static void registerPreferences() {
            if (!preferencesLoaded) {
                preferencesLoaded = true;
                Preferences.AddPreference<int>(new IntPreference(StartWithMeal, 0));
                Preferences.AddPreference<string>(new StringPreference(StartWithSeed, ""));
                Preferences.AddPreference<int>(new IntPreference(StartWithSetting, 0));
                Preferences.Load();
            }
        }

        public static int getStartWithMeal() {
            return Preferences.Get<int>(StartWithMeal);
        }

        public static void setStartWithMeal(int value) {
            Preferences.Set<int>(StartWithMeal, value);
        }

        public static string getStartWithSeed() {
            return Preferences.Get<string>(StartWithSeed);
        }

        public static void setStartWithSeed(string value) {
            Preferences.Set<string>(StartWithSeed, value);
        }

        public static int getStartWithSetting() {
            return Preferences.Get<int>(StartWithSetting);
        }

        public static void setStartWithSetting(int value) {
            Preferences.Set<int>(StartWithSetting, value);
        }
    }

    public class StringPreference : Preference<string> {

        public StringPreference(Pref key, string default_value, Action<string> action = null) : base(key, default_value, action) { }

        public override void Save() {
            PlayerPrefs.SetString(Key, Value);
        }

        public override void Load() {
            if (PlayerPrefs.HasKey(Key)) {
                Value = PlayerPrefs.GetString(Key);
            } else {
                Value = Default;
            }
        }
    }
}
