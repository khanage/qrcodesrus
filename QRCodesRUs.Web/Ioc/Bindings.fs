#nowarn "0020" 
// Disable "need to bind to a result" warning, 
// ninject syntax always returns a result
// but we are calling this for it's side effects
namespace QRCodesRUs.Web

open QRCodesRUs.Web.Model

type Bindings() =
    inherit Ninject.Modules.NinjectModule()

    override x.Load() =
        x.Kernel.Bind<Thing>().To<Implementation>()
        ()


