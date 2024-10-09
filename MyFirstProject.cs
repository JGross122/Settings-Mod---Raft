using UnityEngine;
using Steamworks;
using HarmonyLib;

public class MyFirstProject : Mod
{
    static MyFirstProject myClass;
    int multiplier = 1;

    public static bool ExtraSettingsAPI_GetCheckboxState(string SettingName) => false;
    public static string ExtraSettingsAPI_GetInputValue(string SettingName) => "";
    public static void ExtraSettingsAPI_SetCheckboxState(string SettingName, bool value) { }
    public static void ExtraSettingsAPI_SetInputValue(string SettingName, string value) { }
    public static string[] ExtraSettingsAPI_GetComboboxContent(string SettingName) => new string[0];
    public static int ExtraSettingsAPI_GetComboboxSelectedIndex(string SettingName) => -1;
    public static void ExtraSettingsAPI_SetComboboxSelectedIndex(string SettingName, int value) { }

    [ConsoleCommand(name: "fishingMultiplier", docs: "changes the multiplier for fishing")]
    public static void fishingMultiplierMethod(string[] arg)
    {
        int x = int.Parse(arg[0]);
        myClass.multiplier = x;
    }

    bool areBaitsInfinite = false;
    [ConsoleCommand(name: "infinitebaits", docs: "1: infinite baits, 0: finite baits")]
    public static void infiniteBaits(string[] arg)
    {
        int x = int.Parse(arg[0]);
        if(x==0) { myClass.areBaitsInfinite = false; }
        if(x==1) { myClass.areBaitsInfinite = true; }
        ExtraSettingsAPI_SetCheckboxState("infiniteBaits", myClass.areBaitsInfinite);
    }

    bool isRodUnbreakable = false;
    [ConsoleCommand(name: "unbreakablerod", docs: "1: unbreakable rod, 2: breakable rod")]
    public static void unbreakableRod(string[] arg)
    {
        int x = int.Parse(arg[0]);
        if(x==0) { myClass.isRodUnbreakable = false; };
        if(x==1) { myClass.isRodUnbreakable = true; };
        ExtraSettingsAPI_SetCheckboxState("unbreakableRod", myClass.isRodUnbreakable);
    }

    int oxygenBarOption = 0;
    [ConsoleCommand(name: "infiniteoxygen", docs: "0: finite oxygen, 1: infinite oxygen, 2: infinite oxygen + remove oxygen bar")]
    public static void infiniteOxygen(string[] arg)
    {
        int x = int.Parse(arg[0]);
        if(x!=0 && x!=1 && x!=2)
        {
            x = 0;
        }
        myClass.oxygenBarOption = x;
        ExtraSettingsAPI_SetComboboxSelectedIndex("infiniteOxygen", myClass.oxygenBarOption);
    }

    [HarmonyPatch(typeof(Stat_Oxygen), "Update")]
    public class oxygenPatch
    {
        [HarmonyPostfix]
        static void oxygen(Stat_Oxygen __instance)
        {
            if(myClass.oxygenBarOption == 1)
            {
                __instance.Value = __instance.Max - 0.01f;
            }
            if(myClass.oxygenBarOption == 2)
            {
                __instance.Value = __instance.Max;
            }
        }
    }

    int sprinklerOption = 0;
    [ConsoleCommand(name: "sprinklercheat", docs: "0: sprinkler requires water and electricty, 1: requires nothing, 2: requires water, 3: requires electricity")]
    public static void sprinklerCheat(string[] arg)
    {
        int x = int.Parse(arg[0]);
        if(x!=0 && x!=1 && x!=2 && x != 3)
        {
            x = 0;
        }
        myClass.sprinklerOption = x;
        ExtraSettingsAPI_SetComboboxSelectedIndex("sprinklerCheat", myClass.sprinklerOption);
    }

    [HarmonyPatch(typeof(Sprinkler), "CanWater", MethodType.Getter)]
    public class sprinklerPatch
    {
        static bool Prefix(Sprinkler __instance, ref bool __result)
        {

            switch(myClass.sprinklerOption)
            {
                case 0:
                    return true;
                    break;
                case 1:
                    __result = true;
                    break;
                case 2:
                    __result = __instance.waterTank.CurrentTankAmount > 0f;
                    break;
                case 3:
                    __result =__instance.battery.CanGiveElectricity;
                    break;
            }
            
            return false;
        }
    }

