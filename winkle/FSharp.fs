namespace Winkle

open Pulumi

module FSharp =
    let ioUnion1Of2 (output: Output<'T0>) : InputUnion<'T0, 'T1> =
        InputUnion.op_Implicit (output.Apply(fun value -> value))

    let ioUnion2Of2 (output: Output<'T1>) : InputUnion<'T0, 'T1> =
        InputUnion.op_Implicit (output.Apply(fun value -> value))
