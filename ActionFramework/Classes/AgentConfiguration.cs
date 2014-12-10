﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ActionFramework.Enum;
using ActionFramework.Interfaces;

namespace ActionFramework.Classes
{
    public class AgentConfiguration : IAgentConfiguration
    {
        
        private string configurationFile = "AgentConfig";
        private string configurationPath = GetDirectoryPath();
        //private DataSourceLocation dataSourceLocation = DataSourceLocation.Local;
        //private DataSourceFormat dataSourceFormat = DataSourceFormat.Simple;
        private bool debug = false;

        public string ConfigurationFile
        {
            get { return configurationFile; }
            set { configurationFile = value; }
        }

        public string ConfigurationPath
        {
            get { return configurationPath; }
        }

        public string AgentId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceDescription { get; set; }
        public string DisplayName { get; set; }
        public string ServerUrl { get; set; }
        public string AgentUrl { get; set; }
        //public string AgentCode { get; set; }
        //public string AgentSecret { get; set; }
        public string ActionFile { get; set; }
        //public bool LogToDisk { get; set; }
        //public bool LogRemote { get; set; }
        public int Interval { get; set; }

        //public DataSourceLocation DataSourceLocation
        //{
        //    get { return dataSourceLocation; }
        //    set { dataSourceLocation = value; }
        //}

        //public DataSourceFormat DataSourceFormat
        //{
        //    get { return dataSourceFormat; }
        //    set { dataSourceFormat = value; }
        //}

        public bool Debug
        {
            get { return debug; }
            set { debug = value; }
        }

        public AgentConfiguration()
        {
            SetConfiguration();
        }

        public AgentConfiguration(string configurationFile)
        {
            this.configurationFile = configurationFile;
            SetConfiguration();
        }

        public bool UpdateSetting(string key, object value)
        {
            try
            {
                string path = Path.Combine(configurationPath, configurationFile + ".xml");
                XDocument xDoc = XDocument.Load(path);

                var setting = (from x in xDoc.Descendants("add")
                               where x.Attribute("key").Value.Equals("LastRunDate")
                               select x).FirstOrDefault();

                setting.Attribute("value").Value = value.ToString();
                xDoc.Save(path);

                ActionFactory.EventLogger(this.ServiceName).Write(EventLogEntryType.Information, string.Format("Agent configuration updated '{0}'. Key '{1}' Value '{2}'", path, key, value), Constants.EventLogId);

                return true;
            }
            catch (Exception ex)
            {
                var exception = new Exception("Could not update agent setting file '" + configurationFile + "' with key '" + key + "' and value '" + value.ToString() + "'. Message '" + ex.Message + "'");
                ActionFactory.EventLogger(this.ServiceName).Write(EventLogEntryType.Error, exception.Message, Constants.EventLogId);
                return false;
            }
        }
        
        private void SetConfiguration()
        {
            string path = Path.Combine(configurationPath, configurationFile + ".xml");
            XDocument xDoc = XDocument.Load(path);

            var settings = (from x in xDoc.Descendants("add") select x);

            AgentId = GetElementValue(settings, "AgentId");
            ServiceName = GetElementValue(settings, "ServiceName");
            ServiceDescription = GetElementValue(settings, "ServiceDescription");
            DisplayName = GetElementValue(settings, "DisplayName");
            //LogRemote = Convert.ToBoolean(GetElementValue(settings, "LogRemote"));
            //LogToDisk = Convert.ToBoolean(GetElementValue(settings, "LogToDisk"));
            //AgentCode = GetElementValue(settings, "AgentCode");
            //AgentSecret = GetElementValue(settings, "AgentSecret");
            ActionFile = GetElementValue(settings, "ActionFile");
            //WebApiUrl = GetElementValue(settings, "WebApiUrl");
            AgentUrl = GetElementValue(settings, "AgentUrl");
            ServerUrl = GetElementValue(settings, "ServerUrl");
            Interval = Convert.ToInt32(GetElementValue(settings, "Interval"));
            //DataSourceLocation = (DataSourceLocation)System.Enum.Parse(typeof(DataSourceLocation), GetElementValue(settings, "DataSourceLocation"));
            //DataSourceFormat = (DataSourceFormat)System.Enum.Parse(typeof(DataSourceFormat), GetElementValue(settings, "DataSourceFormat"));
            Debug = Convert.ToBoolean(GetElementValue(settings, "Debug"));

            ActionFactory.EventLogger(ServiceName).Write(EventLogEntryType.Information, "Loaded agent configuration: " + path, Constants.EventLogId);
        }

        private static string GetElementValue(IEnumerable<XElement> elements, string key)
        {
            return elements.Where(s => s.Attribute("key").Value.Equals(key)).First().Attribute("value").Value;
        }

        private static string GetDirectoryPath()
        {
            string path = Assembly.GetExecutingAssembly().Location;
            FileInfo fileInfo = new FileInfo(path);
            string dir = fileInfo.DirectoryName;
            return dir;
        }


    }
}
