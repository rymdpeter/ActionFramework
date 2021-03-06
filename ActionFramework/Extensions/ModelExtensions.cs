﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Classes;
using ActionFramework.Model;
using System.Xml;
using System.Text.RegularExpressions;
using System.Runtime.Serialization;
using System.IO;

namespace ActionFramework.Extensions
{
    public static class ModelExtensions
    {
        public static ApplicationInfo FillActionsAndProperties(this App app)
        {
            Assembly ass = ActionFactory.Compression.DecompressAssembly(app.Assembly);
            ApplicationInfo assemblyInfo = new ApplicationInfo(ass);
            app.Actions = ass.GetActionAndProperties(app);
            return assemblyInfo;
        }

        public static ApplicationInfo FillActionsAndProperties(this App app, byte[] assemblyData)
        {
            Assembly ass = ActionFactory.Compression.LoadAssembly(assemblyData);
            ApplicationInfo assemblyInfo = new ApplicationInfo(ass);
            app.Actions = ass.GetActionAndProperties(app);
            return assemblyInfo;
        }

        public static string FormatCount(this string obj)
        {
            var counters = GetVariables(@"\[(.*?)\]", obj);
            var returnvalue = string.Empty;
            foreach (var c in counters)
            {
                returnvalue += string.Format("{0} {1} ", c.Split(':')[0], c.Split(':')[1]);
            }

            return returnvalue.Trim();
        }

        public static string Serialize(this object obj)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                DataContractSerializer serializer = new DataContractSerializer(obj.GetType());
                serializer.WriteObject(memoryStream, obj);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        public static T Deserialize<T>(this string rawXml)
        {
            using (XmlReader reader = XmlReader.Create(new StringReader(rawXml)))
            {
                DataContractSerializer formatter0 =
                    new DataContractSerializer(typeof(T));
                return (T)formatter0.ReadObject(reader);
            }
        }

        private static List<string> GetVariables(string pattern, string value)
        {
            List<string> variables = new List<string>();
            //var pattern = @"\{(.*?)\}";

            var matches = Regex.Matches(value, pattern);

            foreach (Match m in matches)
            {
                if (!string.IsNullOrEmpty(m.Groups[1].Value))
                    variables.Add(m.Groups[1].Value);
            }

            return variables;
        }
    }
}
