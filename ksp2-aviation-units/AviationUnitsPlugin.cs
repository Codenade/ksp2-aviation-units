using BepInEx;
using HarmonyLib;
using KSP.Game;
using KSP.Messages;
using KSP.UI.Binding;
using KSP.UI.Binding.Core;
using System.Reflection;
using UnityEngine;

namespace Codenade.AviationUnits
{
    [BepInPlugin("1b1f2cc6-9f9b-4ad7-b70c-2dbedafce2e8", "ksp2-aviation-units", "0.1.1")]
	sealed class AviationUnitsPlugin : BaseUnityPlugin
	{
        public static readonly string NAME = typeof(AviationUnitsPlugin).GetCustomAttribute<BepInPlugin>().Name;
        public static readonly string GUID = typeof(AviationUnitsPlugin).GetCustomAttribute<BepInPlugin>().GUID;
        private static AviationUnitsPlugin _instance;

		void Awake()
		{
            _instance = this;
            enabled = false;
            Harmony.CreateAndPatchAll(typeof(AviationUnitsPlugin), GUID);
		}

        [HarmonyPatch(typeof(KSP.Game.StartupFlow.CreateMainMenuFlowAction), "DoAction")]
        [HarmonyPostfix]
        public static void Initialize() => _instance.enabled = true;

        void OnEnable()
		{
            if (GameManager.Instance is object)
			    GameManager.Instance.Game.Messages.PersistentSubscribe<VesselChangedMessage>(OnVesselChanged);
        }

		void OnDisable()
		{
            if (GameManager.Instance is object)
			    GameManager.Instance.Game.Messages.Unsubscribe<VesselChangedMessage>(OnVesselChanged);
        }

		void OnVesselChanged(MessageCenterMessage msg)
		{
            SetAviationUnits();
		}

        static T_Field GetPrivateFieldValue<T_Target, T_Field>(object obj, string fieldName)
        {
            return (T_Field)typeof(T_Target).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
        }

        static void SetPrivateFieldValue<T_Target>(object obj, string fieldName, object value)
        {
            typeof(T_Target).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, value);
        }

