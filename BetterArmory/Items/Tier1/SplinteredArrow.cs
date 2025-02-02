using BepInEx.Configuration;
using BetterArmory.Utils;
using R2API;
using RoR2;
using System;
using UnityEngine;
using static BetterArmory.Main;

namespace BetterArmory.Items.Tier1
{
    public class SplinteredArrow : ItemBase
    {
        public override string ItemName => "Splintered Arrow";
        public override string ItemLangTokenName => "SPLINTERED_ARROW";
        public override string ItemPickupDesc => "The shaft is broken, the arrowhead missing—yet its mark remains. Attacks have a chance to curse enemies, amplifying the pain they suffer.";
        public override string ItemFullDescription => $"<style=cIsUtility>{ChanceToMark.Value * 100}% to curse</style> enemies on hit, dealing <style=cIsDamage>{BaseDamageBonusMark.Value *100}%</style> <style=cStack>(+ {StackDamageBonusMark.Value *100}% per stack)</style> more damage.";
        public override string ItemLore => "Lore";

        public override ItemTier Tier => ItemTier.Tier1;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("BrokenArrowDisplay.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("BrokenArrow_icon.png");

        public static BuffDef MarkDebuff;


        protected ConfigEntry<float> ChanceToMark;
        protected ConfigEntry<float> TimeDebuff;
        protected ConfigEntry<float> BaseDamageBonusMark;
        protected ConfigEntry<float> StackDamageBonusMark;


        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateBuffs();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            ChanceToMark = config.Bind<float>("Item: " + ItemLangTokenName,"Chance to mark an enemy with the debuff", 0.25f,"How much chance each hit has to set debuff on enemy hit ?");
            TimeDebuff = config.Bind<float>("Item: " + ItemLangTokenName,"Lifetime debuff on enemy", 4f,"How much lifetime debuff on enemy marked ?");
            BaseDamageBonusMark = config.Bind<float>("Item: " + ItemLangTokenName, "Base damage bonus on debuff enemy", 0.25f, "How much bonus damage debuff give at base ?");
            StackDamageBonusMark = config.Bind<float>("Item: " + ItemLangTokenName, "Stakc damage bonus on debuff enemy", 0.1f, "How much bonus damage debuff give by stack ?");
        }
        public override void CreateBuffs()
        {
            MarkDebuff = Buffs.AddNewBuff("Splintered_Mark", MainAssets.LoadAsset<Sprite>("SplinteredArrow.png"), Color.magenta, canStack: false, isDebuff: true, isHidden: false);
        }

        public override void Hooks()
        {
            On.RoR2.GlobalEventManager.OnHitEnemy += MarkDebuffEnemy;
            On.RoR2.HealthComponent.TakeDamage += DamageUpOnDebuff;
        }

        private void DamageUpOnDebuff(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            if (self.body && self.body.HasBuff(MarkDebuff))
            {
                damageInfo.damage *= 1 + BaseDamageBonusMark.Value + (StackDamageBonusMark.Value * (GetCount(damageInfo.attacker.GetComponent<CharacterBody>())-1) );
            }
            orig(self, damageInfo);
        }

        /// <summary>
        /// Mark a victim with a debuff for the timeDebuff value.
        /// </summary>
        /// <param name="orig"></param>
        /// <param name="self"></param>
        /// <param name="damageInfo"></param>
        /// <param name="victim"></param>
        private void MarkDebuffEnemy(On.RoR2.GlobalEventManager.orig_OnHitEnemy orig, GlobalEventManager self, DamageInfo damageInfo, GameObject victim)
        {
            if (orig != null)
            {
                if (victim != null)
                {
                    if (damageInfo.inflictor != null)
                    {
                        CharacterBody attacker = damageInfo.inflictor.GetComponent<CharacterBody>();
                        if ( attacker != null && GetCount(attacker) > 0)
                        {
                            float roll = UnityEngine.Random.Range(0f, 1f);
                            if (roll <= ChanceToMark.Value)
                            {
                                CharacterBody vBody = victim.GetComponent<CharacterBody>();
                                if (vBody != null)
                                {
                                    vBody.AddTimedBuff(MarkDebuff, TimeDebuff.Value);
                                }
                            }
                        }
                    }
                }
            }
            orig(self,damageInfo,victim);
        }
    

    }
}
