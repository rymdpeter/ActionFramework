﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ActionFramework.Domain.Interface;
using ActionFramework.Domain.Model.LogModel;
using ActionFramework.Model.Attributes;

namespace ActionFramework.Domain.Model
{
    public partial class Log
    {
        private IList<ErrorLog> errorLogs = new List<ErrorLog>();
        //private IList<FatalLog> fatalLogs = new List<FatalLog>();

        [DapperIgnore]
        public XDocument XmlMessage
        {
            get
            {
                if (!string.IsNullOrEmpty(Message))
                    try
                    {
                        return XDocument.Parse(Message);
                    }
                    catch (Exception ex)
                    {
                        var exception = new Exception("Can not parse log message to xml document in model class 'Log'. Message '" + Message + "' Exception " + ex.Message);
                        throw exception;
                    }
                else
                    throw new Exception("Can not parse log message to xml document in model class 'Log'. Mesasge is null or empty");
            }
        }

        public IList<ErrorLog> ErrorLogs
        {
            get { return errorLogs; }
            set { errorLogs = value; }
        }

        //public IList<FatalLog> FatalLogs
        //{
        //    get { return fatalLogs; }
        //    set { fatalLogs = value; }
        //}

        private IList<InformationLog> informationLogs = new List<InformationLog>();

        public IList<InformationLog> InformationLogs
        {
            get { return informationLogs; }
            set { informationLogs = value; }
        }
        private IList<WarningLog> warningLogs = new List<WarningLog>();

        public IList<WarningLog> WarningLogs
        {
            get { return warningLogs; }
            set { warningLogs = value; }
        }

        public Agent Agent { get; set; }

        public AgentLog AgentLog { get; set; }

        public void Serialize()
        {
            var doc = XmlMessage;
            //foreach (var element in doc.Element("ActionLog").Element("Log").Elements())
            foreach (var element in doc.Elements().First().Elements().First().Elements())
            {
                switch (element.Name.LocalName)
                {
                    case "Error":
                        {
                            var obj = new ErrorLog();
                            FillProperties(obj, element);
                            FillExceptionDetails(obj, element);
                            ErrorLogs.Add(obj);
                            break;
                        }
                    case "Warning":
                        {
                            var obj = new WarningLog();
                            FillProperties(obj, element);
                            FillExceptionDetails(obj, element);
                            WarningLogs.Add(obj);
                            break;
                        }
                    case "Agent":
                        {
                            var obj = new AgentLog();
                            obj.Message = element.Element("Message").Value;
                            obj.Created = Convert.ToDateTime(element.Attribute("Created").Value);

                            if (element.Element("Assembly") != null)
                                obj.AssemblyInfo = element.Element("Assembly").Value;

                            obj.Count = element.Element("Count").Value;

                            if (element.Element("DataSource") != null)
                                obj.DataSource = element.Element("DataSource").Value;

                            if (element.Element("Runtime") != null)
                                obj.Runtime = element.Element("Runtime").Value;

                            AgentLog = obj;
                            break;
                        }
                    case "Information":
                        {
                            var obj = new InformationLog();
                            FillProperties(obj, element);
                            InformationLogs.Add(obj);
                            break;
                        }
                    default:
                        {

                            break;
                        }
                }
            }
        }

        private IActionLog FillProperties(IActionLog obj, XElement element)
        {
            if (element.Element("Message") != null)
                obj.Message = element.Element("Message").Value;

            if (element.Attribute("Created") != null)
                obj.Created = Convert.ToDateTime(element.Attribute("Created").Value);

            if (element.Attribute("ActionId") != null)
                obj.ActionId = element.Attribute("ActionId").Value;

            if (element.Attribute("ActionType") != null)
                obj.ActionType = element.Attribute("ActionType").Value;

            if (element.Attribute("Assembly") != null)
                obj.Assembly = element.Attribute("Assembly").Value;

            return obj;
        }

        private IExceptionLog FillExceptionDetails(IExceptionLog obj, XElement element)
        {
            if (element.Element("Source") != null)
                obj.Source = element.Element("Source").Value;

            if (element.Element("StackTrace") != null)
                obj.StackTrace = element.Element("StackTrace").Value;

            if (element.Element("ExceptionMessage") != null)
                obj.ExceptionMessage = element.Element("ExceptionMessage").Value;

            return obj;
        }

        //public IEnumerable<ErrorLog> ErrorLogs { get; set; }

        //public IEnumerable<InformationLog> InformationLogs { get; set; }

        //public IEnumerable<WarningLog> WarningLogs { get; set; }
    }
}