        public void SetAviationUnits()
        {
            // Set feet as altitude unit
            var altitudeLabel = GameObject.Find(@"GameManager/Default Game Instance(Clone)/UI Manager(Clone)/Scaled Main Canvas/FlightHudRoot(Clone)/group_navball(Clone)/Container/GRP-ALT/Container/DataContainer/Items/Value").GetComponent<UIValue_ReadNumber_TextUnits>();
            {
                var unitEntryArray = GetPrivateFieldValue<UIValue_ReadNumber_TextUnits, UIValue_ReadNumber_TextUnits.UnitEntry[]>(altitudeLabel, "unitEntryArray");
                unitEntryArray[0].unitMultiplier = 0.3208f;
                unitEntryArray[0].unitValue = "ft";
                unitEntryArray[0].numberTextFormat = "{0:0}";
                unitEntryArray[0].dontTruncateValue = false;
                unitEntryArray[0].switchThresholdMultiplier = 0f;
                SetPrivateFieldValue<UIValue_ReadNumber_TextUnits>(altitudeLabel, "unitEntryArray", unitEntryArray);
            }

            // Change altitude tape to feet too and be more sensitive
            var altitudeTape = GameObject.Find(@"GameManager/Default Game Instance(Clone)/UI Manager(Clone)/Scaled Main Canvas/FlightHudRoot(Clone)/group_navball(Clone)/Container/GRP-ALT/Container/DialBorder/DialMask").GetComponent<UIValue_ReadNumberValueTape>();
            SetPrivateFieldValue<UIValue_ReadNumberValueTape>(altitudeTape, "_offsetUnits", 3);
            SetPrivateFieldValue<UIValue_ReadNumberValueTape>(altitudeTape, "_valueInterval", 100f);
            SetPrivateFieldValue<UIValue_ReadNumberValueTape>(altitudeTape, "numberTextFormat", "{0:0}");
            {
                var unitEntryArray = GetPrivateFieldValue<UIValue_ReadNumberValueTape, UIValue_ReadNumber_TextUnits.UnitEntry[]>(altitudeTape, "unitEntryArray");
                unitEntryArray[0].unitMultiplier = 3.2808f;
                unitEntryArray[0].unitValue = "ft";
                unitEntryArray[0].dontTruncateValue = false;
                unitEntryArray[0].switchThresholdMultiplier = 0f;
                SetPrivateFieldValue<UIValue_ReadNumberValueTape>(altitudeTape, "unitEntryArray", unitEntryArray);
            }

            // Set knots as speed unit
            var velocityLabel = GameObject.Find(@"GameManager/Default Game Instance(Clone)/UI Manager(Clone)/Scaled Main Canvas/FlightHudRoot(Clone)/group_navball(Clone)/Container/GRP-VEL/Container/DataContainer/Items/Value").GetComponent<UIValue_ReadNumber_TextUnits>();
            {
                var unitEntryArray = GetPrivateFieldValue<UIValue_ReadNumber_TextUnits, UIValue_ReadNumber_TextUnits.UnitEntry[]>(velocityLabel, "unitEntryArray");
                unitEntryArray[0].unitMultiplier = 0.5144f;
                unitEntryArray[0].unitValue = "kt";
                unitEntryArray[0].dontTruncateValue = true;
                unitEntryArray[0].switchThresholdMultiplier = 1f;
                SetPrivateFieldValue<UIValue_ReadNumber_TextUnits>(velocityLabel, "unitEntryArray", unitEntryArray);
            }

            // Change speed tape to knots too and be more sensitive
            var velocityTape = GameObject.Find(@"GameManager/Default Game Instance(Clone)/UI Manager(Clone)/Scaled Main Canvas/FlightHudRoot(Clone)/group_navball(Clone)/Container/GRP-VEL/Container/DialBorder/DialMask").GetComponent<UIValue_ReadNumberValueTape>();
            SetPrivateFieldValue<UIValue_ReadNumberValueTape>(velocityTape, "_valueInterval", 5.1444f);
            SetPrivateFieldValue<UIValue_ReadNumberValueTape>(velocityTape, "_offsetUnits", 3);
            SetPrivateFieldValue<UIValue_ReadNumberValueTape>(velocityTape, "numberTextFormat", "{0:00}");
            {
                var unitEntryArray = GetPrivateFieldValue<UIValue_ReadNumberValueTape, UIValue_ReadNumber_TextUnits.UnitEntry[]>(velocityTape, "unitEntryArray");
                unitEntryArray[0].unitMultiplier = 0.5144f;
                unitEntryArray[0].unitValue = "kt";
                unitEntryArray[0].dontTruncateValue = false;
                unitEntryArray[0].switchThresholdMultiplier = 1000f;
                SetPrivateFieldValue<UIValue_ReadNumberValueTape>(velocityTape, "unitEntryArray", unitEntryArray);
            }

            // Make vertical speed indicator show feet/minute
            var arrowIndicator = GameObject.Find(@"GameManager/Default Game Instance(Clone)/UI Manager(Clone)/Scaled Main Canvas/FlightHudRoot(Clone)/widget_indicator_verticalspeed_horizontal_new(Clone)/Container/Bound UI - VertSpeed/velocity - arrow indicator/Binding - Arrow Position").GetComponent<UIValue_ReadNumber_Dial>();
            typeof(UIValueNumberBinder).GetProperty("NumberRange").GetSetMethod(true).Invoke(arrowIndicator, new object[] { new UIValueNumberRange(-5.08d, 5.08d) });
            SetPrivateFieldValue<UIValueNumberBinder>(arrowIndicator, "valueMappedMin", -5.08d);
            SetPrivateFieldValue<UIValueNumberBinder>(arrowIndicator, "valueMappedMax", 5.08d);
        }
    }
}