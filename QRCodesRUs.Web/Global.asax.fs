#nowarn "20"
namespace QRCodesRUs.Web

open System
open System.Net.Http
open System.Web
open System.Web.Http
open System.Web.Mvc
open System.Web.Routing
open System.Web.Optimization

open System.Net.Http.Formatting
open QRCodesRUs.Web.Model
open FSharpx

type BundleConfig() =
    static member RegisterBundles (bundles:BundleCollection) =

        bundles.UseCdn <- true

        bundles.Add(ScriptBundle("~/bundles/jquery", "http://ajax.aspnetcdn.com/ajax/jQuery/jquery-2.0.3.min.js")
                        .Include([|"~/Scripts/jquery-{version}.js"|]))

        // Use the development version of Modernizr to develop with and learn from. Then, when you're
        // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
        bundles.Add(ScriptBundle("~/bundles/modernizr").Include([|"~/Scripts/modernizr-*"|]))

        bundles.Add(ScriptBundle("~/bundles/bootstrap").Include(
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/respond.js"))

        bundles.Add(ScriptBundle("~/bundles/app").Include([|"~/Scripts/ReminderFluentInterface.js"|]))

        bundles.Add(StyleBundle("~/Content/css").Include(
                        "~/Content/bootstrap.css",
                        "~/Content/Reminder.css",
                        "~/Content/site.css"))

/// Route for ASP.NET MVC applications
type Route = { 
    controller : string
    action : string
    id : UrlParameter }

type ControllerAndId = {
    controller : string
    id : RouteParameter }

type HomeControllerRoute = {
    hController : string
    hAction : string
}

type ImageRoute = {
    imageController : string
}
    
type QrCodeIdValueProvider(context: System.Web.Http.Controllers.HttpActionContext) =
    let parameters = context.Request.GetRouteData().Values

    interface System.Web.Http.ValueProviders.IValueProvider with
        member x.ContainsPrefix(prefix) =
            parameters.ContainsKey prefix

        member x.GetValue(key) =
            Option.fromTryPattern parameters.TryGetValue key 
         |> Option.map (fun value -> ValueProviders.ValueProviderResult(value, key, Globalization.CultureInfo.CurrentCulture))
         |> Option.getOrDefault

type Global() =
    inherit System.Web.HttpApplication() 

    static member RegisterWebApi(config: HttpConfiguration) =
        // Configure routing
        config.MapHttpAttributeRoutes()
        config.Routes.MapHttpRoute(
            "DefaultApi",
            "api/{controller}/{id}"
        ) |> ignore

        config.Routes.MapHttpRoute(
            "Image",
            "qrcode/{id}",
            { controller = "QrCode"; id = RouteParameter.Optional }
        ) |> ignore
        // Additional Web API settings
        config.Services.Add(typeof<System.Web.Http.ValueProviders.ValueProviderFactory>, 
            { new System.Web.Http.ValueProviders.ValueProviderFactory() with 
                member x.GetValueProvider context = new QrCodeIdValueProvider(context) :> System.Web.Http.ValueProviders.IValueProvider 
            })
        config.Services.Insert(typeof<Http.ModelBinding.ModelBinderProvider>, 0, new System.Web.Http.ModelBinding.Binders.SimpleModelBinderProvider(typeof<QrCodeId>, new QrCodeIdHttpModelBinder()))

        ()

    static member RegisterFilters(filters: GlobalFilterCollection) =
        filters.Add(new HandleErrorAttribute())

    static member RegisterRoutes(routes:RouteCollection) =
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}")

        routes.MapRoute(
            "GetQRCodeById",
            "generatedcodes/{id}",
            { controller = "CodeGeneration"; action = "ItemById"; id = UrlParameter.Optional }
        ) |> ignore

        routes.MapRoute(
            "DefaultWeb",
            "{controller}/{action}/{id}",
            { controller = "Home"; action = "Index"; id = UrlParameter.Optional}
        ) |> ignore
        
        routes.MapRoute(
            "Home",
            "",
            { controller = "Home"; action = "Index"; id = UrlParameter.Optional}
        ) |> ignore

    member x.Application_Start() =
        ModelBinderProviders.BinderProviders.Add(new QrCodeIdModelBinder())
        AreaRegistration.RegisterAllAreas()
        GlobalConfiguration.Configure(Action<_> Global.RegisterWebApi)
        Global.RegisterFilters(GlobalFilters.Filters)
        Global.RegisterRoutes(RouteTable.Routes)
        BundleConfig.RegisterBundles BundleTable.Bundles