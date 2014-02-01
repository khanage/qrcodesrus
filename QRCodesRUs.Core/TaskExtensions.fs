namespace QRCodesRUs.Core

module Task =
    open FSharpx
    open System
    open System.Threading.Tasks

    let toAsync_ (t: Task): Async<unit> =
        let abegin (cb: AsyncCallback, state: obj) : IAsyncResult = 
            match cb with
            | null -> upcast t
            | cb -> 
                t.ContinueWith(fun _ -> cb.Invoke t) |> ignore
                upcast t
        Async.FromBeginEnd(abegin, konst ())