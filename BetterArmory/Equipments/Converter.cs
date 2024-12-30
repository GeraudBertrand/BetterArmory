using BepInEx.Configuration;
using R2API;
using R2API.Utils;
using RoR2;
using UnityEngine;
using static BetterArmory.Main;
using BetterArmory.Buffs;

namespace BetterArmory.Equipment
{
    public class Converter : EquipmentBase
    {
        public override string EquipmentName => "Converter";
        public override string EquipmentLangTokenName => "CONVERTER";
        public override string EquipmentPickupDesc => "";
        public override string EquipmentFullDescription => "";
        public override string EquipmentLore => "";

        public override GameObject EquipmentModel => MainAssets.LoadAsset<GameObject>("assets/models/prefabs/equipment/converteur/converteur.prefab");
        public override Sprite EquipmentIcon => MainAssets.LoadAsset<Sprite>("assets/textures/icons/item/littleplate_icon.png");

        public ConfigEntry<float> ConverterRate;
        public ConfigEntry<float> ConverterCooldown;
        public override float Cooldown => ConverterCooldown.Value;

        public float exchange { get; private set; }

        public BuffDef ConvertBuff { get; private set; }

        public override void Init(ConfigFile config)
        {
            SetupBuff();
            CreateConfig(config);
            CreateLang();
            CreateEquipment();
            Hooks();
        }

        protected void SetupBuff()
        {
            ConvertBuff = ScriptableObject.CreateInstance<BuffDef>();
            ConvertBuff.name = "BUFF_LIFESWITCHSHIELD";
            ConvertBuff.canStack = true;
            ConvertBuff.isCooldown = false;
            ConvertBuff.isDebuff = false;
            ConvertBuff.isHidden = false;
            ConvertBuff.buffColor = Color.white;
            ConvertBuff.iconSprite = MainAssets.LoadAsset<Sprite>("MyOrb.png");

            ContentAddition.AddBuffDef(ConvertBuff);
        }

        protected override void CreateConfig(ConfigFile config)
        {
            ConverterCooldown = config.Bind<float>("Equipment : "+ EquipmentLangTokenName, "cooldown",10f,"how much cooldown");
            ConverterRate = config.Bind<float>("Equipment : "+ EquipmentLangTokenName, "rate",.2f,"how much rate");
        }

        public override ItemDisplayRuleDict CreateItemDisplayRules()
        {
            return new ItemDisplayRuleDict();
        }


        /// <summary>
        /// Hook in the awake and use a buff to check for change
        /// Maybe : slot.characterBody.isLocalPlayer
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        protected override bool ActivateEquipment(EquipmentSlot slot)
        {
            CharacterBody body = slot.characterBody;
            if (!body || !body.teamComponent) return false;
            ChatMessage.SendColored("Converting...", Color.green);

            var health = body.healthComponent;
            if (health.fullHealth > 1f)
            {
                body.AddBuff(ConvertBuff);
                return true;
            }
            else ChatMessage.SendColored("Not enough health !", Color.red);
           

            return false;
        }

        private void ModifyHealthAndShield(CharacterBody sender, RecalculateStatsAPI.StatHookEventArgs args)
        {
            if (sender.isPlayerControlled && sender.HasBuff(ConvertBuff))
            {
                exchange = sender.baseMaxHealth * ConverterRate.Value;
                sender.baseMaxShield += exchange;
                sender.baseMaxHealth -= exchange;

                ChatMessage.SendColored("Conversion done !", Color.green);
                sender.RemoveBuff(ConvertBuff);
            }
        }

        public override void Hooks()
        {
            RecalculateStatsAPI.GetStatCoefficients += ModifyHealthAndShield;
        }
    }
}
