#nowarn "0020" 
// Disable "need to bind to a result" warning, 
// ninject syntax always returns a result
// but we are calling this for it's side effects
namespace QRCodesRUs.Web

open QRCodesRUs.Web.Model
open QRCodesRUs.Web.Controllers
open QRCodesRUs.CodeGeneration


open Microsoft.AspNet.Identity

type Bindings() =
    inherit Ninject.Modules.NinjectModule()

    override x.Load() =
        x.Kernel.Bind<QrCodeCreator>().To<QrCodeCreatorImplementation>()
        x.Kernel.Bind<QrCodeRepository>().To<InMemoryQrCodeRepository>().InSingletonScope()
        x.Kernel.Bind<IUserStore<ApplicationUser>>().To<QRCodesRUs.Web.Controllers.UserManager<ApplicationUser>>().InSingletonScope()
        x.Kernel.Bind<UserRepository<ApplicationUser>>().To<HardcodedUserRepository<ApplicationUser>>().InSingletonScope()
        ()