    int weakSharkOption = 0;
    [ConsoleCommand(name: "weakshark", docs: "1: shark doesnt do any damage to a player, 0: shark does damage to a player")]
    public static void weakShark(string[] arg)
    {
        int x = int.Parse(arg[0]);
        if(x!=0 && x!=1)
        {
            x = 0;
        }

        if (x==1) { myClass.weakSharkOption = 0; }
        if (x==0) { myClass.weakSharkOption = 1; }

        ExtraSettingsAPI_SetComboboxSelectedIndex("weakShark", myClass.weakSharkOption);

        GameModeValueManager.GetCurrentGameModeValue().sharkVariables.damageMultiplier = myClass.weakSharkOption;
    }

    [ConsoleCommand(name: "setsprintspeed", docs: "sets player's sprint speed, default - 5")]
    public static void setsprintSpeed(string[] arg)
    {
        RAPI.GetLocalPlayer().PersonController.sprintSpeed = float.Parse(arg[0]);
        ExtraSettingsAPI_SetInputValue("sprintSpeed", (float.Parse(arg[0])).ToString());
    }

    [ConsoleCommand(name: "setswimspeed", docs: "sets player's swim speed, default - 3")]
    public static void setSwimSpeed(string[] arg)
    {
        RAPI.GetLocalPlayer().PersonController.swimSpeed = float.Parse(arg[0]);
        ExtraSettingsAPI_SetInputValue("swimSpeed", (float.Parse(arg[0])).ToString());
    }
    
    [ConsoleCommand(name: "setspeed", docs: "set player's speed, default - 1")]
    public static void setSpeed(string[] arg)
    {
        RAPI.GetLocalPlayer().PersonController.normalSpeed = float.Parse(arg[0]);
        ExtraSettingsAPI_SetInputValue("normalSpeed", (float.Parse(arg[0])).ToString());
    }

    [ConsoleCommand(name: "setgravity", docs: "set player's gravity, default - 20")]
    public static void setGravity(string[] arg)
    {
        RAPI.GetLocalPlayer().PersonController.gravity = float.Parse(arg[0]);
        ExtraSettingsAPI_SetInputValue("gravity", (float.Parse(arg[0])).ToString());
    }

    [ConsoleCommand(name: "setjumpspeed", docs: "set player's jump speed, default - 7")]
    public static void setJumpSpeed(string[] arg)
    {
        RAPI.GetLocalPlayer().PersonController.jumpSpeed = float.Parse(arg[0]);
    }

    bool disableHungerOption = false;
    [ConsoleCommand(name: "disablehunger", docs: "1: disable hunger, 0: enable hunger")]
    public static void disableHunger(string[] arg)
    {
        NourishmentVariables nv = GameModeValueManager.GetCurrentGameModeValue().nourishmentVariables;
        int x = int.Parse(arg[0]);
        if(x!=0 && x!=1)
        {
            x = 0;
        }
        
        if(x==0) { myClass.disableHungerOption = false; }
        if(x==1) { myClass.disableHungerOption = true; }

        ExtraSettingsAPI_SetCheckboxState("disableHunger", myClass.disableHungerOption);

        if(myClass.disableHungerOption == true)
        {
            nv.foodDecrementRateMultiplier = 0f;
        }
        if(myClass.disableHungerOption == false)
        {
            nv.foodDecrementRateMultiplier = 1f;
        }
    }

    bool disableThirstOption = false;
    [ConsoleCommand(name: "disablethirst", docs: "1: disable thirst, 0: enable thirst")]
    public static void disableThirst(string[] arg)
    {
        NourishmentVariables nv = GameModeValueManager.GetCurrentGameModeValue().nourishmentVariables;
        int x = int.Parse(arg[0]);
        if (x != 0 && x != 1)
        {
            x = 0;
        }
        
        if(x==0) { myClass.disableThirstOption = false; }
        if(x==1) { myClass.disableThirstOption = true; }

        ExtraSettingsAPI_SetCheckboxState("disableThirst", myClass.disableHungerOption);

        if (myClass.disableThirstOption == true)
        {
            nv.thirstDecrementRateMultiplier = 0f;
        }
        if (myClass.disableThirstOption == false)
        {
            nv.thirstDecrementRateMultiplier = 1f;
        }
    }

  

