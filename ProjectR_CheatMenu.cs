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

        private bool foldoutSlaveCheats = true;
        private bool foldoutMonsterCheats = true;

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
            GUILayout.Window(0, new Rect(20, 20, 350, 400), DrawCheatWindow, "Cheat Menu");
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

        private void UpdateCharacterId(string id, int health = 0, int defense = 0, int attackspeed = 0, int movementspeed = 0, int damage = 0, int critChance = 0, int critMultiplier = 0, int knockout = 0, int growth = 0, int cumshotTime = 0)
        {
            SeqList<Character> characterSeqList = PlayerDataManager.Instance.GetCharacterSeqList();

            for (int index = 0; index < characterSeqList.Count; ++index) 
            {
                Character character = characterSeqList[index];

                if (character.DataId != id) {
                    continue;
                }

                character.Health_Additional += health;
                character.Defense_Additional += defense;
                character.AttackSpeed_Additional += attackspeed;
                character.MovementSpeed_Additional += movementspeed;
                character.Damage_Additional += damage;
                character.CriticalHitChance_Additional += critChance;
                character.CriticalHitMultiplier_Additional += critMultiplier;
                character.MaxKnockbackStability_Additional += knockout;
                character.BabyGrowthTime_Additional -= growth;
                character.CumshotTime_Additional -= cumshotTime;

                CharacterManager.Instance.RefreshStatus(character);

                if (character.CumshotTime <= 0)
                {
                    character.CumshotTime = character.CumshotTime_Additional = 0;
                }

                if (character.BabyGrowthTime <= 0)
                {
                    character.BabyGrowthTime = character.BabyGrowthTime_Additional = 0;
                }

                CharacterManager.Instance.RefreshStatus(character);
            }
        }
        private void UpdateAllCharacters(int health = 0, int defense = 0, int attackspeed = 0, int movementspeed = 0, int damage = 0, int critChance = 0, int critMultiplier = 0, int knockout = 0, int growth = 0, int cumshotTime = 0)
        {
            SeqList<Character> characterSeqList = PlayerDataManager.Instance.GetCharacterSeqList();

            for (int index = 0; index < characterSeqList.Count; ++index)
            {
                Character character = characterSeqList[index];

                character.Health_Additional += health;
                character.Defense_Additional += defense;
                character.AttackSpeed_Additional += attackspeed;
                character.MovementSpeed_Additional += movementspeed;
                character.Damage_Additional += damage;
                character.CriticalHitChance_Additional += critChance;
                character.CriticalHitMultiplier_Additional += critMultiplier;
                character.MaxKnockbackStability_Additional += knockout;
                character.BabyGrowthTime_Additional -= growth;
                character.CumshotTime_Additional -= cumshotTime;

                CharacterManager.Instance.RefreshStatus(character);

                if (character.CumshotTime <= 0)
                {
                    character.CumshotTime = character.CumshotTime_Additional = 0;
                }

                if (character.BabyGrowthTime <= 0)
                {
                    character.BabyGrowthTime = character.BabyGrowthTime_Additional = 0;
                }

                CharacterManager.Instance.RefreshStatus(character);
            }
        }

        private void createLayoutButtonsForCharacterId(string id)
        {
            string buttonId = id;

            if (id == "Player1")
            {
                buttonId = "Player";
            }
            if (GUILayout.Button($"+10 Health To All {buttonId}s"))
            {
                UpdateCharacterId(id, health: 10);
            }
            if (GUILayout.Button($"+10 Defense To All {buttonId}s"))
            {
                UpdateCharacterId(id, defense: 10);
            }
            if (GUILayout.Button($"+10 Damage To All {buttonId}s"))
            {
                UpdateCharacterId(id, damage: 10);
            }
            if (GUILayout.Button($"+10 Attack Speed To All {buttonId}s"))
            {
                UpdateCharacterId(id, attackspeed: 10);
            }
            if (GUILayout.Button($"+10 Movement Speed To All {buttonId}s"))
            {
                UpdateCharacterId(id, movementspeed: 10);
            }
            if (GUILayout.Button($"+10 CritChance To All {buttonId}s"))
            {
                UpdateCharacterId(id, critChance: 10);
            }
            if (GUILayout.Button($"+10 CritMultiplier To All {buttonId}s"))
            {
                UpdateCharacterId(id, critMultiplier: 10);
            }
            if (GUILayout.Button($"+10 Knockout Stability To All {buttonId}s"))
            {
                UpdateCharacterId(id, knockout: 10);
            }
            if (GUILayout.Button($"+10 BabyGrowth Time To All {buttonId}s"))
            {
                UpdateCharacterId(id, growth: 10);
            }
            if (GUILayout.Button($"+10 CumShot Time To All {buttonId}s"))
            {
                UpdateCharacterId(id, cumshotTime: 10);
            }
            if (GUILayout.Button($"+10 All To All {buttonId}s"))
            {
                UpdateCharacterId(id, health: 10, defense: 10, damage: 10, attackspeed: 10, movementspeed: 10, critChance: 10, critMultiplier: 10, knockout: 10, cumshotTime: 10, growth: 10);
            }
        }

        private void createLayoutButtonsForCharacterIdAll(string allId, string[] all)
        {
            string id = "";
            string buttonId = allId;

            if (GUILayout.Button($"+10 Health To All {buttonId}s"))
            {
                for (int i = 0; i < all.Length; i++)
                {
                    id = all[i];
                    UpdateCharacterId(id, health: 10);
                }
            }
            if (GUILayout.Button($"+10 Defense To All {buttonId}s"))
            {
                for (int i = 0; i < all.Length; i++)
                {
                    id = all[i];
                    UpdateCharacterId(id, defense: 10);
                }
            }
            if (GUILayout.Button($"+10 Attack Speed To All {buttonId}s"))
            {
                for (int i = 0; i < all.Length; i++)
                {
                    id = all[i];
                    UpdateCharacterId(id, attackspeed: 10);
                }
            }
            if (GUILayout.Button($"+10 Movement Speed To All {buttonId}s"))
            {
                for (int i = 0; i < all.Length; i++)
                {
                    id = all[i];
                    UpdateCharacterId(id, movementspeed: 10);
                }
            }
            if (GUILayout.Button($"+10 Damage To All {buttonId}s"))
            {
                for (int i = 0; i < all.Length; i++)
                {
                    id = all[i];
                    UpdateCharacterId(id, damage: 10);
                }
            }
            if (GUILayout.Button($"+10 CritChance To All {buttonId}s"))
            {
                for (int i = 0; i < all.Length; i++)
                {
                    id = all[i];
                    UpdateCharacterId(id, critChance: 10);
                }
            }
            if (GUILayout.Button($"+10 CritMultiplier To All {buttonId}s"))
            {
                for (int i = 0; i < all.Length; i++)
                {
                    id = all[i];
                    UpdateCharacterId(id, critMultiplier: 10);
                }
            }
            if (GUILayout.Button($"+10 Knockout Stability To All {buttonId}s"))
            {
                for (int i = 0; i < all.Length; i++)
                {
                    id = all[i];
                    UpdateCharacterId(id, knockout: 10);
                }
            }
            if (GUILayout.Button($"+10 BabyGrowth Time To All {buttonId}s"))
            {
                for (int i = 0; i < all.Length; i++)
                {
                    id = all[i];
                    UpdateCharacterId(id, growth: 10);
                }
            }
            if (GUILayout.Button($"+10 CumShot Time To All {buttonId}s"))
            {
                for (int i = 0; i < all.Length; i++)
                {
                    id = all[i];
                    UpdateCharacterId(id, cumshotTime: 10);
                }
            }
            if (GUILayout.Button($"+10 All To All {buttonId}s"))
            {
                for (int i = 0; i < all.Length; i++)
                {
                    id = all[i];
                    UpdateCharacterId(id, health: 10, defense: 10, damage: 10, attackspeed: 10, movementspeed: 10, critChance: 10, critMultiplier: 10, knockout: 10, cumshotTime: 10, growth: 10);
                }
            }
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
                if (GUILayout.Button("+100 Energy"))
                {
                    gm.Pd.Energy += 100;
                }
                if (GUILayout.Button("+1000 Energy"))
                {
                    gm.Pd.Energy += 1000;
                }
                createLayoutButtonsForCharacterId("Player1");
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
                    if (x == 0 && y == 0)
                    {
                        x = -2;
                        y = 1;
                    }

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

            GUILayout.Space(10);

            foldoutSlaveCheats = EditorLikeFoldout(foldoutSlaveCheats, "Slave Cheats");
            
            if (foldoutSlaveCheats)
            {
                string[] slaves = new string[] { "Human", "Elf1", "Knight", "Elf", "Sister", "Man", "Oni", "Shield", "Shortstack"};

                Dictionary<string, bool> foldouts = new Dictionary<string, bool>();

                for (int i = 0; i < slaves.Length; i++)
                {
                    string slave = slaves[i];
                    if (!foldouts.ContainsKey(slave))
                    {
                        foldouts.Add(slave, true);
                    }

                    foldouts[slave] = EditorLikeFoldoutEx(foldouts[slave], slave);

                    if (foldouts[slave])
                    {
                        createLayoutButtonsForCharacterId(slave);
                    }
                }

                string allId = "Slaves";

                if (!foldouts.ContainsKey(allId))
                {
                    foldouts.Add(allId, true);
                }

                foldouts[allId] = EditorLikeFoldoutEx(foldouts[allId], allId);

                if (foldouts[allId])
                {
                    createLayoutButtonsForCharacterIdAll(allId, slaves);
                }
            }

            GUILayout.Space(10);

            foldoutMonsterCheats = EditorLikeFoldout(foldoutMonsterCheats, "Monster Cheats");

            if (foldoutMonsterCheats)
            {
                string[] monsters = new string[] { "Horse", "GoblinBow", "Orc", "Goblin", "GoblinPillory" };

                Dictionary<string, bool> foldouts = new Dictionary<string, bool>();

                for (int i = 0; i < monsters.Length; i++)
                {
                    string slave = monsters[i];
                    if (!foldouts.ContainsKey(slave))
                    {
                        foldouts.Add(slave, true);
                    }

                    foldouts[slave] = EditorLikeFoldoutEx(foldouts[slave], slave);

                    if (foldouts[slave])
                    {
                        createLayoutButtonsForCharacterId(slave);
                    }
                }


                string allId = "Monsters";

                if (!foldouts.ContainsKey(allId))
                {
                    foldouts.Add(allId, true);
                }

                foldouts[allId] = EditorLikeFoldoutEx(foldouts[allId], allId);

                if (foldouts[allId])
                {
                    createLayoutButtonsForCharacterIdAll(allId, monsters);
                }
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

        private bool EditorLikeFoldoutEx(bool foldout, string title)
        {
            GUILayout.BeginHorizontal();
            string arrow = foldout ? "_▼" : "_▶";
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