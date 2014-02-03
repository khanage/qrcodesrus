namespace QRCodesRUs.Web.Model

open System

type QrCodeId(id: Guid) =
    member val Id = id with get, set
    member x.Equals(other: QrCodeId) =  x.Id.Equals(other.Id)

    override x.GetHashCode() = x.Id.GetHashCode()
    override x.Equals (o: obj) =
        match o with
        | :? QrCodeId as qrcode -> (x :> IEquatable<QrCodeId>).Equals(qrcode)
        | _ -> false

    interface IEquatable<QrCodeId> with
        member x.Equals(other) = x.Equals(other)