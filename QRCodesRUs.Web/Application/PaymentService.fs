namespace QRCodesRUs.Web

open System
open QRCodesRUs.Web.Model

type PaymentService =
    abstract ChargeAmmount : CardDetails -> Decimal -> bool
        
type DummyPaymentService() =
    interface PaymentService with
        member x.ChargeAmmount _ _ = true   