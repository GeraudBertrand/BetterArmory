using BepInEx.Configuration;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

using static BetterArmory.Main;
using static R2API.RecalculateStatsAPI;

namespace BetterArmory.Items
{
    /*public class GiantGloves : ItemBase
    {
        public override string ItemName => "Giants Gloves";
        public override string ItemLangTokenName => "GIANTS_GLOVES";
        public override string ItemPickupDesc => "Giants always are stronger than you!";
        public override string ItemFullDescription => $"Increase your health by <style=cIsHealth>{GrantedHealth.Value}%</style> <style=cStack>(+{GrantedHealth.Value}% per stack)</style> and make you taller by 3% <style=cStack>(+3% per stack)</style>. Boost your damage by <style=cIsHealth>3% of your <style=cIsHealth>max health.";
        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Lunar;

        public ConfigEntry<float> GrantedHealth;

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            GrantedHealth = config.Bind<float>("Item: " + ItemLangTokenName, "Percent of health granted by stack", 0.03f, "How much percent of health is granted");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        public override void Hooks()
        {
            GetStatCoefficients += GrantMaxHealth;
            On.RoR2.CharacterBody.FixedUpdate += GainDamage;
            GetStatCoefficients += GiantGlovesCalc;
        }

        private void GrantMaxHealth(CharacterBody sender, StatHookEventArgs args)
        {
            var count = GetCount(sender);
            if (count > 0)
            {
                HealthComponent healthC = sender.GetComponent<HealthComponent>();
                args.baseHealthAdd += healthC.fullHealth * ( GrantedHealth.Value * count);
            }
        }

        private void GainDamage(On.RoR2.CharacterBody.orig_FixedUpdate orig, RoR2.CharacterBody self)
        {
            orig(self);

            var glovesComponent = self.GetComponent<GiantGloveComponent>();
            if (!glovesComponent) { glovesComponent = self.gameObject.AddComponent<GiantGloveComponent>(); }


            var newCount = GetCount(self);
            var maxHealth = self.healthComponent.fullHealth;

            bool IsDifferent = false;
            if (glovesComponent.cachedInventoryCount != newCount)
            {
                IsDifferent = true;
                glovesComponent.cachedInventoryCount = newCount;
            }
            if(glovesComponent.cachedMaxHealth != maxHealth)
            {
                IsDifferent = true;
                glovesComponent.cachedMaxHealth = maxHealth;
            }

            if (!IsDifferent) return;
        }

        private void GiantGlovesCalc(CharacterBody sender, StatHookEventArgs args)
        {
            var glovesComponent = sender.GetComponent<GiantGloveComponent>();
            if(glovesComponent && glovesComponent.cachedInventoryCount > 0)
            {
                args.baseDamageAdd += 0.05f * glovesComponent.cachedMaxHealth;
            }
        }

        public class GiantGloveComponent : MonoBehaviour 
        {
            public int cachedInventoryCount = 0;
            public float cachedMaxHealth = 0;
        }

        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("assets/textures/icons/item/littleplate_icon.png");
        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("assets/models/prefabs/item/giantgloves/giantgloves.prefab");
        //This work is based on "Boxing Gloves - Right Handed" (https://sketchfab.com/3d-models/boxing-gloves-right-handed-1ae09e8e4959418b9c4274f9515c5d29) by Gohar.Munir (https://sketchfab.com/Gohar.Munir) licensed under CC-BY-4.0 (http://creativecommons.org/licenses/by/4.0/)
    }*/
}
