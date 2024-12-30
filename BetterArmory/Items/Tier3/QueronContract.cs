using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using UnityEngine;

using static BetterArmory.Main;
using BetterArmory.Utils;
using static R2API.RecalculateStatsAPI;

namespace BetterArmory.Items.Tier3
{
    public class QueronContract : ItemBase
    {
        public override string ItemName => "Contract : Queron";
        public override string ItemLangTokenName => "QUERON_CONTRACT";
        public override string ItemPickupDesc => "LORE";
        public override string ItemFullDescription => $"Killing give a chance (1%) to get a permanent bonus of <style=></style> <style=cStack>(+ per stack)</style> to a stat";
        public override string ItemLore => "LORE";

        public override ItemTier Tier => ItemTier.Tier1; //ItemTier.Lunar

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("MyOrbDisplay.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("MyOrb.png");

        protected ConfigEntry<int> BaseStatPerm;
        protected ConfigEntry<int> StackStatPerm;

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            BaseStatPerm = config.Bind<int>("Item: " + ItemLangTokenName, "Base stat bonus", 2,"");
            StackStatPerm = config.Bind<int>("Item: " + ItemLangTokenName, "Stat bonus per stack", 2,"");
        }

        public override void Hooks()
        {
            GetStatCoefficients += UpdatePermBonus;
        }

        private void UpdatePermBonus(CharacterBody sender, StatHookEventArgs args)
        {
        }

        private StatHookEventArgs AddBonus(StatHookEventArgs args)
        {
            StatSelector ss = RandomSelectStat();
            switch (ss)
            {
                case StatSelector.HEALTH:
                    args.baseHealthAdd += 5;
                    break;
                case StatSelector.SHIELD:
                    args.baseShieldAdd += 5;
                    break;
                case StatSelector.ARMOR:
                    args.armorAdd += 2;
                    break;
                case StatSelector.REGEN:
                    args.baseRegenAdd += 2;
                    break;
                case StatSelector.MOVESPEED:
                    args.baseMoveSpeedAdd += 0.4f;
                    break;
                case StatSelector.SPRINTSPEED:
                    args.sprintSpeedAdd += 0.2f;
                    break;
                case StatSelector.JUMP:
                    args.baseJumpPowerAdd += 0.5f;
                    break;
                case StatSelector.DAMAGE:
                    args.baseDamageAdd += 2f;
                    break;
                case StatSelector.ATTACKSPEED:
                    args.baseAttackSpeedAdd += 0.2f;
                    break;
                case StatSelector.CRITCHANCE:
                    args.critAdd += 2f;
                    break;
                case StatSelector.CRITDAMAGE:
                    args.critDamageMultAdd += 0.2f;
                    break;
                case StatSelector.CURSE:
                    args.baseCurseAdd += 0.2f;
                    break;
                case StatSelector.COOLDOWN:
                    args.cooldownMultAdd -= 0.02f;
                    break;
            }

            return args;
        }

        public StatSelector RandomSelectStat()
        {
            return new System.Random().NextEnum<StatSelector>();
        }
    }

    public enum StatSelector
    {
        HEALTH = 0,         // baseHealthAdd
        SHIELD = 1,         // baseShieldAdd
        ARMOR = 2,          // armorAdd
        REGEN = 3,          // baseRegenAdd
        MOVESPEED = 4,      // baseMoveSpeedAdd
        SPRINTSPEED = 5,    // sprintSpeedAdd
        JUMP = 6,           // baseJumpPowerAdd
        DAMAGE = 7,         // baseDamageAdd
        ATTACKSPEED = 8,    // baseAttackSpeedAdd
        CRITCHANCE = 9,     // critAdd
        CRITDAMAGE = 10,    // critDamageMultAdd
        CURSE = 11,         // baseCurseAdd
        COOLDOWN = 12,      // cooldownReductionAdd

    }

}
