﻿using ActionFramework;
using ActionFramework.Base;
using ActionFramework.Classes;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerApps
{
    public class WriteLog : ActionBase
    {
        /// <summary>
        /// object input [0] = compressed xml [1] = agentId
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public override object Execute(object input)
        {
            JArray obj = (JArray)input;
            var xml = ActionFactory.Compression.DecompressString(obj[0].ToString());
            var agentId = obj[1].ToString();
            
            string file = new GlobalActionFunctions().GetCurrentFormatDateTimeString() + ".xml";
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Agents", agentId.ToString(), "Logs");
            File.WriteAllText(path + file, xml, Encoding.UTF8);

            Log.Info(string.Format("Writing log for agent '{0}'. File '{1}'", agentId, path + file));


            return "Log saved successfully";
        }
    }
}
