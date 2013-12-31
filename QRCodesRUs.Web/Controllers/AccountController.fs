namespace QRCodesRUs.Web.Controllers

open System
open System.Collections.Generic
open System.Linq
open System.Web
open System.Web.Mvc
open System.Web.Mvc.Ajax
open Microsoft.AspNet.Identity
open System.Threading.Tasks
open System.Security.Claims
open System.ComponentModel.DataAnnotations
open System.Web.Routing
open FSharpx
open FSharpx.Task
open System.Web
open Microsoft.Owin.Security
open Microsoft.AspNet.Identity
open System.Web.Routing

module Option =
    let ofNullVal = function 
                    | null -> None
                    | x    -> Some x

    let interopNullVal<'a> (x: 'a) = x :> obj |> ofNullVal |> Option.map (fun o -> o :?> 'a)

    let toBool = function
                 | None -> false
                 | _    -> true
[<AutoOpen>]
module Extensions =
    type RouteValueDictionary with
        static member ofTupledValues (tupledValues: (string * #obj) seq) = 
            RouteValueDictionary(Map<_,_> tupledValues)

    type KeyValuePair<'a,'b> with
        member x.asTuple: ('a * 'b) = x.Key, x.Value

    type Task with
        static member NonGeneric(a: unit -> unit): Task = 
            Task.Run(Action a)
    
type UserId = string
type UserName = string

type HasLogins =
    abstract member Logins: System.Collections.Generic.ICollection<UserLoginInfo> with get

type UserRepository<'user when 'user :> IUser> =
    abstract member Add: 'user -> unit
    abstract member Upsert: 'user -> unit
    abstract member Remove: UserId -> unit
    abstract member ById: UserId -> 'user
    abstract member ByName: UserName -> 'user

type HardcodedUserRepository<'user when 'user :> IUser>() =

    let padlock = ()

    let idStore = System.Collections.Generic.Dictionary<UserId,'user>()
    let nameMap = System.Collections.Generic.Dictionary<UserName, UserId>()

    let add (user: 'user) =
        idStore.[user.Id] <- user
        nameMap.[user.UserName] <- user.Id

    let upsert (user: 'user) =
        if idStore.ContainsKey user.Id then
            let oldName = idStore.[user.Id].UserName
            nameMap.Remove oldName |> ignore
        add user

    let remove (id: UserId) =
        let user = idStore.[id]
        idStore.Remove(user.Id) |> ignore
        nameMap.Remove(user.UserName) |> ignore
        
    let findById (id: UserId) = 
        idStore.[id]

    let findByName (name: UserName) = 
        idStore.[nameMap.[name]]

    interface UserRepository<'user> with
        override x.Add user = 
            lock padlock <| fun () -> add user

        override x.Upsert user = 
            lock padlock <| fun() -> upsert user

        override x.Remove id = 
            lock padlock <| fun () -> remove id

        override x.ById id = 
            lock padlock <| fun () -> findById id

        override x.ByName name = 
            lock padlock <| fun () -> findByName name

type UserManager<'user when 'user :> IUser>(userRepository: UserRepository<'user>) =

    let task = new TaskBuilder()

    let update (user: 'user) = userRepository.Upsert user

    let ifHasLoginsOtherwise (user: 'user) (f: HasLogins -> 'a) (otherwise: 'a) =
        match box user with
        | :? HasLogins as loginContainer -> f loginContainer
        | _ -> otherwise

    let ifHasLogins (user: 'user) (f: HasLogins -> unit) = ifHasLoginsOtherwise user f ()

    member x.CreateAsync(user: 'user) =
        task { userRepository.Add user } :> Task

    member x.UpdateAsync(user: 'user) = task { update user } :> Task

    member x.DeleteAsync(user: 'user) =
        task { userRepository.Remove user.Id } :> Task

    member x.FindByIdAsync(userId: string) =
        task { return userRepository.ById userId }

    member x.FindByNameAsync(userName: string) =
        task { return userRepository.ByName userName }

    member x.AddLoginAsync(user: 'user, login: UserLoginInfo) = 
        task {
            ifHasLogins user (fun loginContainer -> 
                loginContainer.Logins.Add login
                update user
            )
        } :> Task

    member x.RemoveLoginAsync(user: 'user, login: UserLoginInfo) =
        task {
            ifHasLogins user (fun loginContainer ->
                if loginContainer.Logins.Remove login then update user
            )
        } :> Task

    member x.GetLoginsAsync(user: 'user) =
        task {
            let res = ifHasLoginsOtherwise user (fun loginContainer -> loginContainer.Logins.ToList()) (new List<UserLoginInfo>())
            return res :> IList<UserLoginInfo>
        }

    member x.FindAsync(login: UserLoginInfo) =
        Unchecked.defaultof<Task<'user>>
        
    member x.GetClaimsAsync(user: 'user) =
        Unchecked.defaultof<Task<IList<Claim>>>

    member x.AddClaimAsync(user: 'user, claim: Claim) =
        Unchecked.defaultof<Task>

    member x.RemoveClaimAsync(user: 'user, claim: Claim) =
        Unchecked.defaultof<Task>

    member x.SetPasswordHashAsync(user: 'user, passwordHash: string) =
        Unchecked.defaultof<Task>

    member x.GetPasswordHashAsync(user: 'user) =
        Unchecked.defaultof<Task<string>>

    member x.HasPasswordAsync(user: 'user) =
        Unchecked.defaultof<Task<bool>>

    interface IDisposable with
        override x.Dispose() =
            ()

    interface IUserStore<'user> with
        override x.CreateAsync(user: 'user) =
            x.CreateAsync(user)

        override x.UpdateAsync(user: 'user) =
            x.UpdateAsync(user)

        override x.DeleteAsync(user: 'user) =
            x.DeleteAsync(user)

        override x.FindByIdAsync(userId: string) =
            x.FindByIdAsync(userId)

        override x.FindByNameAsync(userName: string) =
            x.FindByNameAsync(userName)

    interface IUserLoginStore<'user> with
        override x.AddLoginAsync(user: 'user, login: UserLoginInfo) = 
            x.AddLoginAsync(user, login)

        override x.RemoveLoginAsync(user: 'user, login: UserLoginInfo) =
            x.RemoveLoginAsync(user, login)

        override x.GetLoginsAsync(user: 'user) =
            x.GetLoginsAsync(user)

        override x.FindAsync(login: UserLoginInfo) =
            x.FindAsync(login)

    interface IUserClaimStore<'user> with
        override x.GetClaimsAsync(user: 'user) =
             x.GetClaimsAsync(user)

        override x.AddClaimAsync(user: 'user, claim: Claim) =
            x.AddClaimAsync(user, claim)

        override x.RemoveClaimAsync(user: 'user, claim: Claim) =
            x.RemoveClaimAsync(user, claim)

    interface IUserPasswordStore<'user> with
        override x.SetPasswordHashAsync(user: 'user, passwordHash: string) =
            x.SetPasswordHashAsync(user, passwordHash)

        override x.GetPasswordHashAsync(user: 'user) =
            x.GetPasswordHashAsync(user)

        override x.HasPasswordAsync(user: 'user) =
            x.HasPasswordAsync(user)
        
type ApplicationUser(id: string) =
    member val UserName = "" with get, set

    member val PasswordHash: Option<string> = None with get, set

    interface IUser with
        member x.Id with get() = id
        member x.UserName with get() = x.UserName and set v = x.UserName <- v

type ExternalLoginConfirmationViewModel() =
    [<Required>][<Display(Name = "User name")>]
    member val UserName = "" with get, set

type ManageUserViewModel() =
    [<Required>]
    [<DataType(DataType.Password)>]
    [<Display(Name = "Current password")>]
    member val OldPassword = "" with get, set
    
    [<Required>]
    [<StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)>]
    [<DataType(DataType.Password)>]
    [<Display(Name = "New password")>]
    member val NewPassword = "" with get, set
    
    [<Required>]
    [<DataType(DataType.Password)>]
    [<Display(Name = "Confirm new password")>]
    [<Compare("NewPassword", ErrorMessage = "The new password and the confirmation password do not match.")>]
    member val ConfirmPassword = "" with get, set

type LoginViewModel() =
    [<Required>]
    [<Display(Name = "User name")>]
    member val UserName = "" with get, set

    [<Required>]
    [<DataType(DataType.Password)>]
    [<Display(Name = "Pasword")>]
    member val Password = "" with get, set

    [<Display(Name = "Remember me?")>]
    member val RememberMe = false with get, set

type RegisterViewModel() =
    [<Required>]
    [<Display(Name = "User name")>]
    member val UserName = "" with get, set
    
    [<Required>]
    [<StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)>]
    [<DataType(DataType.Password)>]
    [<Display(Name = "New password")>]
    member val Password = "" with get, set
    
    [<Required>]
    [<DataType(DataType.Password)>]
    [<Display(Name = "Confirm new password")>]
    [<Compare("NewPassword", ErrorMessage = "The new password and the confirmation password do not match.")>]
    member val ConfirmPassword = "" with get, set


type DissociateResult = { Message : string}

module ChallengeResult =
    let XsrfKey = "XsrfId"

    type ChallengeResult(provider: string, redirectUri: string, ?userId: string) =
        inherit HttpUnauthorizedResult()

        member val LoginProvider = "" with get, set
        member val RedirectUri = "" with get, set
        member val UserId = "" with get, set

        override x.ExecuteResult(context: ControllerContext) =
            let properties = new AuthenticationProperties(RedirectUri = x.RedirectUri)

            if not(String.IsNullOrWhiteSpace x.UserId) then properties.Dictionary.[XsrfKey] <- x.UserId

            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, x.LoginProvider)
         
[<Authorize>]
type AccountController(userManager: UserManager<ApplicationUser>) =
    inherit Controller()

    let task = Task.TaskBuilder()

    member private x.AuthenticationManager with get() = x.HttpContext.GetOwinContext().Authentication

    member private x.SignInAsync(user: ApplicationUser, isPersistent: bool) =
        task {
            x.AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie)
            let! identity = userManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie)
            x.AuthenticationManager.SignIn(new AuthenticationProperties(IsPersistent = isPersistent), identity)
        }

    member private x.GoHome(): Task<ActionResult> = 
        x.RedirectToAction("Home", "Index") :> ActionResult
     |> Task.returnM
        
    member private x.RedirectToLocal(returnUrl: string): Task<ActionResult> =
        if x.Url.IsLocalUrl returnUrl
        then x.Redirect returnUrl :> ActionResult |> Task.returnM
        else x.GoHome()

    member private x.InvalidUsernameOrPassword(model: LoginViewModel): Task<ActionResult> = 
        x.ModelState.AddModelError("", "Invalid username or password.")
        x.View(model) :> ActionResult   
     |> Task.returnM     

    member private x.AddErrors (viewName: string) (result: IdentityResult) (model:obj): Task<ActionResult>  =
        result.Errors |> Seq.iter (fun err -> x.ModelState.AddModelError("", err))
        x.View(viewName, model) :> ActionResult
     |> Task.returnM

    member private x.RedirectToManageWithMessage (message: DissociateResult): Task<ActionResult> =
        x.RedirectToAction("Manage", message :> obj) :> ActionResult
     |> Task.returnM

    member private x.TaskView (model: #obj): Task<ActionResult> = 
        base.View(model :> obj) :> ActionResult 
     |> Task.returnM

    member private x.TaskView (name: string, model: #obj): Task<ActionResult> = 
        base.View(name, model :> obj) :> ActionResult 
     |> Task.returnM

    member private x.TaskAction (actionName: string) =
        base.RedirectToAction actionName :> ActionResult
     |> Task.returnM

    member private x.TaskAction (actionName: string, model: #obj) =
        base.RedirectToAction(actionName, model :> obj) :> ActionResult
     |> Task.returnM

    member x.CurrentUserId with get() = x.User.Identity.GetUserId()
    member x.HasPassword() =
        let user = userManager.FindByIdAsync(x.CurrentUserId).Result
        user |> Option.interopNullVal |> Option.bind (fun u -> u.PasswordHash) |> Option.toBool

    member x.Index() =
        x.View()

    [<AllowAnonymous>]
    member x.Login(returnUrl: string) =
        x.ViewData.["ReturnUrl"] <- returnUrl
        x.View() :> ActionResult

    [<HttpPost>]
    [<AllowAnonymous>]
    [<ValidateAntiForgeryToken>]
    member x.Login(model: LoginViewModel, returnUrl: string): Task<ActionResult> =
        if not x.ModelState.IsValid
        then x.TaskView model
        else task {
            let! mUser = userManager.FindAsync(model.UserName, model.Password)
            match mUser |> Option.interopNullVal with
            | None -> return! x.InvalidUsernameOrPassword model
            | Some(user) ->
                do! x.SignInAsync(user, model.RememberMe)
                return! x.RedirectToLocal returnUrl
        }
          
    [<AllowAnonymous>]
    member x.Register() =
        x.View()  

    [<HttpPost>]
    [<AllowAnonymous>]
    [<ValidateAntiForgeryToken>]
    member x.Register(model: RegisterViewModel) =
        let createUser name = new ApplicationUser("", UserName = name)

        task {
        if not x.ModelState.IsValid then return! x.TaskView(model)
        else
            let user = createUser model.UserName
            let! result = userManager.CreateAsync(user, model.Password)

            if not result.Succeeded then 
                return! x.AddErrors "Register" result model
            else do! x.SignInAsync(user, false)
                 return! x.GoHome()                    
        } 

    [<HttpPost>]
    [<ValidateAntiForgeryToken>]
    member x.Disassociate(loginProvider: string, providerKey: string) =
        x.RedirectToAction("Manage", { Message = "blahdy"} :> obj) :> ActionResult
        
    [<HttpPost>]
    [<ValidateAntiForgeryToken>]
    member x.Manage(model: ManageUserViewModel) =
        let hasPassword = x.HasPassword()

        x.ViewData.["HasLocalPassword"] <- hasPassword
        x.ViewData.["ReturnUrl"] <- x.Url.Action("Manage")

        task {
            if hasPassword then 
                if x.ModelState.IsValid then
                    let! result = userManager.ChangePasswordAsync(x.CurrentUserId, model.OldPassword, model.NewPassword)

                    if result.Succeeded then return! x.RedirectToManageWithMessage { Message = "change password sucess"}
                    else return! x.AddErrors "Manage" result model

                else return! x.TaskView model
            else 
                x.ModelState.["OldPassword"] |> Option.interopNullVal |> Option.iter (fun s -> s.Errors.Clear())

                if x.ModelState.IsValid then 
                    let! result = userManager.AddPasswordAsync(x.CurrentUserId, model.NewPassword)

                    if result.Succeeded then return! x.RedirectToManageWithMessage { Message = "set password success" }
                    else return! x.AddErrors "Manage" result model

                else return! x.TaskView model
        } 

    [<HttpPost>]
    [<AllowAnonymous>]
    [<ValidateAntiForgeryToken>]
    member x.ExternalLogin (provider: string, returnUrl: string) =
        let callbackRouteValues = RouteValueDictionary.ofTupledValues(["ReturnUrl", returnUrl])
        let callbackUrl =  x.Url.Action("ExternalLoginCallback", "Account", callbackRouteValues)

        new ChallengeResult.ChallengeResult(provider, callbackUrl) :> ActionResult
        
    [<AllowAnonymous>]
    member x.ExternalLoginCallback(returnUrl: string) =
        task {
            let! loginInfo = x.AuthenticationManager.GetExternalLoginInfoAsync()

            match loginInfo |> Option.interopNullVal with
            | None -> return! x.TaskAction "Login"
            | _ ->
                let! user = userManager.FindAsync loginInfo.Login
                match user |> Option.interopNullVal with
                | None -> 
                    x.ViewData.["ReturnUrl"] <- returnUrl
                    x.ViewData.["LoginProvider"] <- loginInfo.Login.LoginProvider
                    return! x.TaskView("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel(UserName = loginInfo.DefaultUserName))
                | _ ->
                    do! x.SignInAsync(user, false)
                    return! x.RedirectToLocal returnUrl
        }

    [<HttpPost>]
    [<ValidateAntiForgeryToken>]
    member x.LinkLogin(provider: string) =
        let callbackUrl = x.Url.Action("LinkLoginCallback", "Account")
        new ChallengeResult.ChallengeResult(provider, callbackUrl, x.User.Identity.GetUserId()) :> ActionResult

    member x.LinkLoginCallback() =
        task {
            let! loginInfo = x.AuthenticationManager.GetExternalLoginInfoAsync(ChallengeResult.XsrfKey, x.CurrentUserId)

            match loginInfo |> Option.interopNullVal with
            | None -> return! x.TaskAction ("Manage", { Message = "Error" })
            | _    -> 
                let! result = userManager.AddLoginAsync(x.CurrentUserId, loginInfo.Login)

                if result.Succeeded then return! x.TaskAction "Manage"
                else return! x.TaskAction ("Manage", { Message = "Error"})
        }

    [<HttpPost>]
    [<AllowAnonymous>]
    [<ValidateAntiForgeryToken>]
    member x.ExternalLoginConfirmation(model: ExternalLoginConfirmationViewModel, returnUrl: string) =
        task{
            if x.User.Identity.IsAuthenticated then return! x.TaskAction "Manage"
            else
                if not x.ModelState.IsValid then
                    x.ViewData.["ReturnUrl"] <- returnUrl
                    return! x.TaskView(model)
                else 
                    let! info = x.AuthenticationManager.GetExternalLoginInfoAsync()
                    match info |> Option.interopNullVal with
                    | None -> return! x.TaskView("ExternalLoginFailure", ())
                    | _ ->
                        let user = ApplicationUser("", UserName = model.UserName)
                        let! result = userManager.CreateAsync user
                        if result.Succeeded then
                            do! x.SignInAsync(user, false)
                            return! x.RedirectToLocal returnUrl
                        else
                            return! x.AddErrors "ExternalLoginConfirmation" result model
        }

    [<HttpPost>]
    [<ValidateAntiForgeryToken>]
    member x.LogOff() =
        x.AuthenticationManager.SignOut()
        x.RedirectToAction("Index", "Home") :> ActionResult
    
    [<AllowAnonymous>]
    member x.ExternalLoginFailure() =
        x.View()

    [<ChildActionOnly>]
    member x.RemoveAccountList() = 
        let linkedAccounts = userManager.GetLogins(x.CurrentUserId)
        x.ViewData.["ShowRemoveButton"] <- x.HasPassword() || linkedAccounts.Count > 1
        x.PartialView("_RemoveAccountPartial", linkedAccounts)