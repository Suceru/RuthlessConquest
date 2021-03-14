using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Game
{
    public class WidgetsList : IEnumerable<Widget>, IEnumerable
    {
        public WidgetsList(ContainerWidget containerWidget)
        {
            this.m_containerWidget = containerWidget;
        }

        public int Count
        {
            get
            {
                return this.m_widgets.Count;
            }
        }

        public Widget this[int index]
        {
            get
            {
                return this.m_widgets[index];
            }
        }

        public void Add(Widget widget)
        {
            this.Insert(this.Count, widget);
        }

        public void Add(params Widget[] widgets)
        {
            this.AddRange(widgets);
        }

        public void AddRange(IEnumerable<Widget> widgets)
        {
            foreach (Widget widget in widgets)
            {
                this.Add(widget);
            }
        }

        public void Insert(int index, Widget widget)
        {
            if (this.m_widgets.Contains(widget))
            {
                throw new InvalidOperationException("Child widget already present in container.");
            }
            if (index < 0 || index > this.m_widgets.Count)
            {
                throw new InvalidOperationException("Widget index out of range.");
            }
            widget.ChangeParent(this.m_containerWidget);
            this.m_widgets.Insert(index, widget);
            this.m_containerWidget.WidgetAdded(widget);
            this.m_version++;
        }

        public void InsertBefore(Widget beforeWidget, Widget widget)
        {
            int num = this.m_widgets.IndexOf(beforeWidget);
            if (num < 0)
            {
                throw new InvalidOperationException("Widget not present in container.");
            }
            this.Insert(num, widget);
        }

        public void InsertAfter(Widget afterWidget, Widget widget)
        {
            int num = this.m_widgets.IndexOf(afterWidget);
            if (num < 0)
            {
                throw new InvalidOperationException("Widget not present in container.");
            }
            this.Insert(num + 1, widget);
        }

        public void Remove(Widget widget)
        {
            int num = this.IndexOf(widget);
            if (num >= 0)
            {
                this.RemoveAt(num);
                return;
            }
            throw new InvalidOperationException("Child widget not present in container.");
        }

        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this.m_widgets.Count)
            {
                throw new InvalidOperationException("Widget index out of range.");
            }
            Widget widget = this.m_widgets[index];
            widget.ChangeParent(null);
            this.m_widgets.RemoveAt(index);
            this.m_containerWidget.WidgetRemoved(widget);
            this.m_version--;
        }

        public void Clear()
        {
            while (this.Count > 0)
            {
                this.RemoveAt(this.Count - 1);
            }
        }

        public int IndexOf(Widget widget)
        {
            return this.m_widgets.IndexOf(widget);
        }

        public bool Contains(Widget widget)
        {
            return this.m_widgets.Contains(widget);
        }

        public Widget Find(string name, Type type, bool throwIfNotFound = true)
        {
            foreach (Widget widget in this.m_widgets)
            {
                if ((name == null || (widget.Name != null && widget.Name == name)) && (type == null || type == widget.GetType() || widget.GetType().GetTypeInfo().IsSubclassOf(type)))
                {
                    return widget;
                }
                ContainerWidget containerWidget = widget as ContainerWidget;
                if (containerWidget != null)
                {
                    Widget widget2 = containerWidget.Children.Find(name, type, false);
                    if (widget2 != null)
                    {
                        return widget2;
                    }
                }
            }
            if (throwIfNotFound)
            {
                throw new Exception(string.Format("Required widget \"{0}\" of type \"{1}\" not found.", name, type));
            }
            return null;
        }

        public Widget Find(string name, bool throwIfNotFound = true)
        {
            return this.Find(name, null, throwIfNotFound);
        }

        public T Find<T>(string name, bool throwIfNotFound = true) where T : class
        {
            return this.Find(name, typeof(T), throwIfNotFound) as T;
        }

        public T Find<T>(bool throwIfNotFound = true) where T : class
        {
            return this.Find(null, typeof(T), throwIfNotFound) as T;
        }

        public WidgetsList.Enumerator GetEnumerator()
        {
            return new WidgetsList.Enumerator(this);
        }

        IEnumerator<Widget> IEnumerable<Widget>.GetEnumerator()
        {
            return new WidgetsList.Enumerator(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new WidgetsList.Enumerator(this);
        }

        private ContainerWidget m_containerWidget;

        private List<Widget> m_widgets = new List<Widget>();

        private int m_version;

        public struct Enumerator : IEnumerator<Widget>, IDisposable, IEnumerator
        {
            internal Enumerator(WidgetsList collection)
            {
                this.m_collection = collection;
                this.m_current = null;
                this.m_index = 0;
                this.m_version = collection.m_version;
            }

            public Widget Current
            {
                get
                {
                    return this.m_current;
                }
            }

            public void Dispose()
            {
            }

            object IEnumerator.Current
            {
                get
                {
                    return this.m_current;
                }
            }

            public bool MoveNext()
            {
                if (this.m_collection.m_version != this.m_version)
                {
                    throw new InvalidOperationException("WidgetsList was modified, enumeration cannot continue.");
                }
                if (this.m_index < this.m_collection.m_widgets.Count)
                {
                    this.m_current = this.m_collection.m_widgets[this.m_index];
                    this.m_index++;
                    return true;
                }
                this.m_current = null;
                return false;
            }

            public void Reset()
            {
                if (this.m_collection.m_version != this.m_version)
                {
                    throw new InvalidOperationException("SortedMultiCollection was modified, enumeration cannot continue.");
                }
                this.m_index = 0;
                this.m_current = null;
            }

            private WidgetsList m_collection;

            private Widget m_current;

            private int m_index;

            private int m_version;
        }
    }
}
