using System;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using PolyTechFramework;
using BepInEx.Configuration;
using UnityEngine;
using System.Reflection;

namespace Rotation
{
    [BepInPlugin("org.bepinex.plugins.pb2betterrotation", "Better Rotation Mod", "1.1.0")]
    [BepInDependency(PolyTechMain.PluginGuid, BepInDependency.DependencyFlags.HardDependency)]
    public class PolyBridgeRotatorPlugin: PolyTechMod
    {
        public static ConfigEntry<bool> mEnabled;
        public static ConfigEntry<BepInEx.Configuration.KeyboardShortcut> _keybindMajorLeft;
        public static ConfigEntry<BepInEx.Configuration.KeyboardShortcut> _keybindMajorRight;
        public static ConfigEntry<BepInEx.Configuration.KeyboardShortcut> _keybindMinorLeft;
        public static ConfigEntry<BepInEx.Configuration.KeyboardShortcut> _keybindMinorRight;
        public static ConfigEntry<int> _controlScale;
        public static ConfigEntry<int> _controlShiftScale;

        public ConfigDefinition modEnableDef = new ConfigDefinition("Rotation Mod", "Enable/Disable Mod");

        public static bool saveEnabled;

        public static bool controlDown;
        public static bool shiftDown;
        public static float scrollClicks;

        public override void enableMod(){
            mEnabled.Value = saveEnabled;
        }
        public override void disableMod(){
            saveEnabled = mEnabled.Value;
            mEnabled.Value = false;
        }
        public override string getSettings(){return "";}
        public override void setSettings(string settings){}
        
        public PolyBridgeRotatorPlugin(){
            Config.Bind(modEnableDef, true, new ConfigDescription("Controls if the mod should be enabled or disabled", null, null));
            _keybindMajorLeft = Config.Bind(new ConfigDefinition("Major Left", "90 Degrees Left"), new BepInEx.Configuration.KeyboardShortcut(UnityEngine.KeyCode.None));
            _keybindMajorRight = Config.Bind(new ConfigDefinition("Major Right", "90 Degrees Right"), new BepInEx.Configuration.KeyboardShortcut(UnityEngine.KeyCode.None));
            _keybindMinorLeft = Config.Bind(new ConfigDefinition("Minor Left", "45 Degrees Left"), new BepInEx.Configuration.KeyboardShortcut(UnityEngine.KeyCode.None));
            _keybindMinorRight = Config.Bind(new ConfigDefinition("Minor Right", "45 Degrees Right"), new BepInEx.Configuration.KeyboardShortcut(UnityEngine.KeyCode.None));
            _controlScale = Config.Bind(new ConfigDefinition("Control + Scroll Scale", "The amount to rotate for each scroll wheel click"), 5);
            _controlShiftScale = Config.Bind(new ConfigDefinition("Control + Shift + Scroll Scale", "The amount to rotate for each scroll wheel click"), 1);
        }

        void Awake()
        {
            mEnabled = (ConfigEntry<bool>)Config[modEnableDef];
            this.isCheat = false;
            this.isEnabled = true;
            saveEnabled = true;
            controlDown = false;
            shiftDown = false;
            PolyTechMain.registerMod(this);
            Logger.LogInfo("die die I hate stuff die");
            Harmony.CreateAndPatchAll(typeof(PolyBridgeRotatorPlugin));
            Logger.LogInfo("It is all dead now, we have peace");
        }

        void Update()
        {
            if (Input.GetKeyDown(UnityEngine.KeyCode.LeftControl)){
                controlDown = true;
            }
            if (Input.GetKeyUp(UnityEngine.KeyCode.LeftControl)){
                controlDown = false;  
            }
            if (Input.GetKeyDown(UnityEngine.KeyCode.LeftShift)){
                shiftDown = true;
            }
            if (Input.GetKeyUp(UnityEngine.KeyCode.LeftShift)){
                shiftDown = false;  
            }
        }

        [HarmonyPatch(typeof(ClipboardManager), "MaybeRotate")]
        [HarmonyPostfix]
        private static void hotkeyRotate(){
            if(mEnabled.Value){
                if (_keybindMajorLeft.Value.IsDown())
                {
                    ClipboardManager.StartRotate(90f);
                }
                if (_keybindMajorRight.Value.IsDown())
                {
                    ClipboardManager.StartRotate(-90f);
                }
                if (_keybindMinorLeft.Value.IsDown())
                {
                    ClipboardManager.StartRotate(45f);
                }
                if (_keybindMinorRight.Value.IsDown())
                {
                    ClipboardManager.StartRotate(-45f);
                }
            }
        }
        [HarmonyPatch(typeof(GameStateCommonInput), "DoMouseScrollWheel")]
        [HarmonyPrefix]
        private static bool scroll(float delta, bool slow){
            scrollClicks = delta*20;
            if(mEnabled.Value){
                if((ClipboardManager.GetJoints().Count > 0 | ClipboardManager.GetEdges().Count > 0) && controlDown)
                {
                    if(shiftDown){
                        ClipboardManager.StartRotate(_controlShiftScale.Value*scrollClicks);
                    }else{
                        ClipboardManager.StartRotate(_controlScale.Value*scrollClicks);
                    }
                    return false;
                }else{
                    return true;
                }
            }else{
                return true;
            }
        }
    }
}
