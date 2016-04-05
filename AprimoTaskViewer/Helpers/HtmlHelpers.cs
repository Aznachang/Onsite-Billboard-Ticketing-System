using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AprimoTaskViewer.Helpers
{
    public static class HtmlHelpers
    {
        //Specify condition - extension of built-in HtmlHelper method
        public static string Truncate(this HtmlHelper helper, string input, int length)
        {
            //don't truncate
            if (input.Length <= length)
            {
                return input;
            }
            //truncate to 'length' # of characters
            else
            {
                return input.Substring(0, length) + "...";
            }
        }
    }
}