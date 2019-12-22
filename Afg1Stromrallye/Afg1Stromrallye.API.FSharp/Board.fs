namespace Afg1Stromrallye.API.FSharp

open System.Collections.Generic

module Board =

    type Board =
        val Width : int
        val Height : int
        val BatteryLocations : HashSet<Vector2Int>
        val Distances : Map<Vector2Int * Vector2Int, int>

    let build width height batteries =
        