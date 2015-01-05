using System;
using System.Collections.Generic;
using System.Linq;
using Moriyama.Runtime.Models;

namespace Moriyama.Runtime.Extension
{
    public static class ContentExtension
    {
        public static string GetValue(this RuntimeContentModel model, string key)
        {
            if (model == null || model.Content == null || !model.Content.ContainsKey(key))
                return string.Empty;

            return model.Content[key].ToString();
        }

        public static T GetValue<T>(this RuntimeContentModel model, string key)
        {
            var v = model.GetValue(key);

            if (v == null || !CanChangeType(v, typeof (T)))
                return (T) Activator.CreateInstance(typeof(T));

            return (T) Convert.ChangeType(v, typeof(T));
        }

        private static bool CanChangeType(object value, Type conversionType)
        {
            if (conversionType == null)
            {
                return false;
            }

            if (value == null)
            {
                return false;
            }

            var convertible = value as IConvertible;

            return convertible != null;
        }

        public static bool HideInNavigation(this RuntimeContentModel model)
        {
            return model.GetValue<bool>("HideInNavigation");
        }

        public static RuntimeContentModel Home(this RuntimeContentModel model)
        {
            return RuntimeContext.Instance.ContentService.Home(model);
        }

        public static IEnumerable<RuntimeContentModel> TopNavigation(this RuntimeContentModel model)
        {
            return RuntimeContext.Instance.ContentService.TopNavigation(model);
        }

        public static IEnumerable<RuntimeContentModel> Descendants(this RuntimeContentModel model)
        {
            return RuntimeContext.Instance.ContentService.Descendants(model);
        }
        
        public static IEnumerable<RuntimeContentModel> Descendants(this RuntimeContentModel model, string type)
        {
            return model.Descendants().Where(item => item.Type == type);
        }

        public static IEnumerable<RuntimeContentModel> Children(this RuntimeContentModel model)
        {
            return RuntimeContext.Instance.ContentService.Children(model);
        }

        public static string View(this RuntimeContentModel model)
        {
            return "~/Views/" + model.Template + ".cshtml";
        }
    }
}
