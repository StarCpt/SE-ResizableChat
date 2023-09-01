using HarmonyLib;
using Sandbox;
using Sandbox.Game.Gui;
using Sandbox.Game.GUI.HudViewers;
using Sandbox.Game.World;
using Sandbox.Graphics;
using Sandbox.Graphics.GUI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using VRage.Game.Localization;
using VRage.Utils;
using VRageMath;

namespace SE_ResizableChat
{
    public class ResizableChatConfigScreen : MyGuiScreenBase
    {
        public const float DEFAULT_TEXTSCALE = 0.7f;
        public const float DEFAULT_CHATWIDTH = 0.339f;
        public const float DEFAULT_CHATHEIGHT = 0.28f;

        public static bool configChanged = true;
        public static bool hideChat = false;

        private MyGuiControlSlider textScaleSlider, chatWidthSlider, chatHeightSlider;
        private MyGuiControlCheckbox hideChatCheckbox;

        public ResizableChatConfigScreen() :
            base(new Vector2(0.5f),
                MyGuiConstants.SCREEN_BACKGROUND_COLOR,
                new Vector2(0.5f),
                true,
                null,
                MySandboxGame.Config.UIBkOpacity,
                MySandboxGame.Config.UIOpacity)
        {

        }

        public override string GetFriendlyName()
        {
            return nameof(ResizableChatConfigScreen);
        }

        public override void LoadContent()
        {
            base.LoadContent();

            RecreateControls(true);
        }

        public override void RecreateControls(bool constructor)
        {
            base.RecreateControls(constructor);

            AddCaption("Resizable Chat Plugin Config");

            var grid = new UniformGrid(2, 4, new Vector2(0.2f, 0.05f));

            grid.Add(new MyGuiControlLabel(text: "Text Scale", originAlign: MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER), 0, 0);
            grid.Add(
                textScaleSlider = new MyGuiControlSlider(
                    toolTip: "Text Scale",
                    originAlign:MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER,
                    width: 0.2f,
                    labelText: "{0}",
                    labelFont: "Blue",
                    labelSpaceWidth: 0.04f,
                    labelDecimalPlaces: 2,
                    showLabel: true,
                    minValue: 0.1f,
                    maxValue: 1f,
                    defaultValue: Config.Static.TextScale), 1, 0);

            grid.Add(new MyGuiControlLabel(text: "Chat Box Width", originAlign: MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER), 0, 1);
            grid.Add(
                chatWidthSlider = new MyGuiControlSlider(
                    toolTip: "Chat Width",
                    originAlign: MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER,
                    width: 0.2f,
                    labelText: "{0}",
                    labelFont: "Blue",
                    labelSpaceWidth: 0.04f,
                    labelDecimalPlaces: 2,
                    showLabel: true,
                    minValue: 0.1f,
                    maxValue: 1f,
                    defaultValue: Config.Static.ChatWidth), 1, 1);

            grid.Add(new MyGuiControlLabel(text: "Chat Box Height", originAlign: MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER), 0, 2);
            grid.Add(
                chatHeightSlider = new MyGuiControlSlider(
                    toolTip: "Chat Height",
                    originAlign: MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER,
                    width: 0.2f,
                    labelText: "{0}",
                    labelFont: "Blue",
                    labelSpaceWidth: 0.04f,
                    labelDecimalPlaces: 2,
                    showLabel: true,
                    minValue: 0.05f,
                    maxValue: 0.7f,
                    defaultValue: Config.Static.ChatHeight), 1, 2);

            grid.Add(new MyGuiControlLabel(text: "Hide Chat", originAlign: MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER), 0, 3);
            grid.Add(
                hideChatCheckbox = new MyGuiControlCheckbox(
                    isChecked: hideChat,
                    originAlign: MyGuiDrawAlignEnum.HORISONTAL_LEFT_AND_VERTICAL_CENTER), 1, 3);

            grid.AddItemsTo(Controls, new Vector2(-0.09f, 0), false);

            CloseButtonEnabled = true;

            float btnYPos = (Size.Value.Y * 0.5f) - (MyGuiConstants.SCREEN_CAPTION_DELTA_Y / 2f);

            MyGuiControlButton saveBtn = new MyGuiControlButton(
                new Vector2(-0.1f, btnYPos),
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM,
                text: new StringBuilder("Save & Exit"),
                onButtonClick: OnSaveButtonClick);

            Controls.Add(saveBtn);

            MyGuiControlButton restoreDefaultsBtn = new MyGuiControlButton(
                new Vector2(0.1f, btnYPos),
                originAlign: MyGuiDrawAlignEnum.HORISONTAL_CENTER_AND_VERTICAL_BOTTOM,
                text: new StringBuilder("Default"),
                onButtonClick: RestoreDefaults);

            Controls.Add(restoreDefaultsBtn);
        }

        private static readonly Action<MyGuiControlMultilineText, float> m_textScaleSetter =
            AccessTools.Field(typeof(MyGuiControlMultilineText), "m_textScale").CreateSetter<MyGuiControlMultilineText, float>();