    [HarmonyPatch(typeof(FishingRod), "PullItemsFromSea")]
    public class fishingRodPatch
    {
        [HarmonyPrefix]
        static bool pullItems(FishingRod __instance)
        {
            bool isMetalRod = Traverse.Create(__instance).Field("isMetalRod").GetValue<bool>();
            Rope rope = Traverse.Create(__instance).Field("rope").GetValue<Rope>();
            Network_Player playerNetwork = Traverse.Create(__instance).Field("playerNetwork").GetValue<Network_Player>();
            PlayerAnimator playerAnimator = Traverse.Create(__instance).Field("playerAnimator").GetValue<PlayerAnimator>();
            Throwable throwable = Traverse.Create(__instance).Field("throwable").GetValue<Throwable>();
            FishingBaitHandler fishingBaitHandler = Traverse.Create(__instance).Field("fishingBaitHandler").GetValue<FishingBaitHandler>();
            

            Item_Base randomItemFromCurrentBaitPool = fishingBaitHandler.GetRandomItemFromCurrentBaitPool(isMetalRod);
            randomItemFromCurrentBaitPool = fishingBaitHandler.GetRandomItemFromCurrentBaitPool(isMetalRod);


            if (randomItemFromCurrentBaitPool != null)
            {
                playerNetwork.Inventory.AddItem(randomItemFromCurrentBaitPool.UniqueName, myClass.multiplier);
                if (myClass.areBaitsInfinite != true)
                {
                    fishingBaitHandler.ConsumeBait();
                }
            }
            playerAnimator.SetAnimation(PlayerAnimation.Trigger_FishingRetract, false);
            __instance.rodAnimator.SetTrigger("FishingRetract");
            ParticleManager.PlaySystem("WaterSplash_Hook", throwable.throwableObject.position + Vector3.up * 0.1f, true);
            Traverse.Create(__instance).Method("ResetRod").GetValue();

            if(myClass.isRodUnbreakable == false)
            {
                if (playerNetwork.Inventory.RemoveDurabillityFromHotSlot(1))
                {
                    rope.gameObject.SetActive(false);
                }
            }
            else
            {
                rope.gameObject.SetActive(false);
            }

            return false;
        }
    }

    Harmony harmonyInstance;
    public void Start()
    {
        myClass = this;
        harmonyInstance = new Harmony("com.Wydra.doorPatch");
        harmonyInstance.PatchAll();
    }

    public void OnModUnload()
    {
        harmonyInstance.UnpatchAll(harmonyInstance.Id);
        Debug.Log("Mod MyFirstProject has been unloaded!");
    }


    bool safeRaftOption = false;
    [ConsoleCommand(name: "saferaft", docs: "1: shark won't attack your raft, 0: will attack")]
    public static void safeRaft(string[] arg)
    {
        int x = int.Parse(arg[0]);
        if(x!=0 && x != 1)
        {
            x = 0;
        }
        if(x==0) { myClass.safeRaftOption = false; }
        if(x==1) { myClass.safeRaftOption = true; }

        ExtraSettingsAPI_SetCheckboxState("safeRaft", myClass.safeRaftOption);
    }

    [HarmonyPatch(typeof(AI_State_Attack_Block_Shark), "FindBlockToAttack")]
    public static class raftAttackPatch
    {
        private static void Postfix(ref Block __result)
        {
            if (myClass.safeRaftOption == true)
            {
                __result = null;
            }
        }
    }

    float customPullSpeed = 3f;
    [ConsoleCommand(name: "sethookpullspeed", docs: "set hook's pulling speed, default - 3")]
    public static void getHookObjects(string[] arg)
    {
        myClass.customPullSpeed = float.Parse(arg[0]);
        ExtraSettingsAPI_SetInputValue("pullSpeed", (float.Parse(arg[0])).ToString());
    }

