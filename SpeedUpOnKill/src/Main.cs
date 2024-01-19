using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using UnityEngine;
using RoR2;
using RiskOfOptions;
using RiskOfOptions.Options;
using RiskOfOptions.OptionConfigs;
using System;

namespace SpeedUpOnKill;
[BepInPlugin(PluginGUID, PluginName, PluginVersion)]

public class Main : BaseUnityPlugin{
    public const string PluginGUID = PluginAuthor + "." + PluginName;
    public const string PluginAuthor = "MarkTullius";
    public const string PluginName = "TimescaleShenanigans";
    public const string PluginVersion = "2.1.2";

    public static ConfigEntry<Scaling> ScalingStyle { get; set; }
    public static ConfigEntry<float> TimeFactor { get; set; }
    public static ConfigEntry<float> Ceiling { get; set; }
    public static ConfigEntry<float> UpperBound { get; set; }
    public static ConfigEntry<float> LowerBound { get; set; }
    public static ConfigEntry<float> KillFactor { get; set; }
    public static ConfigEntry<bool> ResetEachStage { get; set; }

    public float killCount = 0f;

    public void Awake(){
        InitConfig();
        if (Chainloader.PluginInfos.ContainsKey("com.rune580.riskofoptions"))
            BuildSettings();
        On.RoR2.GlobalEventManager.OnCharacterDeath += IncreaseTimeScale;
        On.RoR2.Run.Start += SetVoidCoins;
        On.RoR2.Run.BeginStage += ResetPerStage;
        On.RoR2.Run.OnDestroy += ResetTimescale;
    }

    public void InitConfig(){
        ScalingStyle = Config.Bind(
            "General"
        ,   "Scaling Style"
        ,   Scaling.Linear
        ,   @"The selected style of time scaling.
            Linear scales the timescale of the engine upon killing an enemy (by default 1 kill will increase, config option to set this to be less frequent).
            Random chooses the timescale to set the engine to between the upper and lower bounds (set in config values)."
        );
        ResetEachStage = Config.Bind(
            "General"
        ,   "Reset Each Stage"
        ,   false
        ,   "Turn on to reset the timescale to 100% every time you load into a new stage."
        );
        KillFactor = Config.Bind(
            "General"
        ,   "Kill Factor"
        ,   1f
        ,   "The kill interval at which the timescale will change. By default one kill will change the timescale, set higher to have the timescale change less frequently."
        );
        TimeFactor = Config.Bind(
            "Linear"
        ,   "Time Factor"
        ,   0.005f
        ,   "The factor by which to increase the timescale by upon a kill. Enter the % factor that you wish the game to increase by every time the Kill Factor is reached. i.e. 0.005 = 0.5%"
        );
        Ceiling = Config.Bind(
            "Linear"
        ,   "Ceiling"
        ,   10f
        ,   "The hard cap that the timescale cannot go above with Linear scaling. WARNING changing this to a higher value may cause your game to crash (or even BSOD at ridiculous values) due to hardware limitations. Reduce this if you have concerns over the capabilities of your machine."
        );
        UpperBound = Config.Bind(
            "Random"
        ,   "Upper Bound"
        ,   3f
        ,   "Defines the maximum that the timescale can be set to."
        );
        LowerBound = Config.Bind(
            "Random"
        ,   "Lower Bound"
        ,   0.5f
        ,   "Defines the minimum that the timescale can be set to."
        );
    }

    public void ResetTimescale(On.RoR2.Run.orig_OnDestroy orig, Run self){
        orig(self);
        Time.timeScale = 1f;
    }

    public void ResetPerStage(On.RoR2.Run.orig_BeginStage orig, Run self){
        orig(self);
        if (ResetEachStage.Value)
        {
            Time.timeScale = 1f;
            #pragma warning disable Publicizer001
            PlayerCharacterMasterController._instances[0].master.voidCoins = (uint) 100f;
            #pragma warning restore Publicizer001
        }
    }

    public void SetVoidCoins(On.RoR2.Run.orig_Start orig, Run self){
        orig(self);
        #pragma warning disable Publicizer001
        PlayerCharacterMasterController._instances[0].master.voidCoins = (uint) 100f;
        #pragma warning restore Publicizer001
    }

    public void BuildSettings(){
        StepSliderConfig generalConfig = new() {
            min = 0.001f, max = Ceiling.Value, increment = 0.001f
        };
        StepSliderConfig ceilingConfig = new() {
            min = 2f, max = 100f, increment = 1f
        };
        StepSliderConfig killConfig = new() {
            min = 1f, max = 100f, increment = 1f
        };
        ModSettingsManager.AddOption(new ChoiceOption(ScalingStyle));
        ModSettingsManager.AddOption(new StepSliderOption(TimeFactor, generalConfig));
        ModSettingsManager.AddOption(new StepSliderOption(UpperBound, generalConfig));
        ModSettingsManager.AddOption(new StepSliderOption(LowerBound, generalConfig));
        ModSettingsManager.AddOption(new StepSliderOption(Ceiling, ceilingConfig));
        ModSettingsManager.AddOption(new StepSliderOption(KillFactor, killConfig));
        ModSettingsManager.AddOption(new CheckBoxOption(ResetEachStage));
    }

    public void IncreaseTimeScale(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport){
        orig(self, damageReport);
        killCount += 1;
        if ((killCount % KillFactor.Value) == 0){
            CharacterBody killerBody = damageReport.attackerBody;
            if (killerBody && (killerBody.isPlayerControlled || killerBody.baseNameToken == "ENGITURRET_BODY_NAME")){
                switch (ScalingStyle.Value){
                    case Scaling.Linear:
                        Time.timeScale += TimeFactor.Value;
                        break;

                    case Scaling.Random:
                        Time.timeScale = UnityEngine.Random.Range(LowerBound.Value, UpperBound.Value);
                        break;
                }
                #pragma warning disable Publicizer001
                PlayerCharacterMasterController._instances[0].master.voidCoins = (uint) (Time.timeScale * 100f);
                #pragma warning restore Publicizer001
            }
        }
    }

    public enum Scaling{
        Linear
    ,   Random
    }
}