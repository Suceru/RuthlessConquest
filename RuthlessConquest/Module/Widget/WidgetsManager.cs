using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Engine;
using Engine.Content;
using Engine.Graphics;
using Engine.Input;
using Engine.Serialization;

namespace Game
{
    public static class WidgetsManager
    {
        public static float Scale
        {
            get
            {
                return m_scale;
            }
        }

        public static ContainerWidget RootWidget
        {
            get
            {
                return m_rootWidget;
            }
        }

        public static bool IsUpdateEnabled { get; set; } = true;

        public static void Initialize()
        {
            Colors.InitializeColorScheme();
            ContentCache.Set("Colors/High", Colors.High);
            ContentCache.Set("Colors/HighDim", Colors.HighDim);
            ContentCache.Set("Colors/HighDark", Colors.HighDark);
            ContentCache.Set("Colors/Fore", Colors.Fore);
            ContentCache.Set("Colors/ForeDim", Colors.ForeDim);
            ContentCache.Set("Colors/ForeDisabled", Colors.ForeDisabled);
            ContentCache.Set("Colors/ForeDark", Colors.ForeDark);
            ContentCache.Set("Colors/Back", Colors.Back);
            ContentCache.Set("Colors/Panel", Colors.Panel);
            m_rootWidget = new CanvasWidget();
            m_rootWidget.WidgetsHierarchyInput = new WidgetInput(WidgetInputDevice.All);
        }

        public static void Update()
        {
            if (IsUpdateEnabled)
            {
                bool isMouseVisible = false;
                UpdateWidgetsHierarchy(m_rootWidget, null, ref isMouseVisible);
                Mouse.IsMouseVisible = isMouseVisible;
            }
        }

        public static void Draw()
        {
            float approximateWindowInches = ScreenResolutionManager.ApproximateWindowInches;
            float num;
            if (approximateWindowInches < 9f)
            {
                num = 800f;
            }
            else if (approximateWindowInches < 16.9f)
            {
                num = 1000f;
            }
            else
            {
                num = 1200f;
            }
            num /= WidgetsDebugScale;
            Vector2 vector = new Vector2(Display.Viewport.Width, Display.Viewport.Height);
            m_scale = vector.X / num;
            Vector2 vector2 = new Vector2(num, num / vector.X * vector.Y);
            float num2 = num * 9f / 16f;
            if (vector.Y / m_scale < num2)
            {
                m_scale = vector.Y / num2;
                vector2 = new Vector2(num2 / vector.Y * vector.X, num2);
            }
            m_rootWidget.LayoutTransform = Matrix.CreateScale(Scale, Scale, 1f);
            if (SettingsManager.UpsideDownLayout)
            {
                m_rootWidget.LayoutTransform *= new Matrix(-1f, 0f, 0f, 0f, 0f, -1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
            }
            m_rootWidget.Measure(vector2);
            m_rootWidget.Arrange(Vector2.Zero, vector2);
            if (DrawWidgets)
            {
                DrawWidgetsHierarchy(m_rootWidget);
            }
            PrimitivesRenderer3D.Flush(Matrix.Identity, true, int.MaxValue);
            PrimitivesRenderer2D.Flush(true, int.MaxValue);
            CursorPrimitivesRenderer2D.Flush(true, int.MaxValue);
        }

        public static Widget HitTest(Vector2 point, Func<Widget, bool> predicate)
        {
            return HitTestWidgetsHierarchy(m_rootWidget, point, predicate);
        }

        public static Widget HitTest(Vector2 point)
        {
            return HitTest(point, null);
        }

        public static Widget LoadWidget(object eventsTarget, XElement node, ContainerWidget parentWidget)
        {
            if (node.Name.LocalName.Contains("."))
            {
                throw new NotImplementedException("Node property specification not implemented.");
            }
            Widget widget = Activator.CreateInstance(FindTypeFromXmlName(node.Name.LocalName, node.Name.NamespaceName)) as Widget;
            if (widget == null)
            {
                throw new Exception(string.Format("Type \"{0}\" is not a Widget.", node.Name.LocalName));
            }
            if (parentWidget != null)
            {
                parentWidget.Children.Add(widget);
            }
            LoadWidgetContents(widget, eventsTarget, node);
            return widget;
        }

        public static void LoadWidgetContents(Widget widget, object eventsTarget, XElement node)
        {
            LoadWidgetProperties(widget, eventsTarget, node);
            LoadWidgetChildren(widget, eventsTarget, node);
        }

        public static void LoadWidgetProperties(Widget widget, object eventsTarget, XElement node)
        {
            IEnumerable<PropertyInfo> runtimeProperties = widget.GetType().GetRuntimeProperties();
            using (IEnumerator<XAttribute> enumerator = node.Attributes().GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    XAttribute attribute = enumerator.Current;
                    if (!attribute.IsNamespaceDeclaration && !attribute.Name.LocalName.StartsWith("_"))
                    {
                        if (attribute.Name.LocalName.Contains('.'))
                        {
                            string[] array = attribute.Name.LocalName.Split(new char[]
                            {
                                '.'
                            });
                            if (array.Length != 2)
                            {
                                throw new InvalidOperationException(string.Format("Attached property reference must have form \"TypeName.PropertyName\", property \"{0}\" in widget of type \"{1}\".", attribute.Name.LocalName, widget.GetType().FullName));
                            }
                            Type type = FindTypeFromXmlName(array[0], (attribute.Name.NamespaceName != string.Empty) ? attribute.Name.NamespaceName : node.Name.NamespaceName);
                            string setterName = "Set" + array[1];
                            MethodInfo methodInfo = type.GetRuntimeMethods().FirstOrDefault((MethodInfo mi) => mi.Name == setterName && mi.IsPublic && mi.IsStatic);
                            if (!(methodInfo != null))
                            {
                                throw new InvalidOperationException(string.Format("Attached property public static setter method \"{0}\" not found, property \"{1}\" in widget of type \"{2}\".", setterName, attribute.Name.LocalName, widget.GetType().FullName));
                            }
                            ParameterInfo[] parameters = methodInfo.GetParameters();
                            if (parameters.Length != 2 || !(parameters[0].ParameterType == typeof(Widget)))
                            {
                                throw new InvalidOperationException(string.Format("Attached property setter method must take 2 parameters and first one must be of type Widget, property \"{0}\" in widget of type \"{1}\".", attribute.Name.LocalName, widget.GetType().FullName));
                            }
                            object obj = HumanReadableConverter.ConvertFromString(parameters[1].ParameterType, attribute.Value);
                            methodInfo.Invoke(null, new object[]
                            {
                                widget,
                                obj
                            });
                        }
                        else
                        {
                            PropertyInfo propertyInfo = (from pi in runtimeProperties
                                                         where pi.Name == attribute.Name.LocalName
                                                         select pi).FirstOrDefault<PropertyInfo>();
                            if (!(propertyInfo != null))
                            {
                                throw new InvalidOperationException(string.Format("Property \"{0}\" not found in widget of type \"{1}\".", attribute.Name.LocalName, widget.GetType().FullName));
                            }
                            if (attribute.Value.StartsWith("{") && attribute.Value.EndsWith("}"))
                            {
                                string name = attribute.Value.Substring(1, attribute.Value.Length - 2);
                                object value = ContentCache.Get(propertyInfo.PropertyType, name, true);
                                propertyInfo.SetValue(widget, value, null);
                            }
                            else
                            {
                                object obj2 = HumanReadableConverter.ConvertFromString(propertyInfo.PropertyType, attribute.Value);
                                if (propertyInfo.PropertyType == typeof(string))
                                {
                                    obj2 = ((string)obj2).Replace("\\n", "\n").Replace("\\t", "\t");
                                }
                                propertyInfo.SetValue(widget, obj2, null);
                            }
                        }
                    }
                }
            }
        }

