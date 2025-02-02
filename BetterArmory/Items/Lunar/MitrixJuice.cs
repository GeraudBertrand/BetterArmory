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
        public override string ItemPickupDesc => "A tonic to hasten your strikes… but power always comes at a price.";
        public override string ItemFullDescription => "Increases attack speed by 60% ( + 60% per stack ) but reduces attack damage by 60% ( + 60% per stack )";
        public override string ItemLore => "Mitrix Juice";

        public override ItemTier Tier => ItemTier.Lunar;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("MitrixJuiceDisplay.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("MitrixJuice.png");


        protected ConfigEntry<float> ChangeStatBase;
        protected ConfigEntry<float> ChangeStatStack;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            ChangeStatBase = config.Bind<float>("Item: "+ItemLangTokenName, "Base modification of attack speed and damage",0.6f,"How much attack speed and damage change should the player have at base ?");
            ChangeStatStack = config.Bind<float>("Item: " + ItemLangTokenName, "Modification of attack speed and damage per stack",0.2f, "How much attack speed and damage change should the player have per stack ?");
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
                args.damageMultAdd -=  (ChangeStatBase.Value + ChangeStatStack.Value * GetCount(sender));
                args.attackSpeedMultAdd += (ChangeStatBase.Value + ChangeStatStack.Value * GetCount(sender));
            }
        }
    }
}
