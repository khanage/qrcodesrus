namespace QRCodesRUs.Web

open Ninject.Activation

open QRCodesRUs.Data
open Microsoft.AspNet.Identity
open Microsoft.AspNet.Identity.EntityFramework

type UserManagerProvider() =
    inherit Provider<UserManager<ApplicationUser>>()

    override x.CreateInstance ctx = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()))

