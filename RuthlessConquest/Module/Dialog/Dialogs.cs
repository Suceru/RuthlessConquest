using System;
using System.Xml.Linq;
using Engine.Content;

namespace Game
{
    public static class Dialogs
    {
        /// <summary>
        /// 文本对话框
        /// </summary>
        public static XElement TextBoxDialog
        {
            get
            {
                return _TextBoxDialog.Get();
            }
        }
        /// <summary>
        /// 繁忙对话框
        /// </summary>
        public static XElement BusyDialog
        {
            get
            {
                return _BusyDialog.Get();
            }
        }
        /// <summary>
        /// 列表选择对话框
        /// </summary>
        public static XElement ListSelectionDialog
        {
            get
            {
                return _ListSelectionDialog.Get();
            }
        }

        private static ContentCache.CachedItemWrapper<XElement> _TextBoxDialog = new ContentCache.CachedItemWrapper<XElement>("Dialogs/TextBoxDialog");

        private static ContentCache.CachedItemWrapper<XElement> _BusyDialog = new ContentCache.CachedItemWrapper<XElement>("Dialogs/BusyDialog");

        private static ContentCache.CachedItemWrapper<XElement> _ListSelectionDialog = new ContentCache.CachedItemWrapper<XElement>("Dialogs/ListSelectionDialog");
    }
}
