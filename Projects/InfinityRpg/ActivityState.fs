﻿namespace InfinityRpg
open System
open OpenTK
open Prime
open Nu
open InfinityRpg

type [<CustomEquality; NoComparison>] NavigationNode =
    { PositionM : Vector2i
      mutable Neighbors : NavigationNode list } // OPTIMIZATION: has to be mutable to be efficiently populated.

    interface NavigationNode IHasNeighbors with
        member this.Neighbors = this.Neighbors :> _ seq

    interface NavigationNode IEquatable with
        member this.Equals that =
            this.PositionM = that.PositionM

    override this.Equals that =
        match that with
        | :? NavigationNode as that -> this.PositionM = that.PositionM
        | _ -> false

    override this.GetHashCode () =
        this.PositionM.GetHashCode ()

type WalkState =
    | WalkFinished
    | WalkContinuing

type WalkDescriptor =
    { WalkDirection : Direction
      WalkOriginM : Vector2i }

    static member nextPositionM walkDescriptor =
        walkDescriptor.WalkOriginM + dtovm walkDescriptor.WalkDirection

type [<StructuralEquality; NoComparison>] NavigationDescriptor =
    { WalkDescriptor : WalkDescriptor
      NavigationPathOpt : NavigationNode list option }

    static member nextPositionM navigationDescriptor =
        WalkDescriptor.nextPositionM navigationDescriptor.WalkDescriptor

    static member nextPositionI navigationDescriptor =
        navigationDescriptor |> NavigationDescriptor.nextPositionM |> vmtovi

    static member nextPosition navigationDescriptor =
        navigationDescriptor |> NavigationDescriptor.nextPositionI |> vitovf

type [<StructuralEquality; NoComparison>] ActionDescriptor =
    { ActionTicks : int64 // an arbitrary number to show a hacky action animation
      ActionTargetPositionMOpt : Vector2i option
      ActionDataName : string }

    static member getActionDirection currentPosition currentDirection actionDescriptor =
        match actionDescriptor.ActionTargetPositionMOpt with
        | Some targetPositionM -> targetPositionM - vftovm currentPosition |> vmtod
        | None -> currentDirection
        
type [<StructuralEquality; NoComparison>] ActivityState =
    | Action of ActionDescriptor
    | Navigation of NavigationDescriptor
    | NoActivity

    static member isActing activity =
        match activity with
        | Action _ -> true
        | Navigation _ | NoActivity -> false

    static member isNotActing activity =
        not (ActivityState.isActing activity)

    static member isNavigating activity =
        match activity with
        | Action _ | NoActivity -> false
        | Navigation _ -> true

    static member isNotNavigating activity =
        not (ActivityState.isNavigating activity)

    static member isNavigatingPath activity =
        match activity with
        | Navigation navigationDescriptor -> Option.isSome navigationDescriptor.NavigationPathOpt
        | Action _ | NoActivity -> false

type [<StructuralEquality; NoComparison>] Turn =
    | ActionTurn of ActionDescriptor
    | NavigationTurn of NavigationDescriptor
    | CancelTurn
    | NoTurn