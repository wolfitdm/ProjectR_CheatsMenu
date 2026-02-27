using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.Mono;
using HarmonyLib;
using SeqScripts;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ProjectR_CheatMenu
{
    [BepInPlugin("com.wolfitdm.ProjectR_CheatMenu", "ProjectR_CheatMenu Plugin", "1.0.0.0")]
    public class ProjectR_CheatMenu : BaseUnityPlugin
    {
        internal static new ManualLogSource Logger;

        private static ConfigEntry<bool> configEnableMe;
        private static ConfigEntry<bool> configMenuDefaultOpen;
        private static ConfigEntry<KeyCode> configKeyCodeToOpenCheatMenu;

        private bool showMenu = false; // Menü sichtbar?
        private Vector2 scrollPos;     // Scroll-Position
        private bool foldoutPlayer = true;
        private bool foldoutSlaves = true;
        private bool foldoutMonsters = true;
        private bool foldoutBuildings = true;

        // Beispielwerte für Cheats
        private int g_minX = -4;
        private int g_maxX = 3;
        private int g_minY = -4;
        private int g_maxY = 3;
        private int g_minSeat = 0;
        private int g_maxSeat = 0;

        private int x;
        private int y;
        private int seat;
        private int order;

        public ProjectR_CheatMenu()
        {
        }

        public static Type MyGetType(string originalClassName)
        {
            return Type.GetType(originalClassName + ",Assembly-CSharp");
        }

        public static Type MyGetTypeUnityEngine(string originalClassName)
        {
            return Type.GetType(originalClassName + ",UnityEngine");
        }

        private static string pluginKey = "General";

        public static bool enableMe = false;

        public static KeyCode keyCodeToOpenCloseTheCheatsMenu = 0;

        private void Update()
        {
            // Menü mit F1 ein-/ausblenden
            if (Input.GetKeyUp(keyCodeToOpenCloseTheCheatsMenu))
            {
                showMenu = !showMenu;
            }
        }

        private void OnGUI()
        {
            if (!enableMe) return;
            if (!showMenu) return;

            // Fenster zentriert anzeigen
            GUILayout.Window(0, new Rect(20, 20, 300, 400), DrawCheatWindow, "Cheat Menu");
        }

        private void initPosition(int minX = -4, int maxX = 3, int minY = -4, int maxY = 3, int minSeat = 0, int maxSeat = 1, bool isBuilding = false)
        {
            GUILayout.Label("Spawn / Create Position:");

            GUILayout.Space(5);

            GUILayout.Label($"x: / Current x: {x}");
            x = (int)GUILayout.HorizontalSlider((float)x, (float)minX, (float)maxX);

            GUILayout.Label($"y: / Current y: {y}");
            y = (int)GUILayout.HorizontalSlider((float)y, (float)minY, (float)maxY);

            order = -1;

            if (isBuilding)
                return;

            GUILayout.Label($"seat: / Current seat: {seat}");
            seat = (int)GUILayout.HorizontalSlider((float)seat, (float)minSeat, (float)maxSeat);
        }

        private void DrawCheatWindow(int windowID)
        {
            scrollPos = GUILayout.BeginScrollView(scrollPos);

            if (gm == null)
            {
                return;
            }

            // Spieler-Cheats
            foldoutPlayer = EditorLikeFoldout(foldoutPlayer, "Player Cheats");
            if (foldoutPlayer)
            {
                if (GUILayout.Button("+10 Energy"))
                {
                    gm.Pd.Energy += 10;
                }
            }

            GUILayout.Space(10);

            foldoutSlaves = EditorLikeFoldout(foldoutSlaves, "Slaves");

            if (foldoutSlaves)
            {
                if (GUILayout.Button("Spawn Human"))
                {   
                    gm.NewCharacter("Human", "Base", "Room", x, y, seat, order);
                }

                if (GUILayout.Button("Spawn Elf1"))
                {
                    gm.NewCharacter("Elf1", "Base", "Room", x, y, seat, order);
                }

                if (GUILayout.Button("Spawn Knight"))
                {
                    gm.NewCharacter("Knight", "Base", "Room", x, y, seat, order);
                }

                if (GUILayout.Button("Spawn Elf"))
                {
                    gm.NewCharacter("Elf", "Base", "Room", x, y, seat, order);
                }

                if (GUILayout.Button("Spawn Sister"))
                {
                    gm.NewCharacter("Sister", "Base", "Room", x, y, seat, order);
                }

                if (GUILayout.Button("Spawn Man"))
                {
                    gm.NewCharacter("Man", "Base", "Room", x, y, seat, order);
                }

                if (GUILayout.Button("Spawn Oni"))
                {
                    gm.NewCharacter("Oni", "Base", "Room", x, y, seat, order);
                }

                if (GUILayout.Button("Spawn Player"))
                {
                    gm.NewCharacter("Player1", "Base", "Room", x, y, seat, order);
                }

                if (GUILayout.Button("Spawn Shield"))
                {
                    gm.NewCharacter("Shield", "Base", "Room", x, y, seat, order);
                }

                if (GUILayout.Button("Spawn Shortstack"))
                {
                    gm.NewCharacter("Shortstack", "Base", "Room", x, y, seat, order);
                }

                initPosition();
            }

            GUILayout.Space(10);

            foldoutMonsters = EditorLikeFoldout(foldoutMonsters, "Monsters");

            if (foldoutMonsters)
            {
                if (GUILayout.Button("Spawn Horse"))
                {
                    gm.NewCharacter("Horse", "Base", "Room", x, y, seat, order);
                }

                if (GUILayout.Button("Spawn GoblinBow"))
                {
                    gm.NewCharacter("GoblinBow", "Base", "Room", x, y, seat, order);
                }

                if (GUILayout.Button("Spawn Orc"))
                {
                    gm.NewCharacter("Orc", "Base", "Room", x, y, seat, order);
                }

                if (GUILayout.Button("Spawn Goblin"))
                {
                    gm.NewCharacter("Goblin", "Base", "Room", x, y, seat, order);
                }

                if (GUILayout.Button("Spawn GoblinPillory"))
                {
                    gm.NewCharacter("GoblinPillory", "Base", "Room", x, y, seat, order);
                }

                initPosition();
            }

            GUILayout.Space(10);

            foldoutBuildings = EditorLikeFoldout(foldoutBuildings, "Buildings");

            if (foldoutBuildings)
            {
                if (GUILayout.Button("Create BreedingRoom"))
                {
                    Building building2 = gm.NewBuilding("BreedingRoom", "Base", "Room", x, y);

                    x = building2.Index0;
                    y = building2.Index1;
                }

                if (GUILayout.Button("Create LactationRoom"))
                {
                    Building building2 = gm.NewBuilding("LactationRoom", "Base", "Room", x, y);
                    
                    x = building2.Index0;
                    y = building2.Index1;
                }

                if (GUILayout.Button("Create BedRoom"))
                {
                    Building building2 = gm.NewBuilding("Room1", "Base", "Room", x, y);

                    x = building2.Index0;
                    y = building2.Index1;
                }

                initPosition(g_minX -3, g_maxX + 3, g_minY - 3, g_maxY + 3, g_minSeat, g_maxSeat, true);
            }
            
            GUILayout.EndScrollView();

            // Fenster verschiebbar machen
            GUI.DragWindow(new Rect(0, 0, 10000, 20));
        }

        /// <summary>
        /// Einfacher Foldout-Style wie im Unity-Editor.
        /// </summary>
        private bool EditorLikeFoldout(bool foldout, string title)
        {
            GUILayout.BeginHorizontal();
            string arrow = foldout ? "▼" : "▶";
            if (GUILayout.Button(arrow + " " + title, GUI.skin.label))
            {
                foldout = !foldout;
            }
            GUILayout.EndHorizontal();
            return foldout;
        }

        private void Awake()
        {
            // Plugin startup logic
            Logger = base.Logger;

            configEnableMe = Config.Bind(pluginKey,
                                              "EnableMe",
                                              true,
                                             "Whether or not you want enable this mod (default true also yes, you want it, and false = no)");


            configMenuDefaultOpen = Config.Bind(pluginKey,
                                  "IsMenuDefaultOpened",
                                  true,
                                 "Is menu default opened, default true");

            configKeyCodeToOpenCheatMenu = Config.Bind(pluginKey,
                                             "KeyCodeToOpenCloseTheCheatsMenu",
                                             KeyCode.R,
                                            "KeyCode to open/close the cheats menu, default R");

            enableMe = configEnableMe.Value;
            showMenu = configMenuDefaultOpen.Value;
            keyCodeToOpenCloseTheCheatsMenu = configKeyCodeToOpenCheatMenu.Value;

            PatchAllHarmonyMethods();

            Logger.LogInfo($"Plugin ProjectR_CheatMenu BepInEx is loaded!");
        }
		
		public static void PatchAllHarmonyMethods()
        {
			if (!enableMe)
            {
                return;
            }
			
            try
            {
                PatchHarmonyMethodUnity(typeof(GameManager), "OnEnable", "GameManager_OnEnable", false, true);
            } catch (Exception ex)
            {
                Logger.LogError(ex.ToString());
            }
        }

        private static GameManager gm = Rf.Gm;
        public static void GameManager_OnEnable(object __instance)
        {
            if (gm != null)
            {
                return;
            }

            gm = (GameManager) __instance;
        }
        public static void PatchHarmonyMethodUnity(Type originalClass, string originalMethodName, string patchedMethodName, bool usePrefix, bool usePostfix, Type[] parameters = null)
        {
            string uniqueId = "com.wolfitdm.ProjectR_CheatMenu";
            Type uniqueType = typeof(ProjectR_CheatMenu);

            // Create a new Harmony instance with a unique ID
            var harmony = new Harmony(uniqueId);

            if (originalClass == null)
            {
                Logger.LogInfo($"GetType originalClass == null");
                return;
            }

            MethodInfo patched = null;

            try
            {
                patched = AccessTools.Method(uniqueType, patchedMethodName);
            }
            catch (Exception ex)
            {
                patched = null;
            }

            if (patched == null)
            {
                Logger.LogInfo($"AccessTool.Method patched {patchedMethodName} == null");
                return;

            }

            // Or apply patches manually
            MethodInfo original = null;

            try
            {
                if (parameters == null)
                {
                    original = AccessTools.Method(originalClass, originalMethodName);
                }
                else
                {
                    original = AccessTools.Method(originalClass, originalMethodName, parameters);
                }
            }
            catch (AmbiguousMatchException ex)
            {
                Type[] nullParameters = new Type[] { };
                try
                {
                    if (patched == null)
                    {
                        parameters = nullParameters;
                    }

                    ParameterInfo[] parameterInfos = patched.GetParameters();

                    if (parameterInfos == null || parameterInfos.Length == 0)
                    {
                        parameters = nullParameters;
                    }

                    List<Type> parametersN = new List<Type>();

                    for (int i = 0; i < parameterInfos.Length; i++)
                    {
                        ParameterInfo parameterInfo = parameterInfos[i];

                        if (parameterInfo == null)
                        {
                            continue;
                        }

                        if (parameterInfo.Name == null)
                        {
                            continue;
                        }

                        if (parameterInfo.Name.StartsWith("__"))
                        {
                            continue;
                        }

                        Type type = parameterInfos[i].ParameterType;

                        if (type == null)
                        {
                            continue;
                        }

                        parametersN.Add(type);
                    }

                    parameters = parametersN.ToArray();
                }
                catch (Exception ex2)
                {
                    parameters = nullParameters;
                }

                try
                {
                    original = AccessTools.Method(originalClass, originalMethodName, parameters);
                }
                catch (Exception ex2)
                {
                    original = null;
                }
            }
            catch (Exception ex)
            {
                original = null;
            }

            if (original == null)
            {
                Logger.LogInfo($"AccessTool.Method original {originalMethodName} == null");
                return;
            }

            HarmonyMethod patchedMethod = new HarmonyMethod(patched);
            var prefixMethod = usePrefix ? patchedMethod : null;
            var postfixMethod = usePostfix ? patchedMethod : null;

            harmony.Patch(original,
                prefix: prefixMethod,
                postfix: postfixMethod);
        }
    }
}