using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace ExtensionTool.Mvc
{
    public static class HtmlHelperExtension
    {
        public static MvcHtmlString TextBoxFor<TModel, TProperty>(
            this HtmlHelper<TModel> htmlHelper,
            Expression<Func<TModel, TProperty>> expression,
            bool isReadOnly,
            object htmlAttributes)
        {
            IDictionary<string, object> htmlAttributes1 = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);

            if (isReadOnly && !htmlAttributes1.ContainsKey("readonly"))
                htmlAttributes1.Add("readonly","readonly");

            return htmlHelper.TextBoxFor(expression, null, htmlAttributes1);
        }
    }
}
