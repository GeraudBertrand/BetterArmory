using BepInEx.Configuration;
using R2API;
using RoR2;
using UnityEngine;
using static R2API.RecalculateStatsAPI;
using static BetterArmory.Main;

namespace BetterArmory.Items.Tier1
{
    public class LittlePlate : ItemBase
    {

        public override string ItemName => "Little Plate";
        public override string ItemLangTokenName => "LITTLE_PLATE";
        public override string ItemPickupDesc => "Gain armor to reinforce yourself.";
        public override string ItemFullDescription => $"Increase armor by <style=cIsHealing>{ArmorBase.Value}</style> <style=cStack>(+{ArmorPerStack.Value} per stack)</style>";
        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier1;

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("MyOrb.png");
        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("MyOrbDisplay.prefab");



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
            ArmorBase = config.Bind("Item: " + ItemLangTokenName, "Base armor for Little Plate", 10.0f, "How much armor should the first gave you");
            ArmorPerStack = config.Bind("Item: " + ItemLangTokenName, "Armor per Little Plate stack", 15.0f, "How much armor should each stack of LitllePlate give");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            GetStatCoefficients += AddArmor;
        }

        private void AddArmor(CharacterBody sender, StatHookEventArgs args)
        {
            var inventoryCount = GetCount(sender);
            if (inventoryCount > 0)
            {
                args.armorAdd += ArmorBase.Value + ArmorPerStack.Value * (inventoryCount - 1);
            }

        }
    }
}