    [HarmonyPatch(typeof(Hook), "OnSelect")]
    public class hookAwakePatch
    {
        private static void Postfix(Hook __instance)
        {
            __instance.pullSpeed = myClass.customPullSpeed;
        }
    }

    [HarmonyPatch(typeof(Hook), "OnDeSelect")]
    public class hookAwakePatch2
    {
        private static void Postfix(Hook __instance)
        {
            __instance.pullSpeed = 3f;
        }
    }

    /// ExtraSettings API section
    /// 

    public void ExtraSettingsAPI_SettingsOpen()
    {
        // multiplier
        if (!string.IsNullOrWhiteSpace(ExtraSettingsAPI_GetInputValue("lootMultiplier")))
        {
            myClass.multiplier = int.Parse(ExtraSettingsAPI_GetInputValue("lootMultiplier"));
        }
        // infinite baits
        myClass.areBaitsInfinite = ExtraSettingsAPI_GetCheckboxState("infiniteBaits");
        // unbreakablerod
        myClass.isRodUnbreakable = ExtraSettingsAPI_GetCheckboxState("unbreakableRod");
        // infiniteoxygen
        myClass.oxygenBarOption = ExtraSettingsAPI_GetComboboxSelectedIndex("infiniteOxygen");
        // sprinklercheat
        myClass.sprinklerOption = ExtraSettingsAPI_GetComboboxSelectedIndex("sprinklerCheat");
        // weakshark
        myClass.weakSharkOption = ExtraSettingsAPI_GetComboboxSelectedIndex("weakShark");
        GameModeValueManager.GetCurrentGameModeValue().sharkVariables.damageMultiplier = myClass.weakSharkOption;
        // sprintspeed
        if (!string.IsNullOrWhiteSpace(ExtraSettingsAPI_GetInputValue("sprintSpeed")))
        {
            RAPI.GetLocalPlayer().PersonController.sprintSpeed = float.Parse(ExtraSettingsAPI_GetInputValue("sprintSpeed"));
        }
        // swimspeed
        if (!string.IsNullOrWhiteSpace(ExtraSettingsAPI_GetInputValue("swimSpeed")))
        {
            RAPI.GetLocalPlayer().PersonController.swimSpeed = float.Parse(ExtraSettingsAPI_GetInputValue("swimSpeed"));
        }
        // speed
        if (!string.IsNullOrWhiteSpace(ExtraSettingsAPI_GetInputValue("normalSpeed")))
        {
            RAPI.GetLocalPlayer().PersonController.normalSpeed = float.Parse(ExtraSettingsAPI_GetInputValue("normalSpeed"));
        }
        // gravity
        if (!string.IsNullOrWhiteSpace(ExtraSettingsAPI_GetInputValue("gravity")))
        {
            RAPI.GetLocalPlayer().PersonController.gravity = float.Parse(ExtraSettingsAPI_GetInputValue("gravity"));
        }
        // jumpspeed
        if (!string.IsNullOrWhiteSpace(ExtraSettingsAPI_GetInputValue("jumpSpeed")))
        {
            RAPI.GetLocalPlayer().PersonController.jumpSpeed = float.Parse(ExtraSettingsAPI_GetInputValue("jumpSpeed"));
        }
        NourishmentVariables nv = GameModeValueManager.GetCurrentGameModeValue().nourishmentVariables;
        // hunger
        myClass.disableHungerOption = ExtraSettingsAPI_GetCheckboxState("disableHunger");
        if (myClass.disableHungerOption == true)
        {
            nv.foodDecrementRateMultiplier = 0f;
        }
        if (myClass.disableHungerOption == false)
        {
            nv.foodDecrementRateMultiplier = 1f;
        }
        // thirst
        myClass.disableThirstOption = ExtraSettingsAPI_GetCheckboxState("disableThirst");
        if (myClass.disableThirstOption == true)
        {
            nv.thirstDecrementRateMultiplier = 0f;
        }
        if (myClass.disableThirstOption == false)
        {
            nv.thirstDecrementRateMultiplier = 1f;
        }
        // saferaft
        myClass.safeRaftOption = ExtraSettingsAPI_GetCheckboxState("safeRaft");
        // sethookpullspeed
        if (!string.IsNullOrWhiteSpace(ExtraSettingsAPI_GetInputValue("pullSpeed")))
        {
            myClass.customPullSpeed = float.Parse(ExtraSettingsAPI_GetInputValue("pullSpeed"));
        }
    }


