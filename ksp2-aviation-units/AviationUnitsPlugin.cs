using BepInEx;
using KSP.Game;
using KSP.Logging;
using KSP.Messages;
using KSP.UI.Binding;
using KSP.UI.Binding.Core;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using static KSP.UI.Binding.UIValue_ReadNumber_TextUnits;

namespace Codenade.AviationUnits
{
	[BepInPlugin("1b1f2cc6-9f9b-4ad7-b70c-2dbedafce2e8", "ksp2-aviation-units", "0.1.0")]
	sealed class AviationUnitsPlugin : BaseUnityPlugin
	{
        public static readonly string NAME = "ksp2-aviation-units";
        private static bool _initialized;

		void Awake()
		{
			gameObject.tag = "Game Manager";
            if (!_initialized)
                new Task(CreateAfterStartup).Start();
		}

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

        void OnDestroy()
        {
            GlobalLog.Log(LogFilter.UserMod, $"[{NAME}] Destroyed by {new StackTrace(0, true)}");
        }

        static async void CreateAfterStartup()
        {
            await Task.Delay(20000);
            _initialized = true;
            GlobalLog.Log(LogFilter.UserMod, $"[{NAME}] Created after startup");
            Type type = null;
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.IsDynamic)
                    continue;
                foreach (var t in asm.GetTypes())
                    if (t.Name == "RuntimeUnityEditor5")
                        type = t;
            }
            new GameObject(NAME, typeof(AviationUnitsPlugin), type);
        }

		void OnVesselChanged(MessageCenterMessage msg)
		{
            SetAviationUnits();
		}

        static T_Field GetPrivateField<T_Target, T_Field>(object obj, string fieldName)
        {
            return (T_Field)typeof(T_Target).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(obj);
        }

        static void SetPrivateField<T_Target>(object obj, string fieldName, object value)
        {
            typeof(T_Target).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(obj, value);
        }

        public void SetAviationUnits()
        {
            GlobalLog.Log(LogFilter.UserMod, $"[{NAME}] setting aviation units...");
            var altitudeLabel = GameObject.Find(@"GameManager/Default Game Instance(Clone)/UI Manager(Clone)/Scaled Main Canvas/FlightHudRoot(Clone)/group_navball(Clone)/Container/GRP-ALT/Container/DataContainer/Items/Value").GetComponent<UIValue_ReadNumber_TextUnits>();
            {
                var unitEntryArray = GetPrivateField<UIValue_ReadNumber_TextUnits, UnitEntry[]>(altitudeLabel, "unitEntryArray");
                unitEntryArray[0].unitMultiplier = 0.3208f;
                unitEntryArray[0].unitValue = "ft";
                unitEntryArray[0].numberTextFormat = "{0:0}";
                unitEntryArray[0].dontTruncateValue = false;
                unitEntryArray[0].switchThresholdMultiplier = 0f;
                SetPrivateField<UIValue_ReadNumber_TextUnits>(altitudeLabel, "unitEntryArray", unitEntryArray);
            }
            var altitudeTape = GameObject.Find(@"GameManager/Default Game Instance(Clone)/UI Manager(Clone)/Scaled Main Canvas/FlightHudRoot(Clone)/group_navball(Clone)/Container/GRP-ALT/Container/DialBorder/DialMask").GetComponent<UIValue_ReadNumberValueTape>();
            SetPrivateField<UIValue_ReadNumberValueTape>(altitudeTape, "_offsetUnits", 3);
            SetPrivateField<UIValue_ReadNumberValueTape>(altitudeTape, "_valueInterval", 100f);
            SetPrivateField<UIValue_ReadNumberValueTape>(altitudeTape, "numberTextFormat", "{0:0}");
            {
                var unitEntryArray = GetPrivateField<UIValue_ReadNumberValueTape, UnitEntry[]>(altitudeTape, "unitEntryArray");
                unitEntryArray[0].unitMultiplier = 3.2808f;
                unitEntryArray[0].unitValue = "ft";
                unitEntryArray[0].dontTruncateValue = false;
                unitEntryArray[0].switchThresholdMultiplier = 0f;
                SetPrivateField<UIValue_ReadNumberValueTape>(altitudeTape, "unitEntryArray", unitEntryArray);
            }
            var velocityLabel = GameObject.Find(@"GameManager/Default Game Instance(Clone)/UI Manager(Clone)/Scaled Main Canvas/FlightHudRoot(Clone)/group_navball(Clone)/Container/GRP-VEL/Container/DataContainer/Items/Value").GetComponent<UIValue_ReadNumber_TextUnits>();
            {
                var unitEntryArray = GetPrivateField<UIValue_ReadNumber_TextUnits, UnitEntry[]>(velocityLabel, "unitEntryArray");
                unitEntryArray[0].unitMultiplier = 0.5144f;
                unitEntryArray[0].unitValue = "kt";
                unitEntryArray[0].dontTruncateValue = true;
                unitEntryArray[0].switchThresholdMultiplier = 1f;
                SetPrivateField<UIValue_ReadNumber_TextUnits>(velocityLabel, "unitEntryArray", unitEntryArray);
            }
            var velocityTape = GameObject.Find(@"GameManager/Default Game Instance(Clone)/UI Manager(Clone)/Scaled Main Canvas/FlightHudRoot(Clone)/group_navball(Clone)/Container/GRP-VEL/Container/DialBorder/DialMask").GetComponent<UIValue_ReadNumberValueTape>();
            SetPrivateField<UIValue_ReadNumberValueTape>(velocityTape, "_valueInterval", 5.1444f);
            SetPrivateField<UIValue_ReadNumberValueTape>(velocityTape, "_offsetUnits", 3);
            SetPrivateField<UIValue_ReadNumberValueTape>(velocityTape, "numberTextFormat", "{0:00}");
            {
                var unitEntryArray = GetPrivateField<UIValue_ReadNumberValueTape, UnitEntry[]>(velocityTape, "unitEntryArray");
                unitEntryArray[0].unitMultiplier = 0.5144f;
                unitEntryArray[0].unitValue = "kt";
                unitEntryArray[0].dontTruncateValue = false;
                unitEntryArray[0].switchThresholdMultiplier = 1000f;
                SetPrivateField<UIValue_ReadNumberValueTape>(velocityTape, "unitEntryArray", unitEntryArray);
            }
            var arrowIndicator = GameObject.Find(@"GameManager/Default Game Instance(Clone)/UI Manager(Clone)/Scaled Main Canvas/FlightHudRoot(Clone)/widget_indicator_verticalspeed_horizontal_new(Clone)/Container/Bound UI - VertSpeed/velocity - arrow indicator/Binding - Arrow Position").GetComponent<UIValue_ReadNumber_Dial>();
            typeof(UIValueNumberBinder).GetProperty("NumberRange").GetSetMethod(true).Invoke(arrowIndicator, new object[] { new UIValueNumberRange(-5.08d, 5.08d) });
            SetPrivateField<UIValueNumberBinder>(arrowIndicator, "valueMappedMin", -5.08d);
            SetPrivateField<UIValueNumberBinder>(arrowIndicator, "valueMappedMax", 5.08d);
            GlobalLog.Log(LogFilter.UserMod, $"[{NAME}] setting aviation units finished");
        }
    }
}