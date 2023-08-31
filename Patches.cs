using HarmonyLib;
using Sandbox.Game.Gui;
using Sandbox.Game.GUI.HudViewers;
using Sandbox.Graphics;
using Sandbox.Graphics.GUI;
using Sandbox.ModAPI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VRage.Utils;
using VRageMath;

namespace SE_ResizableChat
{
    [HarmonyPatch]
    public static class Patches
    {
        [HarmonyPatch(typeof(MyGuiControlMultilineText), "TextScale", MethodType.Setter)]
        [HarmonyPrefix]
        public static void MyHudControlChat_TextScale_Setter_Prefix(ref float value, MyGuiControlMultilineText __instance)
        {
            if (__instance is MyHudControlChat)
            {
                value = Config.Static.TextScale;
            }
        }
        
        [HarmonyPatch(typeof(MyGuiControlMultilineText), "Draw", MethodType.Normal)]
        [HarmonyPrefix]
        public static bool MyHudControlChat_Draw_Prefix(MyGuiControlMultilineText __instance)
        {
            if (ResizableChatConfigScreen.hideChat && __instance is MyHudControlChat)
            {
                return false;
            }
        
            return true;
        }

        [HarmonyPatch(typeof(MyHudControlChat))]
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPatch(new Type[]{
            typeof(MyHudChat),
            typeof(Vector2?),
            typeof(Vector2?),
            typeof(Vector4?),
            typeof(string),
            typeof(float),
            typeof(MyGuiDrawAlignEnum),
            typeof(StringBuilder),
            typeof(MyGuiDrawAlignEnum),
            typeof(int?),
            typeof(bool) })]
        [HarmonyPrefix]
        public static void MyHudControlChat_Constructor_Prefix(ref Vector2? size, ref float textScale)
        {
            size = new Vector2(Config.Static.ChatWidth, Config.Static.ChatHeight);
            textScale = Config.Static.TextScale;
        }
    }
}
