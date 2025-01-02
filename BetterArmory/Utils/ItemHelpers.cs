using BepInEx.Configuration;
using BetterArmory.Utils.Components;
using R2API;
using RoR2;
using System.Collections.Generic;
using UnityEngine;

namespace BetterArmory.Utils
{
    internal class ItemHelpers
    {
        /// <summary>
        /// A helper that will set up the RendererInfos of a GameObject that you pass in.
        /// <para>This allows it to go invisible when your character is not visible, as well as letting overlays affect it.</para>
        /// </summary>
        /// <param name="obj">The GameObject/Prefab that you wish to set up RendererInfos for.</param>
        /// <param name="debugmode">Do we attempt to attach a material shader controller instance to meshes in this?</param>
        /// <returns>Returns an array full of RendererInfos for GameObject.</returns>
        public static CharacterModel.RendererInfo[] ItemDisplaySetup(GameObject obj, bool debugmode = false)
        {
            List<Renderer> AllRenderers = new List<Renderer>();

            var meshRenderers = obj.GetComponentsInChildren<MeshRenderer>();
            if (meshRenderers.Length > 0) { AllRenderers.AddRange(meshRenderers); }

            var skinnedMeshRenderers = obj.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (skinnedMeshRenderers.Length > 0) { AllRenderers.AddRange(skinnedMeshRenderers); }

            CharacterModel.RendererInfo[] renderInfos = new CharacterModel.RendererInfo[AllRenderers.Count];

            for (int i = 0; i < AllRenderers.Count; i++)
            {
                if (debugmode)
                {
                    var controller = AllRenderers[i].gameObject.AddComponent<MaterialControllerComponents.HGControllerFinder>();
                    controller.Renderer = AllRenderers[i];
                }
                renderInfos[i] = new CharacterModel.RendererInfo
                {
                    defaultMaterial = AllRenderers[i] is SkinnedMeshRenderer ? AllRenderers[i].sharedMaterial : AllRenderers[i].material,
                    renderer = AllRenderers[i],
                    defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                    ignoreOverlays = false //We allow the mesh to be affected by overlays like OnFire or PredatoryInstinctsCritOverlay.
                };
            }
            return renderInfos;
        }

        public static void RefreshTimedBuffs(CharacterBody body, BuffDef buffDef, float duration)
        {
            if (!body || body.GetBuffCount(buffDef) <= 0) { return; }
            
        }

        public static void RefreshTimedBuffs(CharacterBody body, BuffDef buffDef, float taperStart, float taperDuration)
        {
            if (!body || body.GetBuffCount(buffDef) <= 0) { return; }
            
        }
    }

    internal static class Buffs
    {
        internal static BuffDef AddNewBuff(string buffName, Sprite buffIcon, Color buffColor, bool canStack = false, bool isDebuff = false, bool isHidden = false, EliteDef elideDef = null)
        {
            //IL_0010: Unknown result type (might be due to invalid IL or missing references)
            //IL_0011: Unknown result type (might be due to invalid IL or missing references)
            BuffDef val = ScriptableObject.CreateInstance<BuffDef>();
            val.name = $"BUFF_{buffName}";
            val.buffColor = buffColor;
            val.canStack = canStack;
            val.isDebuff = isDebuff;
            val.eliteDef = elideDef;
            val.iconSprite = buffIcon;
            val.isHidden = isHidden;
            ContentAddition.AddBuffDef(val);
            return val;
        }
    }

    internal static class ConfigCreator
    {
        internal static ConfigEntry<int> IntEntry(ConfigFile config, string itemName, string key, int value, string desc)
        {
            return config.Bind<int>( itemName, key, value, desc);
        }

        internal static ConfigEntry<float> FloatEntry(ConfigFile config, string itemName, string key, float value, string desc)
        {
            return config.Bind<float>( itemName, key, value, desc);
        }

        internal static ConfigEntry<string> StringEntry(ConfigFile config, string itemName, string key, string value, string desc)
        {
            return config.Bind<string>( itemName, key, value, desc);
        }
        internal static ConfigEntry<bool> BoolEntry(ConfigFile config, string itemName, string key, bool value, string desc)
        {
            return config.Bind<bool>(itemName, key, value, desc);
        }
    }
}
