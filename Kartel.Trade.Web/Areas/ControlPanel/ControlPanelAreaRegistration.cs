﻿using System.Web.Mvc;

namespace Kartel.Trade.Web.Areas.ControlPanel
{
    public class ControlPanelAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "ControlPanel";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "ControlPanel_default",
                "ControlPanel/{controller}/{action}/{id}",
                new { action = "Index", Controller = "Root", id = UrlParameter.Optional }
            );
        }
    }
}
