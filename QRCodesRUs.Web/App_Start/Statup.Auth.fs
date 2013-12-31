namespace QRCodesRUs.Web.App_Start

open Owin
open Microsoft.AspNet.Identity
open Microsoft.Owin
open Microsoft.Owin.Security.Cookies

type StartupAuth() =
    member x.Configuration(app: IAppBuilder) =
        
        app.UseCookieAuthentication <| new CookieAuthenticationOptions(
            AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie, 
            LoginPath = new PathString("/Account/Login")
        ) |> ignore

        app.UseGoogleAuthentication() |> ignore

        app.UseExternalSignInCookie DefaultAuthenticationTypes.ExternalCookie


module AuthAssemblyLevelAttributes =
    [<assembly: OwinStartupAttribute(typeof<StartupAuth>)>]
    ()