' https://msdn.microsoft.com/en-us/library/system.drawing.printing.printdocument(v=vs.110).aspx

Imports System.ComponentModel
Imports System.Drawing.Printing
Imports System.Collections.Generic


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
        Dim mpd As MyPrintDoc = New MyPrintDoc()
        Dim pd = New PrintDialog()
        Const PREFIX As String = "S"
        Const SN_LEN As Integer = 8
        Const MAX_SERIAL_NUMBERS As Integer = 67
        Dim fmt As String

        fmt = New String("0", SN_LEN - 1)
        For I As Integer = 0 To MAX_SERIAL_NUMBERS
            mpd.addSerialNumber(PREFIX + I.ToString(fmt))
        Next
        pd.AllowPrintToFile = True
        pd.AllowCurrentPage = False
        pd.AllowSelection = False
        pd.AllowSomePages = False
        pd.Document = mpd
        If pd.ShowDialog() <> DialogResult.Cancel Then
            mpd.Print()

        End If
    End Sub
#End Region

End Class
Public Class MyPrintDoc
    Inherits PrintDocument

    Dim _sns As List(Of String) = Nothing

    Sub New()
        init()
    End Sub

    Sub New(zzz As List(Of String))
        init()
        If Not zzz Is Nothing Then
            _sns.AddRange(zzz)
        End If
    End Sub

    Sub init()
        '       _raiseEvents = True
        _sns = New List(Of String)
    End Sub
    Protected Overrides Sub OnBeginPrint(e As PrintEventArgs)
        MyBase.OnBeginPrint(e)
        currentPage = 1
        maxPage = _sns.Count / MAX_PER_PAGE

    End Sub

    Protected Overrides Sub OnEndPrint(e As PrintEventArgs)
        MyBase.OnEndPrint(e)
    End Sub
    Dim currentPage As Integer
    Dim maxPage As Integer
    Const MAX_PER_PAGE As Integer = 100

    Protected Overrides Sub OnPrintPage(e As PrintPageEventArgs)
        MyBase.OnPrintPage(e)
        e.Graphics.DrawRectangle(
            Pens.Red,
            New Rectangle(
                e.PageBounds.X,
                e.PageBounds.Y,
                e.PageBounds.Width,
                e.PageBounds.Height))
        e.Graphics.DrawRectangle(
            Pens.Black,
            New Rectangle(
                e.MarginBounds.X,
                e.MarginBounds.Y,
                e.MarginBounds.Width,
                e.MarginBounds.Height))
        Dim startIndex = (currentPage - 2) * MAX_PER_PAGE
        Const COLS As Integer = 4
        Const SN_WIDTH As Integer = 100
        Const SN_HEIGHT As Integer = 20

        Dim row As Integer, col As Integer
        Dim afont = New System.Drawing.Font("Courier New", 10.0F, FontStyle.Regular)

        For index As Integer = 1 To MAX_PER_PAGE
            If index > _sns.Count Then
                Continue For
            End If
            row = (index - 1) \ COLS
            col = (index - 1) - row * COLS
            e.Graphics.DrawString(_sns(index-1), afont, Brushes.Black, e.MarginBounds.Left + (col + 1) * SN_WIDTH, e.MarginBounds.Top + row * SN_HEIGHT)
        Next
        currentPage = currentPage + 1
        e.HasMorePages = currentPage <= maxPage
    End Sub

    Protected Overrides Sub OnQueryPageSettings(e As QueryPageSettingsEventArgs)
        MyBase.OnQueryPageSettings(e)
        e.PageSettings.PrinterSettings.MinimumPage = 0
        e.PageSettings.PrinterSettings.MaximumPage = maxPage
    End Sub

    Friend Sub addSerialNumber(v As String)
        If Not _sns.Contains(v) Then
            _sns.Add(v)
        End If
    End Sub


End Class

