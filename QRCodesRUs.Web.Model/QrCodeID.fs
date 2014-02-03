namespace QRCodesRUs.Web.Model

open System

type QrCodeId(id: Guid) =
    member val Id = id with get, set

    interface IEquatable<QrCodeId> with
        member x.Equals(other) = x.Id.Equals(other.Id)


