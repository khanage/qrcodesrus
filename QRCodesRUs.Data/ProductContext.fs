namespace QRCodesRUs.Data

open System.Collections.Generic
open System.ComponentModel.DataAnnotations
open System.Data.Entity

type Category() =
    [<Key>]
    member val CategoryID = -1 with get, set
    member val Name = "" with get, set
    
    [<DefaultValue>] val mutable private products: ICollection<Product>
    abstract Products: ICollection<Product> with get, set
    default x.Products with get() = x.products and set v = x.products <- v

and Product() =
    [<Key>]
    member val ProductID = -1 with get, set
    member val Name = "" with get, set
    member val CategoryID = -1 with get, set
    member val ImageName = "" with get, set
    member val Price = 0.0m with get, set

    [<DefaultValue>] val mutable private category: Category
    abstract Category: Category with get, set
    default x.Category with get() = x.category and set v = x.category <- v

type ProductContext() = 
    inherit DbContext("ProductContext")
    
    do Database.SetInitializer<ProductContext>(new CreateDatabaseIfNotExists<ProductContext>())

    [<DefaultValue>] val mutable categories: IDbSet<Category>
    member public x.Categories with get() = x.categories and set v = x.categories <- v

    [<DefaultValue>] val mutable products: IDbSet<Product>
    member public x.Products with get() = x.products and set v = x.products <- v