using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;

namespace Lab1.UI.TagHelpers
{
    [HtmlTargetElement("pager", TagStructure = TagStructure.NormalOrSelfClosing)]
    public class PagerTagHelper : TagHelper
    {
        private readonly LinkGenerator _linkGenerator;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PagerTagHelper(LinkGenerator linkGenerator, IHttpContextAccessor httpContextAccessor)
        {
            _linkGenerator = linkGenerator;
            _httpContextAccessor = httpContextAccessor;
        }

        [HtmlAttributeName("current-page")]
        public int CurrentPage { get; set; } = 1;

        [HtmlAttributeName("total-pages")]
        public int TotalPages { get; set; } = 1;

        [HtmlAttributeName("admin")]
        public bool Admin { get; set; }

        [HtmlAttributeName("asp-controller")]
        public string? Controller { get; set; }

        [HtmlAttributeName("asp-action")]
        public string? Action { get; set; }

        [HtmlAttributeName("asp-page")]
        public string? Page { get; set; }

        [HtmlAttributeName(DictionaryAttributePrefix = "asp-route-")]
        public Dictionary<string, string?> RouteValues { get; set; } = new(StringComparer.OrdinalIgnoreCase);

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (TotalPages <= 1)
            {
                output.SuppressOutput();
                return;
            }

            var httpContext = _httpContextAccessor.HttpContext
                ?? throw new InvalidOperationException("HTTP context is not available.");

            var effectiveController = Controller ?? httpContext.GetRouteData().Values["controller"]?.ToString();
            var effectiveAction = Action ?? httpContext.GetRouteData().Values["action"]?.ToString();
            var effectivePage = Page ?? httpContext.GetRouteData().Values["page"]?.ToString();

            output.TagName = "nav";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Attributes.SetAttribute("aria-label", "Page navigation");

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination justify-content-center");

            ul.InnerHtml.AppendHtml(CreatePageItem(httpContext, effectiveController, effectiveAction, effectivePage,
                CurrentPage - 1, CurrentPage <= 1, isActive: false, label: "<i class=\"bi bi-chevron-left\"></i>"));

            for (var pageNumber = 1; pageNumber <= TotalPages; pageNumber++)
            {
                ul.InnerHtml.AppendHtml(CreatePageItem(httpContext, effectiveController, effectiveAction, effectivePage,
                    pageNumber, disabled: false, isActive: pageNumber == CurrentPage, label: pageNumber.ToString()));
            }

            ul.InnerHtml.AppendHtml(CreatePageItem(httpContext, effectiveController, effectiveAction, effectivePage,
                CurrentPage + 1, CurrentPage >= TotalPages, isActive: false, label: "<i class=\"bi bi-chevron-right\"></i>"));

            output.Content.AppendHtml(ul);
        }

        private TagBuilder CreatePageItem(HttpContext httpContext,
            string? controller,
            string? action,
            string? page,
            int targetPage,
            bool disabled,
            bool isActive,
            string label)
        {
            var li = new TagBuilder("li");
            li.AddCssClass("page-item");
            if (disabled)
            {
                li.AddCssClass("disabled");
            }
            if (isActive)
            {
                li.AddCssClass("active");
            }

            var url = disabled
                ? "#"
                : BuildUrl(httpContext, controller, action, page, targetPage);

            var anchor = new TagBuilder("a");
            anchor.AddCssClass("page-link");
            anchor.Attributes["href"] = url ?? "#";

            if (disabled)
            {
                anchor.Attributes["tabindex"] = "-1";
                anchor.Attributes["aria-disabled"] = "true";
            }
            anchor.InnerHtml.AppendHtml(label);

            li.InnerHtml.AppendHtml(anchor);
            return li;
        }

        private string? BuildUrl(HttpContext httpContext,
            string? controller,
            string? action,
            string? page,
            int targetPage)
        {
            var routeValues = new RouteValueDictionary();

            foreach (var kvp in RouteValues)
            {
                if (!string.IsNullOrWhiteSpace(kvp.Key) && !string.IsNullOrWhiteSpace(kvp.Value))
                {
                    routeValues[kvp.Key] = kvp.Value;
                }
            }

            routeValues["pageNo"] = targetPage;

            if (Admin)
            {
                return _linkGenerator.GetPathByPage(
                    httpContext,
                    page ?? httpContext.Request.Path,
                    values: routeValues);
            }

            return _linkGenerator.GetPathByAction(
                httpContext,
                action,
                controller,
                routeValues);
        }
    }
}