        private static readonly Action<MyGuiControlMultilineText, float> m_textScaleWithLanguageSetter =
            AccessTools.Field(typeof(MyGuiControlMultilineText), "m_textScaleWithLanguage").CreateSetter<MyGuiControlMultilineText, float>();

        private void OnSaveButtonClick(MyGuiControlButton btn)
        {
            if (Config.Static.TextScale != textScaleSlider.Value ||
                Config.Static.ChatWidth != chatWidthSlider.Value ||
                Config.Static.ChatHeight != chatHeightSlider.Value)
            {
                configChanged = true;
            }

            Config.Static.TextScale = textScaleSlider.Value;
            Config.Static.ChatWidth = chatWidthSlider.Value;
            Config.Static.ChatHeight = chatHeightSlider.Value;
            hideChat = hideChatCheckbox.IsChecked;

            CloseScreen();

            if (configChanged && MySession.Static != null && MyHud.Chat?.ChatControl != null)
            {
                MyHudControlChat chat = MyHud.Chat.ChatControl;

                m_textScaleSetter.Invoke(chat, Config.Static.TextScale);
                m_textScaleWithLanguageSetter.Invoke(chat, Config.Static.TextScale * MyLanguage.Instance?.LanguageTextScale ?? 1f);

                chat.Size = new Vector2(Config.Static.ChatWidth, Config.Static.ChatHeight);

                RecalculateLineHeight(chat, Config.Static.TextScale * MyLanguage.Instance?.LanguageTextScale ?? 1f);
                chat.RefreshText(false);

                chat.Visibility = MyHudControlChat.MyChatVisibilityEnum.AlwaysVisible;
                MyHud.Chat.ChatOpened();
                MyHud.Chat.Update();
                MyHud.Chat.ChatClosed();
            }

            Config.Save();
        }

        private static readonly StringBuilder m_lineHeightMeasure = new StringBuilder("Ajqypdbfgjl");

        private static readonly Func<MyGuiControlMultilineText, MyRichLabel> m_labelGetter =
            AccessTools.Field(typeof(MyGuiControlMultilineText), "m_label").CreateGetter<MyGuiControlMultilineText, MyRichLabel>();
        private static readonly Action<MyRichLabel, float> m_minLineHeightSetter =
            AccessTools.Field(typeof(MyRichLabel), "m_minLineHeight").CreateSetter<MyRichLabel, float>();

        public static void RecalculateLineHeight(MyGuiControlMultilineText control, float newFontScale)
        {
            var label = m_labelGetter.Invoke(control);

            if (label == null)
                return;

            float y = MyGuiManager.MeasureString(control.Font, m_lineHeightMeasure, newFontScale).Y;

            m_minLineHeightSetter.Invoke(label, y);

            var lines = (IList)AccessTools.Field(typeof(MyRichLabel), "m_lineSeparators").GetValue(label);
            foreach (var line in lines)
            {
                if (line == null)
                    continue;

                Type t = line.GetType();
                AccessTools.Field(t, "m_minLineHeight").SetValue(line, y);
                AccessTools.Method(t, "RecalculateSize").Invoke(line, null);
            }
        }

        private void RestoreDefaults(MyGuiControlButton btn)
        {
            textScaleSlider.Value = DEFAULT_TEXTSCALE;
            chatWidthSlider.Value = DEFAULT_CHATWIDTH;
            chatHeightSlider.Value = DEFAULT_CHATHEIGHT;
            hideChatCheckbox.IsChecked = false;
        }
    }

    public class GuiControlWrapper
    {
        public MyGuiControlBase Control;
        public int Column;
        public int Row;
    }

    public class UniformGrid
    {
        public int Columns { get; set; }
        public int Rows { get; set; }

        public Vector2 CellSize { get; set; }

        private List<GuiControlWrapper> controls;

        public UniformGrid(int columns, int rows, Vector2 cellSize)
        {
            Columns = columns;
            Rows = rows;
            CellSize = cellSize;

            controls = new List<GuiControlWrapper>();
        }

        public void Add(MyGuiControlBase control, int column, int row)
        {
            controls.Add(new GuiControlWrapper
            {
                Control = control,
                Column = column,
                Row = row,
            });
        }

        public void AddItemsTo(MyGuiControls target, Vector2 position, bool addBorderLines = false)
        {
            Vector2 totalSize = new Vector2(Columns, Rows) * CellSize;
            Vector2 topLeft = position - (totalSize / 2);
            //Vector2 bottomRight = position + (totalSize / 2);
            foreach (var control in controls)
            {
                control.Control.Position =
                    topLeft + (CellSize / 2) + (new Vector2(control.Column, control.Row) * CellSize);

                target.Add(control.Control);
            }

            if (addBorderLines)
            {
                var separators = new MyGuiControlSeparatorList
                {
                    Position = position,
                };

                for (int x = 0; x < Columns + 1; x++)
                {
                    separators.AddVertical(new Vector2(topLeft.X + (CellSize.X * x), topLeft.Y), totalSize.Y, 0.001f);
                }

                for (int y = 0; y < Rows + 1; y++)
                {
                    separators.AddHorizontal(new Vector2(topLeft.X, topLeft.Y + (CellSize.Y * y)), totalSize.X, 0.0015f);
                }

                target.Add(separators);
            }
        }
    }
}
