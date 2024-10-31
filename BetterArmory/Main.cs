using BepInEx;
using BepInEx.Logging;
using BetterArmory.Artifact;
using BetterArmory.Buffs;
using BetterArmory.Equipment;
using BetterArmory.Items;
using R2API;
using R2API.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BetterArmory
{
#pragma warning restore CS0618 // Type or member is obsolete
    [BepInPlugin(ModGuid, ModName, ModVer)]
    [BepInDependency(R2API.R2API.PluginGUID, R2API.R2API.PluginVersion)]
    [NetworkCompatibility(CompatibilityLevel.EveryoneMustHaveMod, VersionStrictness.EveryoneNeedSameModVersion)]
    public class Main : BaseUnityPlugin
    {
        public const string ModGuid = "com.Tios.BetterArmory";
        public const string ModName = "Better Armory";
        public const string ModVer = "0.5.0";

        public static AssetBundle MainAssets;

        public List<ArtifactBase> Artifacts = new List<ArtifactBase>();
        public List<BuffBase> Buffs = new List<BuffBase>();
        public List<ItemBase> Items = new List<ItemBase>();
        public List<EquipmentBase> Equipments = new List<EquipmentBase>();

        private void Awake()
        {
            // Don't know how to create/use an asset bundle, or don't have a unity project set up?
            // Look here for info on how to set these up: https://github.com/KomradeSpectre/AetheriumMod/blob/rewrite-master/Tutorials/Item%20Mod%20Creation.md#unity-project

            
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("BetterArmory.betterarmoryassets"))
            {
                MainAssets = AssetBundle.LoadFromStream(stream);
            }


            //This section automatically scans the project for all artifacts
            var ArtifactTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ArtifactBase)));

            Logger.LogInfo($"ARTIFACT creation phase\n");
            foreach (var artifactType in ArtifactTypes)
            {
                ArtifactBase artifact = (ArtifactBase)Activator.CreateInstance(artifactType);
                if (ValidateArtifact(artifact, Artifacts))
                {
                    artifact.Init(Config);
                    Logger.LogInfo($"{artifact.ArtifactLangTokenName} was created !");
                }
            }


            //This section automatically scans the project for all buffs
            /*var BuffTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(BuffBase)));

            Logger.LogInfo($"Buff creation phase\n");
            foreach (var buffType in BuffTypes)
            {
                BuffBase buff = (BuffBase)Activator.CreateInstance(buffType);
                if (ValidateBuff(buff, Buffs))
                {
                    buff.Init(Config);
                    Logger.LogInfo($"{buff.BuffName} was created !");
                }
            }*/


            //This section automatically scans the project for all items
            var ItemTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(ItemBase)));

            Logger.LogInfo($"ITEM creation phase\n");
            foreach (var itemType in ItemTypes)
            {
                ItemBase item = (ItemBase)System.Activator.CreateInstance(itemType);
                if (ValidateItem(item, Items))
                {
                    item.Init(Config);
                    Logger.LogInfo($"{item.ItemLangTokenName} was created !");
                }
            }


            //this section automatically scans the project for all equipment
            var EquipmentTypes = Assembly.GetExecutingAssembly().GetTypes().Where(type => !type.IsAbstract && type.IsSubclassOf(typeof(EquipmentBase)));

            Logger.LogInfo($"EQUIPMENT creation phase\n");
            foreach (var equipmentType in EquipmentTypes)
            {
                EquipmentBase equipment = (EquipmentBase)System.Activator.CreateInstance(equipmentType);
                if (ValidateEquipment(equipment, Equipments))
                {
                    equipment.Init(Config);
                    Logger.LogInfo($"{equipment.EquipmentLangTokenName} was created !");
                }
            }
        }


        /// <summary>
        /// A helper to easily set up and initialize an artifact from your artifact classes if the user has it enabled in their configuration files.
        /// </summary>
        /// <param name="artifact">A new instance of an ArtifactBase class."</param>
        /// <param name="artifactList">The list you would like to add this to if it passes the config check.</param>
        public bool ValidateArtifact(ArtifactBase artifact, List<ArtifactBase> artifactList)
        {
            var enabled = Config.Bind<bool>("Artifact: " + artifact.ArtifactLangTokenName, "Enable Artifact?", true, "Should this artifact appear for selection?").Value;

            if (enabled)
            {
                artifactList.Add(artifact);
            }
            return enabled;
        }

        /// <summary>
        /// A helper to easily set up and initialize an item from your item classes if the user has it enabled in their configuration files.
        /// <para>Additionally, it generates a configuration for each item to allow blacklisting it from AI.</para>
        /// </summary>
        /// <param name="item">A new instance of an ItemBase class."</param>
        /// <param name="itemList">The list you would like to add this to if it passes the config check.</param>
        public bool ValidateItem(ItemBase item, List<ItemBase> itemList)
        {
            var enabled = Config.Bind<bool>("Item: " + item.ItemLangTokenName, "Enable Item?", true, "Should this item appear in runs?").Value;
            var aiBlacklist = Config.Bind<bool>("Item: " + item.ItemLangTokenName, "Blacklist Item from AI Use?", false, "Should the AI not be able to obtain this item?").Value;
            if (enabled)
            {
                itemList.Add(item);
                if (aiBlacklist)
                {
                    item.AIBlacklisted = true;
                }
            }
            return enabled;
        }

        /// <summary>
        /// A helper to easily set up and initialize an equipment from your equipment classes if the user has it enabled in their configuration files.
        /// </summary>
        /// <param name="equipment">A new instance of an EquipmentBase class."</param>
        /// <param name="equipmentList">The list you would like to add this to if it passes the config check.</param>
        public bool ValidateEquipment(EquipmentBase equipment, List<EquipmentBase> equipmentList)
        {
            if (Config.Bind<bool>("Equipment: " + equipment.EquipmentLangTokenName, "Enable Equipment?", true, "Should this equipment appear in runs?").Value)
            {
                equipmentList.Add(equipment);
                return true;
            }
            return false;
        }

        /// <summary>
        /// A helper to easily set up and initialize an buff from your buff classes.
        /// </summary>
        /// <param name="buff">A new instance of an BuffBase class."</param>
        /// <param name="buffList">The list you would like to add this to if it passes the config check.</param>
        public bool ValidateBuff(BuffBase buff, List<BuffBase> buffList)
        {
            buffList.Add(buff);
            return true;
        }
    }
}
