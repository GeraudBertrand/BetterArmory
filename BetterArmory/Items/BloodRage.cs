using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;

using static BetterArmory.Main;
using static BetterArmory.Utils.ItemHelpers;
using static R2API.RecalculateStatsAPI;

namespace BetterArmory.Items
{
    class BloodRage : ItemBase
    {
        public override string ItemName => "Blood Rage";
        public override string ItemLangTokenName => "BLOOD_RAGE";
        public override string ItemPickupDesc => "";
        public override string ItemFullDescription => "";
        public override string ItemLore => "";

        public override ItemTier Tier => ItemTier.Tier2;

        public override GameObject ItemModel => MainAssets.LoadAsset<GameObject>("assets/models/prefabs/item/firstitem/littleplate.prefab");
        public override Sprite ItemIcon => MainAssets.LoadAsset<Sprite>("assets/textures/icons/item/littleplate_icon.png");

        public ConfigEntry<float> BaseDamageGranted;
        public ConfigEntry<float> StackDamageGranted;

        public BuffDef DamageBuff { get; private set; }

        public override void Init(ConfigFile config)
        {
            CreateConfig(config);
            CreateLang();
            CreateBuff();
            CreateItem();
            Hooks();
        }

        public override void CreateConfig(ConfigFile config)
        {
            BaseDamageGranted = config.Bind<float>("Item: Blood Rage", "Base damage granted by Blood Rage", 10f, "How much damage should the first gave you");
            StackDamageGranted = config.Bind<float>("Item: Blood rage", "Stack damage granted by Blood Rage", 8f, "How much damage each stack give you");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }

        private void CreateBuff()
        {
            DamageBuff = ScriptableObject.CreateInstance<BuffDef>();
            DamageBuff.name = "BetterArmory: Blood Rage Damage";
            DamageBuff.canStack = true;
            DamageBuff.isDebuff = false;

            BuffAPI.Add(new CustomBuff(DamageBuff));
        }

        public override void Hooks()
        {
            //Update buff on health
            On.RoR2.CharacterBody.FixedUpdate += CalculateDamage;

            //Update time buff on damage take
            On.RoR2.HealthComponent.TakeDamage += ResetTimer;
            
            //Add bonus damage on buff count
            GetStatCoefficients += AddDamageReward;
        }

        

        private void CalculateDamage(On.RoR2.CharacterBody.orig_FixedUpdate orig, CharacterBody self)
        {
            orig(self);

            var rageComponent = self.GetComponent<RageComponent>();
            if (!rageComponent) { 
                rageComponent = self.gameObject.AddComponent<RageComponent>();
                rageComponent.cachedHealth = self.healthComponent.health;
            }
            var newInventoryCount = GetCount(self);
            var actualLife = self.healthComponent.health;

            if(rageComponent.cachedInventoryCount != newInventoryCount)
            {
                rageComponent.cachedInventoryCount = newInventoryCount;
            }

            
            var buffCount = self.GetBuffCount(DamageBuff);
                
            if (rageComponent.cachedInventoryCount > 0)
            {
                var stack = StackOfPercentLost(actualLife, self.healthComponent.fullHealth);
                rageComponent.cachedStack = stack;
                if (!self.HasBuff(DamageBuff.buffIndex))
                {
                    self.AddTimedBuffAuthority(DamageBuff.buffIndex,20f);
                }
                self.SetBuffCount(DamageBuff.buffIndex, rageComponent.cachedStack);
            }
            rageComponent.cachedHealth = actualLife;
        }

        private void ResetTimer(On.RoR2.HealthComponent.orig_TakeDamage orig, HealthComponent self, DamageInfo damageInfo)
        {
            orig(self,damageInfo);

            var body = self.body;
            if (body)
            {
                if (body.HasBuff(DamageBuff.buffIndex))
                {
                    RefreshTimedBuffs(body,DamageBuff,20f);
                }
            }
        }

        private void AddDamageReward(CharacterBody sender, StatHookEventArgs args)
        {
            if (sender.HasBuff(DamageBuff)) { args.baseDamageAdd += 5f * sender.GetBuffCount(DamageBuff); }
        }

        private int StackOfPercentLost(float actual, float full)
        {
            var actualPercent = Mathf.Round((actual*100)/ full);
            var lostPercent = 100 - actualPercent;
            int stack = (int)(lostPercent / 5f);
            return stack;
        }

        public class RageComponent : MonoBehaviour
        {
            public int cachedInventoryCount = 0;
            public int cachedStack = 0;
            public float cachedHealth = 0;
        }
    }
}
