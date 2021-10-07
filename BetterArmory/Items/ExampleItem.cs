using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using System;
using UnityEngine;
using static BetterArmory.Main;

namespace BetterArmory.Items
{
    public class ExampleItem : ItemBase
    {

        public override string ItemName => "Deprecate Me Item";
        public override string ItemLangTokenName => "DEPRECATE_ME_ITEM";
        public override string ItemPickupDesc => "Give armor to proctect you";
        public override string ItemFullDescription => "Let armor be the thoughest thing in the world";
        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("assets/models/prefabs/item/firstitem/littleplate.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("assets/textures/icons/item/littleplate_icon.png");

        public ConfigEntry<float> ArmorPerStack;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            ArmorPerStack= config.Bind<float>("Item: "+ItemName,"Armor per Little Plate stack",0.08f,"How much armor should each stack of LitllePlate give");

        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += AddArmor;
            
        }


        private void AddArmor(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            ChatMessage.Send(sender.armor.ToString());
            args.armorAdd += ArmorPerStack.Value * (GetCount(sender) - 1);
            ChatMessage.Send(sender.armor.ToString());
        }

       
    }
}
