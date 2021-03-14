using System;
using System.Collections.Generic;
using Engine;

namespace Game
{
    internal static class DialogsManager
    {
        public static ReadOnlyList<Dialog> Dialogs
        {
            get
            {
                return new ReadOnlyList<Dialog>(m_dialogs);
            }
        }

        public static bool HasDialogs(ContainerWidget parentWidget)
        {
            if (parentWidget == null)
            {
                parentWidget = (ScreensManager.CurrentScreen ?? WidgetsManager.RootWidget);
            }
            using (List<Dialog>.Enumerator enumerator = m_dialogs.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    if (enumerator.Current.ParentWidget == parentWidget)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static void ShowDialog(ContainerWidget parentWidget, Dialog dialog, bool useAnimation = true)
        {
            ShowDialog(parentWidget, dialog, useAnimation, new Color(0, 0, 0, 192));
        }

        public static void ShowDialog(ContainerWidget parentWidget, Dialog dialog, bool useAnimation, Color backgroundCoverColor)
        {
            Dispatcher.Dispatch(delegate
            {
                if (!m_dialogs.Contains(dialog))
                {
                    if (parentWidget == null)
                    {
                        parentWidget = (ScreensManager.CurrentScreen ?? WidgetsManager.RootWidget);
                    }
                    dialog.IsUpdateEnabled = true;
                    dialog.WidgetsHierarchyInput = null;
                    m_dialogs.Add(dialog);
                    DialogsManager.AnimationData animationData = new DialogsManager.AnimationData
                    {
                        Direction = 1,
                        Factor = useAnimation ? 0 : 1
                    };
                    animationData.CoverWidget.Rectangle.FillColor = backgroundCoverColor;
                    m_animationData[dialog] = animationData;
                    parentWidget.Children.Add(animationData.CoverWidget);
                    if (dialog.ParentWidget != null)
                    {
                        dialog.ParentWidget.Children.Remove(dialog);
                    }
                    parentWidget.Children.Add(dialog);
                    UpdateDialog(dialog, animationData);
                    dialog.Input.Clear();
                    dialog.DialogShown();
                }
            }, false);
        }

        public static void HideDialog(Dialog dialog, bool useAnimation = true)
        {
            Dispatcher.Dispatch(delegate
            {
                if (m_dialogs.Contains(dialog))
                {
                    dialog.DialogHidden();
                    dialog.IsUpdateEnabled = false;
                    dialog.ParentWidget.Input.Clear();
                    dialog.WidgetsHierarchyInput = new WidgetInput(WidgetInputDevice.None);
                    m_dialogs.Remove(dialog);
                    m_animationData[dialog].Direction = -1;
                    if (!useAnimation)
                    {
                        m_animationData[dialog].Factor = 0f;
                    }
                }
            }, false);
        }

        public static void HideAllDialogs(ContainerWidget parentWidget = null, bool useAnimation = true)
        {
            foreach (Dialog dialog in m_dialogs.ToArray())
            {
                if (parentWidget == null || dialog.IsChildWidgetOf(parentWidget))
                {
                    HideDialog(dialog, useAnimation);
                }
            }
        }

        public static void Update()
        {
            foreach (KeyValuePair<Dialog, DialogsManager.AnimationData> keyValuePair in m_animationData)
            {
                Dialog key = keyValuePair.Key;
                DialogsManager.AnimationData value = keyValuePair.Value;
                if (value.Direction > 0)
                {
                    value.Factor = MathUtils.Min(value.Factor + 6f * Time.FrameDuration, 1f);
                }
                else if (value.Direction < 0)
                {
                    value.Factor = MathUtils.Max(value.Factor - 6f * Time.FrameDuration, 0f);
                    if (value.Factor <= 0f)
                    {
                        m_toRemove.Add(key);
                    }
                }
                UpdateDialog(key, value);
            }
            foreach (Dialog dialog in m_toRemove)
            {
                DialogsManager.AnimationData animationData = m_animationData[dialog];
                m_animationData.Remove(dialog);
                dialog.ParentWidget.Children.Remove(dialog);
                animationData.CoverWidget.ParentWidget.Children.Remove(animationData.CoverWidget);
            }
            m_toRemove.Clear();
        }

        private static void UpdateDialog(Dialog dialog, DialogsManager.AnimationData animationData)
        {
            if (animationData.Factor < 1f)
            {
                float factor = animationData.Factor;
                float num = 0.75f + 0.25f * MathUtils.Pow(animationData.Factor, 0.25f);
                dialog.RenderTransform = Matrix.CreateTranslation(-dialog.ActualSize.X / 2f, -dialog.ActualSize.Y / 2f, 0f) * Matrix.CreateScale(num, num, 1f) * Matrix.CreateTranslation(dialog.ActualSize.X / 2f, dialog.ActualSize.Y / 2f, 0f);
                dialog.ColorTransform = Color.White * factor;
                animationData.CoverWidget.Rectangle.ColorTransform = Color.White * factor;
                return;
            }
            dialog.RenderTransform = Matrix.Identity;
            dialog.ColorTransform = Color.White;
            animationData.CoverWidget.Rectangle.ColorTransform = Color.White;
        }

        private static Dictionary<Dialog, DialogsManager.AnimationData> m_animationData = new Dictionary<Dialog, DialogsManager.AnimationData>();

        private static List<Dialog> m_dialogs = new List<Dialog>();

        private static List<Dialog> m_toRemove = new List<Dialog>();

        private class AnimationData
        {
            public float Factor;

            public int Direction;

            public DialogCoverWidget CoverWidget = new DialogCoverWidget();
        }
    }
}