    public void ExtraSettingsAPI_SettingsClose()
    {
        // multiplier
        if (!string.IsNullOrWhiteSpace(ExtraSettingsAPI_GetInputValue("lootMultiplier")))
        {
            myClass.multiplier = int.Parse(ExtraSettingsAPI_GetInputValue("lootMultiplier"));
        }
        // infinite baits
        myClass.areBaitsInfinite = ExtraSettingsAPI_GetCheckboxState("infiniteBaits");
        // unbreakablerod
        myClass.isRodUnbreakable = ExtraSettingsAPI_GetCheckboxState("unbreakableRod");
        // infiniteoxygen
        myClass.oxygenBarOption = ExtraSettingsAPI_GetComboboxSelectedIndex("infiniteOxygen");
        // sprinklercheat
        myClass.sprinklerOption = ExtraSettingsAPI_GetComboboxSelectedIndex("sprinklerCheat");
        // weakshark
        myClass.weakSharkOption = ExtraSettingsAPI_GetComboboxSelectedIndex("weakShark");
        GameModeValueManager.GetCurrentGameModeValue().sharkVariables.damageMultiplier = myClass.weakSharkOption;
        // sprintspeed
        if (!string.IsNullOrWhiteSpace(ExtraSettingsAPI_GetInputValue("sprintSpeed")))
        {
            RAPI.GetLocalPlayer().PersonController.sprintSpeed = float.Parse(ExtraSettingsAPI_GetInputValue("sprintSpeed"));
        }
        // swimspeed
        if (!string.IsNullOrWhiteSpace(ExtraSettingsAPI_GetInputValue("swimSpeed")))
        {
            RAPI.GetLocalPlayer().PersonController.swimSpeed = float.Parse(ExtraSettingsAPI_GetInputValue("swimSpeed"));
        }
        // speed
        if (!string.IsNullOrWhiteSpace(ExtraSettingsAPI_GetInputValue("normalSpeed")))
        {
            RAPI.GetLocalPlayer().PersonController.normalSpeed = float.Parse(ExtraSettingsAPI_GetInputValue("normalSpeed"));
        }
        // gravity
        if (!string.IsNullOrWhiteSpace(ExtraSettingsAPI_GetInputValue("gravity")))
        {
            RAPI.GetLocalPlayer().PersonController.gravity = float.Parse(ExtraSettingsAPI_GetInputValue("gravity"));
        }
        // jumpspeed
        if (!string.IsNullOrWhiteSpace(ExtraSettingsAPI_GetInputValue("jumpSpeed")))
        {
            RAPI.GetLocalPlayer().PersonController.jumpSpeed = float.Parse(ExtraSettingsAPI_GetInputValue("jumpSpeed"));
        }
        NourishmentVariables nv = GameModeValueManager.GetCurrentGameModeValue().nourishmentVariables;
        // hunger
        myClass.disableHungerOption = ExtraSettingsAPI_GetCheckboxState("disableHunger");
        if (myClass.disableHungerOption == true)
        {
            nv.foodDecrementRateMultiplier = 0f;
        }
        if (myClass.disableHungerOption == false)
        {
            nv.foodDecrementRateMultiplier = 1f;
        }
        // thirst
        myClass.disableThirstOption = ExtraSettingsAPI_GetCheckboxState("disableThirst");
        if (myClass.disableThirstOption == true)
        {
            nv.thirstDecrementRateMultiplier = 0f;
        }
        if (myClass.disableThirstOption == false)
        {
            nv.thirstDecrementRateMultiplier = 1f;
        }
        // saferaft
        myClass.safeRaftOption = ExtraSettingsAPI_GetCheckboxState("safeRaft");
        // sethookpullspeed
        if (!string.IsNullOrWhiteSpace(ExtraSettingsAPI_GetInputValue("pullSpeed")))
        {
            myClass.customPullSpeed = float.Parse(ExtraSettingsAPI_GetInputValue("pullSpeed"));
        }
    }

}