        public static void LoadWidgetChildren(Widget widget, object eventsTarget, XElement node)
        {
            if (node.HasElements)
            {
                ContainerWidget containerWidget = widget as ContainerWidget;
                if (containerWidget == null)
                {
                    throw new Exception(string.Format("Type \"{0}\" is not a ContainerWidget, but it contains child widgets.", node.Name.LocalName));
                }
                foreach (XElement node2 in node.Elements())
                {
                    if (IsNodeIncludedOnCurrentPlatform(node2))
                    {
                        Widget widget2 = null;
                        string attributeValue = XmlUtils.GetAttributeValue<string>(node2, "Name", null);
                        if (attributeValue != null)
                        {
                            widget2 = containerWidget.Children.Find(attributeValue, false);
                        }
                        if (widget2 != null)
                        {
                            LoadWidgetContents(widget2, eventsTarget, node2);
                        }
                        else
                        {
                            LoadWidget(eventsTarget, node2, containerWidget);
                        }
                    }
                }
            }
        }

        public static bool IsWidgetEnabled(Widget widget)
        {
            return widget.IsEnabled && (widget.ParentWidget == null || IsWidgetEnabled(widget.ParentWidget));
        }

        public static bool IsWidgetVisible(Widget widget)
        {
            return widget.IsVisible && (widget.ParentWidget == null || IsWidgetVisible(widget.ParentWidget));
        }

