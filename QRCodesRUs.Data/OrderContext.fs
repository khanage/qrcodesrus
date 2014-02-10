namespace QRCodesRUs.Data

open System
open System.ComponentModel.DataAnnotations
open System.Data.Entity

type Order() =
    [<Key>]
    member val OrderID = Guid.Empty with get, set
    [<Required>]
    member val ProductID = -1 with get, set
    [<Required>]
    member val UserId = "" with get, set
    [<Required>]
    member val Address = "" with get, set

    member val EstimatedDelivery = DateTime.Now.AddDays(10.) with get, set
    
    [<DefaultValue>] val mutable private product : Product
    abstract Product : Product with get, set
    default x.Product with get() = x.product and set v = x.product <- v

type OrderContext() = 
    inherit DbContext("OrderContext")
    
    do Database.SetInitializer<OrderContext>(new CreateDatabaseIfNotExists<OrderContext>())

    [<DefaultValue>] val mutable orders: IDbSet<Order>
    member public x.Orders with get() = x.orders and set v = x.orders <- v
