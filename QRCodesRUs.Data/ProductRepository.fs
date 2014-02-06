namespace QRCodesRUs.Data

open System
open System.Collections.Generic
open System.ComponentModel.DataAnnotations
open System.Data.Entity
open FSharpx
open System.Linq
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns
open Microsoft.FSharp.Linq.RuntimeHelpers.LeafExpressionConverter

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


module ProductRepository =
    do use db = new ProductContext()
       if db.Database.CreateIfNotExists() then
           let floorItems = new Category(Name = "Floor items")
           let printedItems = new Category(Name = "Printed")
           [
               db.Products.Add(new Product(Name = "Doormat", Price = 39.95m, Category = floorItems, ImageName = "doormat.jpg"))
               db.Products.Add(new Product(Name = "Wallframe", Price = 59.95m, Category = printedItems, ImageName = "photo-frame.jpg"))
               db.Products.Add(new Product(Name = "Sticker", Price = 9.95m, Category = printedItems, ImageName = "sticker.jpg"))
           ] |> ignore

           db.SaveChanges() |> ignore

    let AllProducts() = 
        use db = new ProductContext()
        let query = query { for product in db.Products.Include("Category") do select product } 

        query |> Seq.toList

    let AllCategories() =
        use db = new ProductContext()
        let query = query { for category in db.Categories do select category }
        query.ToListAsync() |> Task.toAsync

    let ProductById(id: int) =
        use db = new ProductContext()
        let query = query { for product in db.Products.Include("Category") do
                            where (product.ProductID = id)
                            select product }

        match query.ToList().ToFSharpList() with
        | [first] -> Some first
        | _ -> None