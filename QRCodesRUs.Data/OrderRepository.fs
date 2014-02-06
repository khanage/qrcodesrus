namespace QRCodesRUs.Data

open FSharpx
open System
open System.Linq

module internal OrderRepositoryModule =
    let saveOrder (order: Order) =
        use db = new OrderContext()
        db.Orders.Add order |> ignore
    let loadOrder (orderId: Guid) =
        use db = new OrderContext()
        let query = query { for order in db.Orders do
                            where (order.OrderID = orderId)
                            select order }
        match query.ToList().ToFSharpList() with
        | [first] -> Some first
        | _ -> None

type OrderRepository =
    abstract SaveOrder : Order -> unit
    abstract LoadOrder : Guid -> Order option

type EntityFrameworkOrderRepository() =
    interface OrderRepository with
        member x.SaveOrder order = OrderRepositoryModule.saveOrder order        
        member x.LoadOrder orderId = OrderRepositoryModule.loadOrder orderId

