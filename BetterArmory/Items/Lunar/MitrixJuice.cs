using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using UnityEngine;
using static BetterArmory.Main;
using static R2API.RecalculateStatsAPI;

namespace BetterArmory.Items.Lunar
{
    /// <summary>
    /// #TODO Stats Affichage
    /// </summary>
    public class MitrixJuice : ItemBase
    {
        public override string ItemName => "Mitrix Juice";
        public override string ItemLangTokenName => "MITRIX_JUICE";
        public override string ItemPickupDesc => "Mitrix Juice";
        public override string ItemFullDescription => "Mitrix Juice";
        public override string ItemLore => "Mitrix Juice";

        public override ItemTier Tier => ItemTier.Lunar;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("MitrixJuiceDisplay.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("MitrixJuice.png");


        protected ConfigEntry<float> ReduceDamageStack;
        protected ConfigEntry<float> AugmentAttackSpeedStack;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            ReduceDamageStack = config.Bind<float>("Item: "+ItemLangTokenName, "Reduction damage per stack",0.6f,"How much damage decrease should the player have per stack ?");
            AugmentAttackSpeedStack = config.Bind<float>("Item: " + ItemLangTokenName, "Augmentation attack speed per stack",0.6f, "How much attack speed should the player have per stack ?");
        }


        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            GetStatCoefficients += ChangeStatPerJuice;
        }

        private void ChangeStatPerJuice(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender && GetCount(sender) > 0)
            {
                args.damageMultAdd -=  ReduceDamageStack.Value * GetCount(sender);
                args.attackSpeedMultAdd += AugmentAttackSpeedStack.Value * GetCount(sender);
            }
        }
    }
}
