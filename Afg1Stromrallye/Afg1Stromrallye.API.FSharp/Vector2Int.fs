namespace Afg1Stromrallye.API.FSharp

[<Struct>]
type Vector2Int =
    val X : int
    val Y : int
    new(x, y) = { X = x; Y = y}
    
    static member inline (+) (lhs : Vector2Int, rhs : Vector2Int) : Vector2Int = Vector2Int(lhs.X + rhs.X, lhs.Y + rhs.Y)
    static member inline (-) (lhs : Vector2Int, rhs : Vector2Int) : Vector2Int = Vector2Int(lhs.X - rhs.X, lhs.Y - rhs.Y)