using HarmonyLib;
using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VRage.Plugins;

namespace SE_ResizableChat
{
    public class Main : IPlugin
    {
        public void Init(object gameInstance)
        {
            new Harmony("ResizableChat").PatchAll(Assembly.GetExecutingAssembly());
            Config.Load();
        }

        public void Update()
        {

        }

        public void Dispose()
        {

        }

        public void OpenConfigDialog()
        {
            MyGuiSandbox.AddScreen(new ResizableChatConfigScreen());
        }
    }
}
