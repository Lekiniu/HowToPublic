﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace HowToWebApplication.Helpers
{    
        public class CultureAttribute : FilterAttribute, IActionFilter
        {
            public void OnActionExecuted(ActionExecutedContext filterContext)
            {
                string cultureName = null;
                // Получаем куки из контекста, которые могут содержать установленную культуру
                HttpCookie cultureCookie = filterContext.HttpContext.Request.Cookies["lang"];
                if (cultureCookie != null)
                    cultureName = cultureCookie.Value;
                else
                    cultureName = "en-US";

                // Список культур
                List<string> cultures = new List<string>() { "en-US", "ka-GE" };
                if (!cultures.Contains(cultureName))
                {
                    cultureName = "en-US";
                }
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureName);
                Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture(cultureName);
            }

            public void OnActionExecuting(ActionExecutingContext filterContext)
            {
                //не реализован
            }
        }
    }