        public static bool IsNodeIncludedOnCurrentPlatform(XElement node)
        {
            string attributeValue = XmlUtils.GetAttributeValue<string>(node, "_IncludePlatforms", null);
            string attributeValue2 = XmlUtils.GetAttributeValue<string>(node, "_ExcludePlatforms", null);
            if (attributeValue != null && attributeValue2 == null)
            {
                if (attributeValue.Split(new char[]
                {
                    ' '
                }).Contains(VersionsManager.Platform.ToString()))
                {
                    return true;
                }
            }
            else
            {
                if (attributeValue2 == null || attributeValue != null)
                {
                    return true;
                }
                if (!attributeValue2.Split(new char[]
                {
                    ' '
                }).Contains(VersionsManager.Platform.ToString()))
                {
                    return true;
                }
            }
            return false;
        }

        private static void UpdateWidgetsHierarchy(Widget widget, WidgetInput input, ref bool isMouseCursorVisible)
        {
            if (widget.IsVisible && widget.IsEnabled && widget.IsUpdateEnabled)
            {
                if (widget.WidgetsHierarchyInput != null)
                {
                    widget.WidgetsHierarchyInput.Update();
                    isMouseCursorVisible |= widget.WidgetsHierarchyInput.IsMouseCursorVisible;
                }
                ContainerWidget containerWidget = widget as ContainerWidget;
                if (containerWidget != null)
                {
                    WidgetsList children = containerWidget.Children;
                    for (int i = children.Count - 1; i >= 0; i--)
                    {
                        if (i < children.Count)
                        {
                            UpdateWidgetsHierarchy(children[i], input, ref isMouseCursorVisible);
                        }
                    }
                }
                widget.Update();
            }
        }

        private static void CollateDrawItems(Widget widget, List<WidgetsManager.DrawItem> list, Rectangle scissorRectangle)
        {
            if (widget.IsVisible)
            {
                bool flag = widget.GlobalBounds.Intersection(new BoundingRectangle(scissorRectangle.Left, scissorRectangle.Top, scissorRectangle.Right, scissorRectangle.Bottom));
                Rectangle? scissorRectangle2 = null;
                if (widget.ClampToBounds && flag)
                {
                    scissorRectangle2 = new Rectangle?(scissorRectangle);
                    int num = (int)MathUtils.Floor(widget.GlobalBounds.Min.X - 0.5f);
                    int num2 = (int)MathUtils.Floor(widget.GlobalBounds.Min.Y - 0.5f);
                    int num3 = (int)MathUtils.Ceiling(widget.GlobalBounds.Max.X - 0.5f);
                    int num4 = (int)MathUtils.Ceiling(widget.GlobalBounds.Max.Y - 0.5f);
                    scissorRectangle = Rectangle.Intersection(new Rectangle(num, num2, num3 - num, num4 - num2), scissorRectangle2.Value);
                    WidgetsManager.DrawItem drawItemFromCache = GetDrawItemFromCache();
                    drawItemFromCache.ScissorRectangle = new Rectangle?(scissorRectangle);
                    list.Add(drawItemFromCache);
                }
                if (widget.IsDrawRequired && flag)
                {
                    WidgetsManager.DrawItem drawItemFromCache2 = GetDrawItemFromCache();
                    drawItemFromCache2.Widget = widget;
                    list.Add(drawItemFromCache2);
                }
                if (flag || !widget.ClampToBounds)
                {
                    ContainerWidget containerWidget = widget as ContainerWidget;
                    if (containerWidget != null)
                    {
                        foreach (Widget widget2 in containerWidget.Children)
                        {
                            CollateDrawItems(widget2, list, scissorRectangle);
                        }
                    }
                }
                if (widget.IsOverdrawRequired && flag)
                {
                    WidgetsManager.DrawItem drawItemFromCache3 = GetDrawItemFromCache();
                    drawItemFromCache3.Widget = widget;
                    drawItemFromCache3.IsOverdraw = true;
                    list.Add(drawItemFromCache3);
                }
                if (scissorRectangle2 != null)
                {
                    WidgetsManager.DrawItem drawItemFromCache4 = GetDrawItemFromCache();
                    drawItemFromCache4.ScissorRectangle = scissorRectangle2;
                    list.Add(drawItemFromCache4);
                }
            }
        }

