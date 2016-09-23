Imports System.Reflection

Class Logger
    Friend Shared Sub log(mb As MethodBase)
        log(makeSig(mb))
    End Sub
    Friend Shared Sub log(msg As String)
        Trace.WriteLine(msg)
    End Sub
    Public Shared Function makeSig(mb As MethodBase) As String
        Return mb.ReflectedType.Name + "." + mb.Name
    End Function
End Class