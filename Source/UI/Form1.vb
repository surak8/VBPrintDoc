' https://msdn.microsoft.com/en-us/library/system.drawing.printing.printdocument(v=vs.110).aspx

Imports System.ComponentModel
Imports System.Reflection

''' <summary></summary>
Public Class Form1
#Region "fields"
#End Region
#Region "ctors"
#End Region
#Region "properties"
#End Region
#Region "methods"
    Sub tsmiExit_Click(sender As Object, e As EventArgs) Handles tsmiExit.Click
        Dim cea = New CancelEventArgs()

        Application.Exit(cea)
        If (cea.Cancel) Then
            Exit Sub
        End If
        Application.Exit()

    End Sub
    Sub tsmiTest_Click(sender As Object, e As EventArgs) Handles tsmiTest.Click
        Dim mpd As SerialNumberPrintDoc = New SerialNumberPrintDoc()
        Dim pd = New PrintDialog()
#If True Then
        Const PREFIX = "IARE00"
        Const SN_LEN As Integer = 10

#Else
        Const PREFIX As String = "S"
        Const SN_LEN As Integer = 8
#End If
        Const MAX_SERIAL_NUMBERS As Integer = 100
        Const KEY = "Previous Printer"
        Dim fmt As String

        Logger.log(MethodBase.GetCurrentMethod())
        mpd.partNum = "400003"
        mpd.lotNum = 29
        fmt = New String("0", SN_LEN - PREFIX.Length)
        For I As Integer = 0 To MAX_SERIAL_NUMBERS - 1
            mpd.addSerialNumber(PREFIX + I.ToString(fmt))
        Next
        pd.AllowPrintToFile = True
        pd.AllowCurrentPage = False
        pd.AllowSelection = False
        pd.AllowSomePages = False
        pd.Document = mpd
        Dim printerName As String = "Foxit PhantomPDF Printer"
        Dim tmp As String

        tmp = CType(Application.UserAppDataRegistry.GetValue(KEY, printerName), String)
        If Not String.IsNullOrEmpty(tmp) Then printerName = tmp
        pd.PrinterSettings.PrinterName = printerName
        If pd.ShowDialog() <> DialogResult.Cancel Then
            mpd.Print()
            Application.UserAppDataRegistry.SetValue(KEY, pd.PrinterSettings.PrinterName)
        End If
        '        End If
    End Sub

    Private Sub SnReportView1_Load(sender As Object, e As EventArgs) Handles SnReportView1.Load

    End Sub
#End Region

End Class

