namespace QRCodesRUs.Web.App_Start

open System
open System.Web
open Microsoft.Web.Infrastructure.DynamicModuleHelper
open Ninject
open Ninject.Web.Common
open Ninject.Web.Mvc
open Ninject.Syntax

type NinjectWebCommon() =
    static let bootstrapper = new Bootstrapper()
    
    static member RegisterServices (kernel: IKernel) = 
        kernel.Load(System.Reflection.Assembly.GetExecutingAssembly())

    static member CreateKernel() =
        let kernel = new StandardKernel() :> IKernel
        
        kernel.Bind<Func<IKernel>>().ToMethod(
            // Yuck! This needs a Func<IContext,Func<IKernel>>
            Func<_,_> (fun ctx -> Func<_> (fun () -> (new Bootstrapper()).Kernel))
        ) |> ignore

        kernel.Bind<IHttpModule>().To<HttpApplicationInitializationHttpModule>() |> ignore

        NinjectWebCommon.RegisterServices(kernel)

        // Set the MVC dependency resolver
        Mvc.DependencyResolver.SetResolver(new NinjectDependencyResolver(kernel :> IResolutionRoot))

        kernel

    static member Start() =
        DynamicModuleUtility.RegisterModule(typeof<OnePerRequestHttpModule>)
        DynamicModuleUtility.RegisterModule(typeof<NinjectHttpModule>)
        bootstrapper.Initialize(Func<_> NinjectWebCommon.CreateKernel)

    static member Stop() =
        bootstrapper.ShutDown()
        
module AssemblyDummy =
    [<assembly: WebActivator.PreApplicationStartMethod(typeof<NinjectWebCommon>, "Start")>]
    [<assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof<NinjectWebCommon>, "Stop")>]
    do ()