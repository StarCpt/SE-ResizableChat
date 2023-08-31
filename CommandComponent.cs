using Sandbox.Game.GameSystems;
using Sandbox.Game.GameSystems.Chat;
using Sandbox.Game.World;
using Sandbox.Graphics.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.Components;

namespace SE_ResizableChat
{
    [MySessionComponentDescriptor(MyUpdateOrder.NoUpdate)]
    public class CommandComponent : MySessionComponentBase
    {
        public override void BeforeStart()
        {
            MySession.Static.GetComponent<MyChatSystem>()
                .CommandSystem
                .ScanAssemblyForCommands(Assembly.GetExecutingAssembly());
        }

        [ChatCommand("/chatconfig", "Resizable Chat Plugin Config", "ResizableChat Config", VRage.Game.ModAPI.MyPromoteLevel.None)]
        public static void CommandChatConfig(string[] args)
        {
            MyGuiSandbox.AddScreen(new ResizableChatConfigScreen());
        }
    }
}
