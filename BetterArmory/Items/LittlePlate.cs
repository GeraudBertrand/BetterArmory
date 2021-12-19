using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;

using static R2API.RecalculateStatsAPI;
using static BetterArmory.Main;

namespace BetterArmory.Items
{
    public class LittlePlate : ItemBase
    {

        public override string ItemName => "Little Plate";
        public override string ItemLangTokenName => "LITTLE_PLATE";
        public override string ItemPickupDesc => "Give armor to protect you";
        public override string ItemFullDescription => "Let armor be the thoughest thing in the world";
        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("assets/models/prefabs/item/firstitem/littleplate.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("assets/textures/icons/item/littleplate_icon.png");

        public ConfigEntry<float> ArmorBase;
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
            ArmorBase = config.Bind<float>("Item: " + ItemLangTokenName, "Base armor for Little Plate", 10.0f, "How much armor should the first gave you");
            ArmorPerStack = config.Bind<float>("Item: " + ItemLangTokenName, "Armor per Little Plate stack", 15.0f, "How much armor should each stack of LitllePlate give");

        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            GetStatCoefficients += AddArmor;
        }

        private void AddArmor(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            var inventoryCount = GetCount(sender);
            if (inventoryCount > 0)
            {
                args.armorAdd += ArmorBase.Value + (ArmorPerStack.Value * (inventoryCount - 1));
            }

        }
    }
}
