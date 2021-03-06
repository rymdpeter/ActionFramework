﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ActionFramework.Interfaces;
using ActionFramework.Entities;
using System.Xml.Linq;
using ActionFramework.Enum;
using ActionFramework.Classes;
using ActionFramework.Context;
using ActionFramework.Domain.Model;
using ActionFramework.Extensions;
using ActionFramework.Model;

namespace ActionFramework.DataSource
{
    public class JSonDataSource : IActionDataSource
    {
        private List<App> apps = new List<App>();

        public List<App> Apps
        {
            get { return apps; }
            set { apps = value; }
        }
      
        public virtual List<IAction> GetActions(ActionStatus status)
        {
            //List<XElement> actionElements = ActionHelper.GetActionElements(status, xmlFilePath);
            //List<XElement> settingElements = ActionHelper.GetSettingElements(xmlFilePath);

            List<IAction> actionList = new List<IAction>();
            Type[] actionTypes = ActionHelper.GetActionTypes(settingElements);

            foreach (XElement e in actionElements)
            {
                try
                {
                    Type actionType = ActionHelper.GetActionType(actionTypes, ActionHelper.GetActionProperty(e, "Type").Value);
                    IAction action = (IAction)Activator.CreateInstance(actionType);
                    action.Id = e.Attribute("Id").Value;
                    action.Type = actionType;
                    action.Assembly = actionType.Assembly;
                    action.Description = e.Attribute("Description").Value;
                    action.AddDynamicProperties(ActionHelper.GetActionProperties(e));
                    action.AddDynamicProperties(ActionHelper.GetSettingProperties(settingElements));

                    if (e.Attribute("BreakOnError") != null)
                        action.BreakOnError = bool.Parse(e.Attribute("BreakOnError").Value);

                    if (e.Attribute("ClientExecute") != null)
                        action.ClientExecute = Convert.ToBoolean(e.Attribute("ClientExecute").Value);

                    //action.DataSource = this;
                    actionList.Add(action);
                }
                catch
                {
                    throw new Exception("GetActions caused an exception in XmlDataSource class. Assembly could not be found for type: '" + e.Attribute("Type").Value + "'");
                    //LogContext.Current().Add(LogType.Error, ex);
                    //throw ex;
                }
            }

            //resolve the properties after list is done, due to that properties values might depend on execution of others
            foreach (var a in actionList)
            {
                //a.DataSource.ActionList = actionList;
                a.ResolveStaticProperties();
            }

            return actionList;
        }

        public List<ActionProperty> GlobalSettings
        {
            get {
                throw new NotImplementedException();
                //List<XElement> settingElements = ActionHelper.GetSettingElements(xmlFilePath);
                //return ActionHelper.GetSettingProperties(settingElements);
            }
        }

        public void FillActions(IActionList actionList, ActionStatus status)
        {
            List<XElement> actionElements = ActionHelper.GetActionElements(status, xmlFilePath);
            List<XElement> settingElements = ActionHelper.GetSettingElements(xmlFilePath);
            Type[] actionTypes = ActionHelper.GetActionTypes(settingElements);

            foreach (XElement e in actionElements)
            {
                try
                {
                    Type actionType = ActionHelper.GetActionType(actionTypes, ActionHelper.GetActionProperty(e, "Type").Value);
                    IAction action = (IAction)Activator.CreateInstance(actionType);
                    action.Id = e.Attribute("Id").Value;
                    action.Type = actionType;
                    action.Assembly = actionType.Assembly;
                    action.Description = e.Attribute("Description").Value;
                    action.AddDynamicProperties(ActionHelper.GetActionProperties(e));
                    action.AddDynamicProperties(ActionHelper.GetSettingProperties(settingElements));

                    if (e.Attribute("BreakOnError") != null)
                        action.BreakOnError = bool.Parse(e.Attribute("BreakOnError").Value);

                    if (e.Attribute("ClientExecute") != null)
                        action.ClientExecute = Convert.ToBoolean(e.Attribute("ClientExecute").Value);

                    //action.DataSource = this;
                    actionList.Add(action);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            //resolve the properties after list is done, due to that properties values might depend on execution of others
            foreach (var a in actionList)
            {
                a.ActionList = actionList;
                a.ResolveStaticProperties();
            }
        }
    }
}