        private static void DrawWidgetsHierarchy(Widget widget)
        {
            ResetDrawItemsCache();
            m_drawItems.Clear();
            CollateDrawItems(m_rootWidget, m_drawItems, Display.ScissorRectangle);
            for (int i = 0; i < m_drawItems.Count; i++)
            {
                WidgetsManager.DrawItem drawItem = m_drawItems[i];
                for (int j = i + 1; j < m_drawItems.Count; j++)
                {
                    WidgetsManager.DrawItem drawItem2 = m_drawItems[j];
                    if (drawItem.ScissorRectangle != null || drawItem2.ScissorRectangle != null)
                    {
                        drawItem2.Layer = MathUtils.Max(drawItem2.Layer, drawItem.Layer + 1);
                    }
                    else if (Widget.TestOverlap(drawItem.Widget, drawItem2.Widget))
                    {
                        drawItem2.Layer = MathUtils.Max(drawItem2.Layer, drawItem.Layer + 1);
                    }
                }
            }
            m_drawItems.Sort();
            Rectangle scissorRectangle = Display.ScissorRectangle;
            int num = 0;
            foreach (WidgetsManager.DrawItem drawItem3 in m_drawItems)
            {
                if (LayersLimit >= 0 && drawItem3.Layer > LayersLimit)
                {
                    break;
                }
                if (drawItem3.Layer != num)
                {
                    num = drawItem3.Layer;
                    PrimitivesRenderer3D.Flush(Matrix.Identity, true, int.MaxValue);
                    PrimitivesRenderer2D.Flush(true, int.MaxValue);
                }
                if (drawItem3.Widget != null)
                {
                    if (drawItem3.IsOverdraw)
                    {
                        drawItem3.Widget.Overdraw();
                    }
                    else
                    {
                        drawItem3.Widget.Draw();
                    }
                }
                else
                {
                    Display.ScissorRectangle = Rectangle.Intersection(scissorRectangle, drawItem3.ScissorRectangle.Value);
                }
            }
            PrimitivesRenderer3D.Flush(Matrix.Identity, true, int.MaxValue);
            PrimitivesRenderer2D.Flush(true, int.MaxValue);
            Display.ScissorRectangle = scissorRectangle;
        }

        private static Widget HitTestWidgetsHierarchy(Widget widget, Vector2 point, Func<Widget, bool> predicate)
        {
            if (widget.IsVisible && (!widget.ClampToBounds || widget.HitTest(point)))
            {
                ContainerWidget containerWidget = widget as ContainerWidget;
                if (containerWidget != null)
                {
                    WidgetsList children = containerWidget.Children;
                    for (int i = children.Count - 1; i >= 0; i--)
                    {
                        Widget widget2 = HitTestWidgetsHierarchy(children[i], point, predicate);
                        if (widget2 != null)
                        {
                            return widget2;
                        }
                    }
                }
                if (widget.IsHitTestVisible && widget.HitTest(point) && (predicate == null || predicate(widget)))
                {
                    return widget;
                }
            }
            return null;
        }

        private static WidgetsManager.DrawItem GetDrawItemFromCache()
        {
            while (m_drawItemsCacheIndex >= m_drawItemsCache.Count)
            {
                m_drawItemsCache.Add(new WidgetsManager.DrawItem());
            }
            WidgetsManager.DrawItem drawItem = m_drawItemsCache[m_drawItemsCacheIndex++];
            drawItem.Layer = 0;
            drawItem.IsOverdraw = false;
            drawItem.Widget = null;
            drawItem.ScissorRectangle = null;
            return drawItem;
        }

        private static void ResetDrawItemsCache()
        {
            foreach (WidgetsManager.DrawItem drawItem in m_drawItemsCache)
            {
                drawItem.Widget = null;
            }
            m_drawItemsCacheIndex = 0;
        }

        private static Type FindTypeFromXmlName(string name, string namespaceName)
        {
            if (string.IsNullOrEmpty(namespaceName))
            {
                throw new InvalidOperationException("Namespace must be specified when creating types in XML.");
            }
            Uri uri = new Uri(namespaceName);
            if (uri.Scheme == "runtime-namespace")
            {
                return TypeCache.FindType(uri.AbsolutePath + "." + name, false, true);
            }
            throw new InvalidOperationException("Unknown uri scheme when loading widget. Scheme must be runtime-namespace.");
        }

        private static CanvasWidget m_rootWidget = new CanvasWidget();

        private static float m_scale = 1f;

        private static List<WidgetsManager.DrawItem> m_drawItems = new List<WidgetsManager.DrawItem>();

        private static List<WidgetsManager.DrawItem> m_drawItemsCache = new List<WidgetsManager.DrawItem>();

        private static int m_drawItemsCacheIndex;

        public static int LayersLimit = -1;

        public static bool DrawWidgets = true;

        public static bool DrawWidgetBounds = false;

        public static bool LogWidgetsHierarchy = false;

        public static float WidgetsDebugScale = 1f;

        public static PrimitivesRenderer2D PrimitivesRenderer2D = new PrimitivesRenderer2D();

        public static PrimitivesRenderer3D PrimitivesRenderer3D = new PrimitivesRenderer3D();

        public static PrimitivesRenderer2D CursorPrimitivesRenderer2D = new PrimitivesRenderer2D();

        private class DrawItem : IComparable<WidgetsManager.DrawItem>
        {
            public int CompareTo(WidgetsManager.DrawItem other)
            {
                return this.Layer - other.Layer;
            }

            public int Layer;

            public bool IsOverdraw;

            public Widget Widget;

            public Rectangle? ScissorRectangle;
        }
    }
}
