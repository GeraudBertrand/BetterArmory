using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

using BetterArmory.Components;
using BetterArmory.Utils;

using static BetterArmory.Main;
using static R2API.RecalculateStatsAPI;
using System;

namespace BetterArmory.Items.Tier3
{
    public class QueronContract : ItemBase
    {
        public override string ItemName => "Contract : Queron";
        public override string ItemLangTokenName => "QUERON_CONTRACT";
        public override string ItemPickupDesc => "The thirst for blood will make you stronger. Kill and you may be rewarded.";
        public override string ItemFullDescription => $"Killing give a chance <style=cIsUtility>( 4% <style=cStack>(+ 2% per stack)</style> )</style> to get a permanent bonus to a stat";
        public override string ItemLore => "LORE";

        public override ItemTier Tier => ItemTier.Tier3;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("QueronBookDisplay.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("QueronBookIcon.png");

        protected ConfigEntry<float> BaseHealthBonus;
        protected ConfigEntry<float> BaseShieldBonus;
        protected ConfigEntry<float> BaseArmorBonus;
        protected ConfigEntry<float> BaseRegenBonus;
        protected ConfigEntry<float> BaseMoveSpeedBonus;
        protected ConfigEntry<float> BaseSprintSpeedBonus;
        protected ConfigEntry<float> BaseDamageBonus;
        protected ConfigEntry<float> BaseAttackSpeedBonus;
        protected ConfigEntry<float> BaseCritChanceBonus;
        protected ConfigEntry<float> BaseCritDamageBonus;
        protected ConfigEntry<float> BaseCooldownBonus;

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
            BaseHealthBonus = ConfigCreator.FloatEntry(config, "Item: " + ItemLangTokenName, "Base health bonus", 10f,"How much health should the player gain from the item on each kill count bonus ?");
            BaseShieldBonus = ConfigCreator.FloatEntry(config, "Item: " + ItemLangTokenName, "Base shield bonus", 5f,"How much health should the player gain from the item on each kill count bonus ?");
            BaseArmorBonus = ConfigCreator.FloatEntry(config, "Item: " + ItemLangTokenName, "Base armor bonus", 2f,"How much health should the player gain from the item on each kill count bonus ?");
            BaseRegenBonus = ConfigCreator.FloatEntry(config, "Item: " + ItemLangTokenName, "Base regen bonus", 2f,"How much health should the player gain from the item on each kill count bonus ?");
            BaseMoveSpeedBonus = ConfigCreator.FloatEntry(config, "Item: " + ItemLangTokenName, "Base move speed bonus", 0.4f,"How much health should the player gain from the item on each kill count bonus ?");
            BaseSprintSpeedBonus = ConfigCreator.FloatEntry(config, "Item: " + ItemLangTokenName, "Base sprint speed bonus", 0.2f,"How much health should the player gain from the item on each kill count bonus ?");
            BaseDamageBonus = ConfigCreator.FloatEntry(config, "Item: " + ItemLangTokenName, "Base damage bonus", 7.5f,"How much health should the player gain from the item on each kill count bonus ?");
            BaseAttackSpeedBonus = ConfigCreator.FloatEntry(config, "Item: " + ItemLangTokenName, "Base attack speed bonus", 0.3f,"How much health should the player gain from the item on each kill count bonus ?");
            BaseCritChanceBonus = ConfigCreator.FloatEntry(config, "Item: " + ItemLangTokenName, "Base crit chance bonus", 3f,"How much health should the player gain from the item on each kill count bonus ?");
            BaseCritDamageBonus = ConfigCreator.FloatEntry(config, "Item: " + ItemLangTokenName, "Base crit damage bonus", 0.1f,"How much health should the player gain from the item on each kill count bonus ?");
            BaseCooldownBonus = ConfigCreator.FloatEntry(config, "Item: " + ItemLangTokenName, "Base cooldown bonus", 0.02f,"How much health should the player gain from the item on each kill count bonus ?");
        }

        public override void Hooks()
        {
            GetStatCoefficients += UpdatePermBonus;
            On.RoR2.CharacterBody.FixedUpdate += QueronStatValidator;
            On.RoR2.GlobalEventManager.OnCharacterDeath += QueronRollBonus;
        }


        private void UpdatePermBonus(CharacterBody sender, StatHookEventArgs args)
        {
            var queronComponent = sender.GetComponent<QueronStatFollower>();
            if (queronComponent && GetCount(sender) > 0)
            {
                queronComponent.ApplyBonusStat(
                    args, 
                    BaseHealthBonus.Value,
                    BaseShieldBonus.Value,
                    BaseArmorBonus.Value,
                    BaseRegenBonus.Value,
                    BaseMoveSpeedBonus.Value,
                    BaseSprintSpeedBonus.Value,
                    BaseDamageBonus.Value,
                    BaseAttackSpeedBonus.Value,
                    BaseCritChanceBonus.Value,
                    BaseCritDamageBonus.Value,
                    BaseCooldownBonus.Value
                    );
            }
        }

        /// <summary>
        /// Verify if the player has the component queron. If not, then add it to the gameobject
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        private void QueronStatValidator(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            orig(self);

            if(self && self.healthComponent)
            {
                var queronComponenent = self.GetComponent<QueronStatFollower>();
                if (!queronComponenent) 
                { 
                    queronComponenent = self.gameObject.AddComponent<QueronStatFollower>();
                    queronComponenent.Initialize();
                }
            }
        }

        /// <summary>
        /// Verify if the attacker has the item. In this case it will roll to see if there is a permanent bonus given.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="damageReport"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void QueronRollBonus(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, GlobalEventManager self, DamageReport damageReport)
        {
            orig(self, damageReport);

            var body = damageReport.attacker.GetComponent<CharacterBody>();
            if (body != null) 
            { 
                int nbItem = GetCount(body);
                if (nbItem > 0)
                {
                    //Roll
                    int chance = UnityEngine.Random.Range(1, 100);
                    if( chance <= (4 +  2*(nbItem-1)) )   // Default chanche 3% +2% par item 
                    {
                        StatSelector r = RandomSelectStat();
                        QueronStatFollower qsf = body.GetComponent<QueronStatFollower>();
                        if (!qsf) 
                        { 
                            qsf = body.gameObject.AddComponent<QueronStatFollower>(); 
                            qsf.Initialize(); 
                        }
                        qsf.Increment(r);
                        body.statsDirty = true;
                    }
                }
            }

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
        DAMAGE = 6,         // baseDamageAdd
        ATTACKSPEED = 7,    // baseAttackSpeedAdd
        CRITCHANCE = 8,     // critAdd
        CRITDAMAGE = 9,    // critDamageMultAdd
        COOLDOWN = 10,      // cooldownReductionAdd

    }

}
