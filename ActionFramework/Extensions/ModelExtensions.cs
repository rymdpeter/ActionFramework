﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ActionFramework.Classes;
using ActionFramework.Domain.Model;

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
    }
}
