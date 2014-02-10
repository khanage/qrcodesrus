namespace QRCodesRUs.Data

open FSharpx
open System.Data.Entity
open System.Linq

module internal ProductRepositoryModule =
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
        query |> Seq.toList

    let ProductById(id: int) =
        use db = new ProductContext()
        let query = query { for product in db.Products.Include("Category") do
                            where (product.ProductID = id)
                            select product }

        match query.ToList().ToFSharpList() with
        | [first] -> Some first
        | _ -> None

type ProductRepository =
    abstract AllProducts: unit -> Product list
    abstract AllCategories: unit -> Category list
    abstract ProductById: int -> Product option

type EntityFrameworkProductRepository() =
    interface ProductRepository with
        member x.AllProducts() = ProductRepositoryModule.AllProducts()
        member x.AllCategories() = ProductRepositoryModule.AllCategories()
        member x.ProductById id = ProductRepositoryModule.ProductById id