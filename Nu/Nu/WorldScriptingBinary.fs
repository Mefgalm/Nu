﻿// Nu Game Engine.
// Copyright (C) Bryan Edds, 2012-2016.

namespace Nu
open System
open OpenTK
open Prime
open Nu
open Nu.Scripting

[<AutoOpen>]
module WorldScriptingBinary =

    module Scripting =

        type [<NoEquality; NoComparison>] BinaryFns =
            { Bool : bool -> bool -> SymbolOrigin option -> Expr
              Int : int -> int -> SymbolOrigin option -> Expr
              Int64 : int64 -> int64 -> SymbolOrigin option -> Expr
              Single : single -> single -> SymbolOrigin option -> Expr
              Double : double -> double -> SymbolOrigin option -> Expr
              Vector2 : Vector2 -> Vector2 -> SymbolOrigin option -> Expr
              String : string -> string -> SymbolOrigin option -> Expr
              Keyword : string -> string -> SymbolOrigin option -> Expr
              Tuple : Expr array -> Expr array -> SymbolOrigin option -> Expr
              Keyphrase : string -> Expr array -> string -> Expr array -> SymbolOrigin option -> Expr
              Codata : Codata -> Codata -> SymbolOrigin option -> Expr
              List : Expr list -> Expr list -> SymbolOrigin option -> Expr
              Ring : Expr Set -> Expr Set -> SymbolOrigin option -> Expr
              Table : Map<Expr, Expr> -> Map<Expr, Expr> -> SymbolOrigin option -> Expr }

        let EqFns =
            { Bool = fun left right _ -> Bool (left = right)
              Int = fun left right _ -> Bool (left = right)
              Int64 = fun left right _ -> Bool (left = right)
              Single = fun left right _ -> Bool (left = right)
              Double = fun left right _ -> Bool (left = right)
              Vector2 = fun left right _ -> Bool (left = right)
              String = fun left right _ -> Bool (left = right)
              Keyword = fun left right _ -> Bool (left = right)
              Tuple = fun left right _ -> Bool (left = right)
              Keyphrase = fun wordLeft phraseLeft wordRight phraseRight _ -> Bool ((wordLeft, phraseLeft) = (wordRight, phraseRight))
              Codata = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Eq"], "Cannot determine equality of codata.", originOpt)
              List = fun left right _ -> Bool (left = right)
              Ring = fun left right _ -> Bool (left = right)
              Table = fun left right _ -> Bool (left = right) }

        let NotEqFns =
            { Bool = fun left right _ -> Bool (left <> right)
              Int = fun left right _ -> Bool (left <> right)
              Int64 = fun left right _ -> Bool (left <> right)
              Single = fun left right _ -> Bool (left <> right)
              Double = fun left right _ -> Bool (left <> right)
              Vector2 = fun left right _ -> Bool (left <> right)
              String = fun left right _ -> Bool (left <> right)
              Keyword = fun left right _ -> Bool (left <> right)
              Tuple = fun left right _ -> Bool (left <> right)
              Keyphrase = fun wordLeft phraseLeft wordRight phraseRight _ -> Bool ((wordLeft, phraseLeft) <> (wordRight, phraseRight))
              Codata = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "NotEq"], "Cannot determine inequality of codata.", originOpt)
              List = fun left right _ -> Bool (left <> right)
              Ring = fun left right _ -> Bool (left <> right)
              Table = fun left right _ -> Bool (left <> right) }

        let LtFns =
            { Bool = fun left right _ -> Bool (left < right)
              Int = fun left right _ -> Bool (left < right)
              Int64 = fun left right _ -> Bool (left < right)
              Single = fun left right _ -> Bool (left < right)
              Double = fun left right _ -> Bool (left < right)
              Vector2 = fun left right _ -> Bool (left.LengthSquared < right.LengthSquared)
              String = fun left right _ -> Bool (left < right)
              Keyword = fun left right _ -> Bool (left < right)
              Tuple = fun left right _ -> Bool (left < right)
              Keyphrase = fun wordLeft phraseLeft wordRight phraseRight _ -> Bool ((wordLeft, phraseLeft) < (wordRight, phraseRight))
              Codata = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Lt"], "Cannot compare codata.", originOpt)
              List = fun left right _ -> Bool (left < right)
              Ring = fun left right _ -> Bool (left < right)
              Table = fun left right _ -> Bool (left < right) }

        let GtFns =
            { Bool = fun left right _ -> Bool (left > right)
              Int = fun left right _ -> Bool (left > right)
              Int64 = fun left right _ -> Bool (left > right)
              Single = fun left right _ -> Bool (left > right)
              Double = fun left right _ -> Bool (left > right)
              Vector2 = fun left right _ -> Bool (left.LengthSquared > right.LengthSquared)
              String = fun left right _ -> Bool (left > right)
              Keyword = fun left right _ -> Bool (left > right)
              Tuple = fun left right _ -> Bool (left > right)
              Keyphrase = fun wordLeft phraseLeft wordRight phraseRight _ -> Bool ((wordLeft, phraseLeft) > (wordRight, phraseRight))
              Codata = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Gt"], "Cannot compare codata.", originOpt)
              List = fun left right _ -> Bool (left > right)
              Ring = fun left right _ -> Bool (left > right)
              Table = fun left right _ -> Bool (left > right) }

        let LtEqFns =
            { Bool = fun left right _ -> Bool (left <= right)
              Int = fun left right _ -> Bool (left <= right)
              Int64 = fun left right _ -> Bool (left <= right)
              Single = fun left right _ -> Bool (left <= right)
              Double = fun left right _ -> Bool (left <= right)
              Vector2 = fun left right _ -> Bool (left.LengthSquared <= right.LengthSquared)
              String = fun left right _ -> Bool (left <= right)
              Keyword = fun left right _ -> Bool (left <= right)
              Tuple = fun left right _ -> Bool (left <= right)
              Keyphrase = fun wordLeft phraseLeft wordRight phraseRight _ -> Bool ((wordLeft, phraseLeft) <= (wordRight, phraseRight))
              Codata = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "LtEq"], "Cannot compare codata.", originOpt)
              List = fun left right _ -> Bool (left <= right)
              Ring = fun left right _ -> Bool (left <= right)
              Table = fun left right _ -> Bool (left <= right) }

        let GtEqFns =
            { Bool = fun left right _ -> Bool (left >= right)
              Int = fun left right _ -> Bool (left >= right)
              Int64 = fun left right _ -> Bool (left >= right)
              Single = fun left right _ -> Bool (left >= right)
              Double = fun left right _ -> Bool (left >= right)
              Vector2 = fun left right _ -> Bool (left.LengthSquared >= right.LengthSquared)
              String = fun left right _ -> Bool (left >= right)
              Keyword = fun left right _ -> Bool (left >= right)
              Tuple = fun left right _ -> Bool (left >= right)
              Keyphrase = fun wordLeft phraseLeft wordRight phraseRight _ -> Bool ((wordLeft, phraseLeft) >= (wordRight, phraseRight))
              Codata = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "GtEq"], "Cannot compare codata.", originOpt)
              List = fun left right _ -> Bool (left >= right)
              Ring = fun left right _ -> Bool (left >= right)
              Table = fun left right _ -> Bool (left >= right) }

        let AddFns =
            { Bool = fun left right _ -> Bool (if left && right then false elif left then true elif right then true else false)
              Int = fun left right _ -> Int (left + right)
              Int64 = fun left right _ -> Int64 (left + right)
              Single = fun left right _ -> Single (left + right)
              Double = fun left right _ -> Double (left + right)
              Vector2 = fun left right _ -> Vector2 (left + right)
              String = fun left right _ -> String (left + right)
              Keyword = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Add"], "Cannot add keywords.", originOpt)
              Tuple = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Add"], "Cannot add tuples.", originOpt)
              Keyphrase =
                fun wordLeft phraseLeft wordRight phraseRight originOpt ->
                    if wordLeft = wordRight
                    then Keyphrase (wordLeft, Array.append phraseLeft phraseRight)
                    else Violation (["InvalidArgumentType"; "Binary"; "Add"], "Cannot add keywords.", originOpt)
              Codata = fun left right _ -> Codata (Add (left, right))
              List = fun left right _ -> List (left @ right)
              Ring = fun left right _ -> Ring (Set.union left right)
              Table = fun left right _ -> Table (left @@ right) }

        let SubFns =
            { Bool = fun left right _ -> Bool (if left && right then false elif left then true elif right then true else false)
              Int = fun left right _ -> Int (left - right)
              Int64 = fun left right _ -> Int64 (left - right)
              Single = fun left right _ -> Single (left - right)
              Double = fun left right _ -> Double (left - right)
              Vector2 = fun left right _ -> Vector2 (left - right)
              String = fun left right _ -> String (left.Replace (right, String.Empty))
              Keyword = fun left right _ -> String (left.Replace (right, String.Empty))
              Tuple = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Sub"], "Cannot subtract tuples.", originOpt)
              Keyphrase = fun _ _ _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Sub"], "Cannot subtract keyphrases.", originOpt)
              Codata = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Sub"], "Cannot subtract codata.", originOpt)
              List = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Sub"], "Cannot subtract lists.", originOpt)
              Ring = fun left right _ -> Ring (Set.difference left right)
              Table = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Sub"], "Cannot subtract tables.", originOpt) }

        let MulFns =
            { Bool = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Mul"], "Cannot multiply bools.", originOpt)
              Int = fun left right _ -> Int (left * right)
              Int64 = fun left right _ -> Int64 (left * right)
              Single = fun left right _ -> Single (left * right)
              Double = fun left right _ -> Double (left * right)
              Vector2 = fun left right _ -> Vector2 (Vector2.Multiply (left, right))
              String = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Mul"], "Cannot multiply strings.", originOpt)
              Keyword = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Mul"], "Cannot multiply keyword.", originOpt)
              Tuple = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Mul"], "Cannot multiply tuples.", originOpt)
              Keyphrase = fun _ _ _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Mul"], "Cannot multiply keyphrases.", originOpt)
              Codata = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Mul"], "Cannot multiply codata.", originOpt)
              List = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Mul"], "Cannot multiply lists.", originOpt)
              Ring = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Mul"], "Cannot multiply rings.", originOpt)
              Table = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Mul"], "Cannot multiply tables.", originOpt) }

        let DivFns =
            { Bool = fun left right originOpt -> if right = false then Violation (["OutOfRangeArgument"; "Binary"; "Div"], "Cannot divide by a false bool.", originOpt) else Bool (if left && right then true else false)
              Int = fun left right originOpt -> if right = 0 then Violation (["OutOfRangeArgument"; "Binary"; "Div"], "Cannot divide by a zero int.", originOpt) else Int (left / right)
              Int64 = fun left right originOpt -> if right = 0L then Violation (["OutOfRangeArgument"; "Binary"; "Div"], "Cannot divide by a zero 64-bit int.", originOpt) else Int64 (left / right)
              Single = fun left right _ -> Single (left / right)
              Double = fun left right _ -> Double (left / right)
              Vector2 = fun left right _ -> Vector2 (Vector2.Divide (left, right))
              String = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Div"], "Cannot divide strings.", originOpt)
              Keyword = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Div"], "Cannot divide keywords.", originOpt)
              Tuple = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Div"], "Cannot divide tuples.", originOpt)
              Keyphrase = fun _ _ _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Div"], "Cannot divide keyphrases.", originOpt)
              Codata = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Div"], "Cannot divide codata.", originOpt)
              List = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Div"], "Cannot divide lists.", originOpt)
              Ring = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Div"], "Cannot divide rings.", originOpt)
              Table = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Div"], "Cannot divide tables.", originOpt) }

        let ModFns =
            { Bool = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Mod"], "Cannot modulate bools.", originOpt)
              Int = fun left right originOpt -> if right = 0 then Violation (["OutOfRangeArgument"; "Binary"; "Mod"], "Cannot modulate by a zero int.", originOpt) else Int (left % right)
              Int64 = fun left right originOpt -> if right = 0L then Violation (["OutOfRangeArgument"; "Binary"; "Mod"], "Cannot divide by a zero 64-bit int.", originOpt) else Int64 (left % right)
              Single = fun left right _ -> Single (left % right)
              Double = fun left right _ -> Double (left % right)
              Vector2 = fun left right _ -> Vector2 (OpenTK.Vector2 (left.X % right.X, left.Y % right.Y))
              String = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Mod"], "Cannot modulate strings.", originOpt)
              Keyword = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Mod"], "Cannot modulate keywords.", originOpt)
              Tuple = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Mod"], "Cannot modulate tuples.", originOpt)
              Keyphrase = fun _ _ _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Mod"], "Cannot modulate keyphrases.", originOpt)
              Codata = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Mod"], "Cannot modulate codata.", originOpt)
              List = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Mod"], "Cannot modulate lists.", originOpt)
              Ring = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Mod"], "Cannot modulate rings.", originOpt)
              Table = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Mod"], "Cannot modulate tables.", originOpt) }

        let PowFns =
            { Bool = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Pow"], "Cannot power bools.", originOpt)
              Int = fun left right _ -> Int (int ^ Math.Pow (double left, double right))
              Int64 = fun left right _ -> Int64 (int64 ^ Math.Pow (double left, double right))
              Single = fun left right _ -> Single (single ^ Math.Pow (double left, double right))
              Double = fun left right _ -> Double (Math.Pow (double left, double right))
              Vector2 = fun left right _ -> Vector2 (OpenTK.Vector2 (single ^ Math.Pow (double left.X, double right.X), single ^ Math.Pow (double left.Y, double right.Y)))
              String = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Pow"], "Cannot power strings.", originOpt)
              Keyword = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Pow"], "Cannot power keywords.", originOpt)
              Tuple = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Pow"], "Cannot power tuples.", originOpt)
              Keyphrase = fun _ _ _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Pow"], "Cannot power keyphrases.", originOpt)
              Codata = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Pow"], "Cannot power codata.", originOpt)
              List = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Pow"], "Cannot power lists.", originOpt)
              Ring = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Pow"], "Cannot power rings.", originOpt)
              Table = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Pow"], "Cannot power tables.", originOpt) }

        let RootFns =
            { Bool = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Root"], "Cannot root bools.", originOpt)
              Int = fun left right _ -> Int (int ^ Math.Pow (double left, 1.0 / double right))
              Int64 = fun left right _ -> Int64 (int64 ^ Math.Pow (double left, 1.0 / double right))
              Single = fun left right _ -> Single (single ^ Math.Pow (double left, 1.0 / double right))
              Double = fun left right _ -> Double (Math.Pow (double left, 1.0 / double right))
              Vector2 = fun left right _ -> Vector2 (OpenTK.Vector2 (single ^ Math.Pow (double left.X, 1.0 / double right.X), single ^ Math.Pow (double left.Y, 1.0 / double right.Y)))
              String = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Root"], "Cannot root strings.", originOpt)
              Keyword = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Root"], "Cannot root keywords.", originOpt)
              Tuple = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Root"], "Cannot root tuples.", originOpt)
              Keyphrase = fun _ _ _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Root"], "Cannot root keyphrases.", originOpt)
              Codata = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Root"], "Cannot root codata.", originOpt)
              List = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Root"], "Cannot root lists.", originOpt)
              Ring = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Root"], "Cannot root rings.", originOpt)
              Table = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Root"], "Cannot root tables.", originOpt) }

        let CrossFns =
            { Bool = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Cross"], "Cannot cross multiply bools.", originOpt)
              Int = fun left right _ -> Int (left * right)
              Int64 = fun left right _ -> Int64 (left * right)
              Single = fun left right _ -> Single (left * right)
              Double = fun left right _ -> Double (left * right)
              Vector2 = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Cross"], "Cannot cross multiply 2-dimensional vectors.", originOpt)
              String = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Cross"], "Cannot cross multiply strings.", originOpt)
              Keyword = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Cross"], "Cannot cross multiply keywords.", originOpt)
              Tuple = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Cross"], "Cannot cross multiply tuples.", originOpt)
              Keyphrase = fun _ _ _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Cross"], "Cannot cross multiple keyphrases.", originOpt)
              Codata = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Cross"], "Cannot cross multiply codata.", originOpt)
              List = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Cross"], "Cannot cross multiply lists.", originOpt)
              Ring = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Cross"], "Cannot cross multiply rings.", originOpt)
              Table = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Cross"], "Cannot cross multiply tables.", originOpt) }

        let DotFns =
            { Bool = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Dot"], "Cannot dot multiply bools.", originOpt)
              Int = fun left right _ -> Int (left * right)
              Int64 = fun left right _ -> Int64 (left * right)
              Single = fun left right _ -> Single (left * right)
              Double = fun left right _ -> Double (left * right)
              Vector2 = fun left right _ -> Single (Vector2.Dot (left, right))
              String = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Dot"], "Cannot dot multiply strings.", originOpt)
              Keyword = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Dot"], "Cannot dot multiply keywords.", originOpt)
              Tuple = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Dot"], "Cannot dot multiply tuples.", originOpt)
              Keyphrase = fun _ _ _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Dot"], "Cannot dot multiply keyphrases.", originOpt)
              Codata = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Dot"], "Cannot dot multiply codata.", originOpt)
              List = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Dot"], "Cannot dot multiply lists.", originOpt)
              Ring = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Dot"], "Cannot dot multiply rings.", originOpt)
              Table = fun _ _ originOpt -> Violation (["InvalidArgumentType"; "Binary"; "Dot"], "Cannot dot multiply tables.", originOpt) }

        let evalBinaryInner (fns : BinaryFns) fnOriginOpt fnName evaledLeft evaledRight world =
            match (evaledLeft, evaledRight) with
            | (Bool boolLeft, Bool boolRight) -> (fns.Bool boolLeft boolRight fnOriginOpt, world)
            | (Int intLeft, Int intRight) -> (fns.Int intLeft intRight fnOriginOpt, world)
            | (Int64 int64Left, Int64 int64Right) -> (fns.Int64 int64Left int64Right fnOriginOpt, world)
            | (Single singleLeft, Single singleRight) -> (fns.Single singleLeft singleRight fnOriginOpt, world)
            | (Double doubleLeft, Double doubleRight) -> (fns.Double doubleLeft doubleRight fnOriginOpt, world)
            | (Vector2 vector2Left, Vector2 vector2Right) -> (fns.Vector2 vector2Left vector2Right fnOriginOpt, world)
            | (String stringLeft, String stringRight) -> (fns.String stringLeft stringRight fnOriginOpt, world)
            | (Keyword keywordLeft, Keyword keywordRight) -> (fns.String keywordLeft keywordRight fnOriginOpt, world)
            | (Tuple tupleLeft, Tuple tupleRight) -> (fns.Tuple tupleLeft tupleRight fnOriginOpt, world)
            | (Keyphrase (nameLeft, phraseLeft), Keyphrase (nameRight, phraseRight)) -> (fns.Keyphrase nameLeft phraseLeft nameRight phraseRight fnOriginOpt, world)
            | (Codata codataLeft, Codata codataRight) -> (fns.Codata codataLeft codataRight fnOriginOpt, world)
            | (List listLeft, List listRight) -> (fns.List listLeft listRight fnOriginOpt, world)
            | (Ring ringLeft, Ring ringRight) -> (fns.Ring ringLeft ringRight fnOriginOpt, world)
            | (Table tableLeft, Table tableRight) -> (fns.Table tableLeft tableRight fnOriginOpt, world)
            | (Violation _ as violation, _) -> (violation, world)
            | (_, (Violation _ as violation)) -> (violation, world)
            | _ -> (Violation (["InvalidArgumentType"; "Binary"; (String.capitalize fnName)], "Cannot apply a binary function on unlike or incompatible values.", fnOriginOpt), world)

        let evalBinary fns fnOriginOpt fnName evaledArgs world =
            match evaledArgs with
            | [evaledLeft; evaledRight] -> evalBinaryInner fns fnOriginOpt fnName evaledLeft evaledRight world                
            | _ -> (Violation (["InvalidArgumentCount"; "Binary"; (String.capitalize fnName)], "Incorrect number of arguments for application of '" + fnName + "'; 2 arguments required.", fnOriginOpt), world)