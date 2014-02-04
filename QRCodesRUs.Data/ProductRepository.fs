namespace QRCodesRUs.Data

open System.Collections.Generic
open System.ComponentModel.DataAnnotations
open System.Data.Entity
open FSharpx
open System.Linq
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Quotations.Patterns

type Category() =
    [<Key>]
    member val CategoryID = -1 with get, set
    member val Name = "" with get, set

    abstract Products: ICollection<Product> with get, set

    [<DefaultValue>] val mutable private products: ICollection<Product>
    default x.Products with get() = x.products and set v = x.products <- v

and Product() =
    [<Key>]
    member val ProductID = -1 with get, set
    member val Name = "" with get, set
    member val CategoryID = -1 with get, set

    abstract Category: Category with get, set

    [<DefaultValue>] val mutable private category: Category
    default x.Category with get() = x.category and set v = x.category <- v

type ProductContext() = 
    inherit DbContext("ProductContext")
    
    do Database.SetInitializer<ProductContext>(new CreateDatabaseIfNotExists<ProductContext>())

    [<DefaultValue>] val mutable categories: IDbSet<Category>
    member public x.Categories with get() = x.categories and set v = x.categories <- v

    [<DefaultValue>] val mutable products: IDbSet<Product>
    member public x.Products with get() = x.products and set v = x.products <- v


module ProductRepository =
    let private runQuery (q:Expr<IQueryable<'T>>) = 
      match q with
      | Application(Lambda(builder, Call(Some builder2, miRun, [Quote body])), queryObj) ->
          query.Run(Expr.Cast<Microsoft.FSharp.Linq.QuerySource<'T, IQueryable>>(body))
      | _ -> failwith "Wrong argument"

    do use db = new ProductContext()
       db.Database.CreateIfNotExists() 
       |> Option.ofBool 
       |> Option.iter (fun _ -> 
           // Hello, world
           let products = [
               db.Products.Add(new Product(ProductID = 1, Name = "Doormat", CategoryID = 1))
               db.Products.Add(new Product(ProductID = 2, Name = "Wallframe", CategoryID = 2))
           ]

           let categories = [
               db.Categories.Add(new Category(CategoryID = 1, Name = "Floor items"))
               db.Categories.Add(new Category(CategoryID = 2, Name = "Printed"))
           ]

           db.SaveChanges() |> ignore
       )


    let AllProducts() = 
        use db = new ProductContext()
        let query = query { for product in db.Products do select product } 
        query |> Seq.toList

    let AllCategories() =
        use db = new ProductContext()
        let query = query { for category in db.Categories do select category }
        query.ToListAsync() |> Task.toAsync

    let FindProducts(predicate: Expr<Product -> bool>) =
        use db = new ProductContext()
        let query = <@ query { for product in db.Products do
                                where ((%predicate) product)
                                select product } @> 
                 |> runQuery
        query.ToListAsync() |> Task.toAsync