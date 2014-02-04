// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

#I @"D:\Dropbox\masters\508\QRCodesRUs\packages\EntityFramework.6.0.2\lib\net45"

#r "System.Data.Entity.dll"
#r "FSharp.Data.TypeProviders.dll"
#r "System.Data.Linq.dll"
#r "System.Configuration.dll"

#r "System.ComponentModel.DataAnnotations.dll"

#r "EntityFramework.dll"
#r "EntityFramework.SqlServer.dll"

open System.Collections.Generic
open System.ComponentModel.DataAnnotations
open System.Data.Entity
open System.Configuration

//#load "Library1.fs"

module SchoolEDM =
    open System.Data.Linq
    open System.Data.Entity
    open Microsoft.FSharp.Data.TypeProviders

    type private EntityConnection = SqlEntityConnection< ConnectionString="Server=(local);Initial Catalog=QRCodesRUs;User ID=qrcodes;password=rus;MultipleActiveResultSets=true", Pluralize = true>

    let private context = EntityConnection.GetDataContext()

    let run() = query { for user in context.AspNetUsers do 
                            select user.UserName }


SchoolEDM.run() |> Seq.iter (printfn "%s")

// Define your library scripting code here

let path = __SOURCE_DIRECTORY__ + "/app.config"
let fileMap = ConfigurationFileMap(path) 
let config = ConfigurationManager.OpenMappedMachineConfiguration(fileMap) 

ConfigurationManager.ConnectionStrings.Add(new ConnectionStringSettings("ProductConext", "Server=(local);Initial Catalog=QRCodesRUs;User ID=qrcodes;password=rus;MultipleActiveResultSets=true"))