namespace QRCodesRUs.Web.App_Start

open System
open System.Web
open Microsoft.Web.Infrastructure.DynamicModuleHelper
open Ninject
open Ninject.Web.Common
open Ninject.Web.Mvc
open Ninject.Syntax
open WebApiContrib.IoC.Ninject
open Microsoft.Practices.ServiceLocation
open System.Web.Http.Dependencies

type NinjectServiceLocator(kernel: IResolutionRoot) =
    inherit ServiceLocatorImplBase()

    override x.DoGetInstance(serviceType: Type, key: string) = 
        kernel.Get(serviceType, key)

    override x.DoGetAllInstances(ofType: Type) =
        kernel.GetAll ofType

type NinjectWebApiDependencyResolver(kernel: IResolutionRoot) =
    interface IDependencyScope with
        member x.GetService(t: Type) = kernel.Get(t, [||])
        member x.GetServices(t: Type) = kernel.GetAll(t, [||])
        member x.Dispose() = ()

    interface IDependencyResolver with
        member x.BeginScope(): IDependencyScope = x :> _

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
        // Set the WebApi dependency resolver
        Http.GlobalConfiguration.Configuration.DependencyResolver <- new NinjectResolver(kernel)

        // Setup service location
        ServiceLocator.SetLocatorProvider(ServiceLocatorProvider(fun () -> new NinjectServiceLocator(kernel :> IResolutionRoot) :> IServiceLocator))

        kernel

    static member Start() =
        DynamicModuleUtility.RegisterModule(typeof<OnePerRequestHttpModule>)
        DynamicModuleUtility.RegisterModule(typeof<NinjectHttpModule>)
        bootstrapper.Initialize(Func<_> NinjectWebCommon.CreateKernel)

    static member Stop() =
        bootstrapper.ShutDown()
        
module NinjectAssemblyLevelAttributes =
    [<assembly: WebActivator.PreApplicationStartMethod(typeof<NinjectWebCommon>, "Start")>]
    [<assembly: WebActivator.ApplicationShutdownMethodAttribute(typeof<NinjectWebCommon>, "Stop")>]
    do ()