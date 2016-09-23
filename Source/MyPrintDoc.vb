Imports System.Drawing.Printing
Imports System.Reflection

Public Class MyPrintDoc
    Inherits PrintDocument
#Region "constants"
    Const MAX_PER_PAGE As Integer = 100
    Const COLS As Integer = 4
    Const SN_WIDTH As Integer = 100
    Const SN_HEIGHT As Integer = 20
    '    Const FONT_NAME As String = "Courier New"
    Const FONT_NAME As String = "Arial"
    Const POINT_SIZE As Single = 12.0F
#End Region

#Region "fields"
    Public Shared showFrames As Boolean = True
    Public Shared showGrid As Boolean = True
    Public Shared showContent As Boolean = True

    Dim afont As Font
    Dim currentPage As Integer
    Dim maxPage As Integer
    Dim _sns As List(Of String) = Nothing
#End Region
#Region "ctors"
    Sub New()
        init()
    End Sub
    Sub New(zzz As List(Of String))
        init()
        If Not zzz Is Nothing Then
            _sns.AddRange(zzz)
        End If
    End Sub
#End Region
#Region "properties"
#End Region
#Region "methods"
    Sub init()
        afont = New Font(FONT_NAME, POINT_SIZE, FontStyle.Regular)
        _sns = New List(Of String)
    End Sub
    Friend Sub addSerialNumber(v As String)
        If Not _sns.Contains(v) Then
            _sns.Add(v)
        End If
    End Sub
#End Region

#Region "PrintDocument implementation"
    Protected Overrides Sub OnBeginPrint(e As PrintEventArgs)
        MyBase.OnBeginPrint(e)
        currentPage = 1
        maxPage = _sns.Count \ MAX_PER_PAGE
        If _sns.Count Mod MAX_PER_PAGE > 0 Then maxPage = maxPage + 1
    End Sub
    Protected Overrides Sub OnPrintPage(e As PrintPageEventArgs)
        '        Dim row As Integer, col As Integer
        '    Dim startIndex As Integer

        MyBase.OnPrintPage(e)
        If showGrid Then drawDecorations(e.Graphics, e.MarginBounds, e.PageBounds)
        If showContent Then drawPageContent(e.Graphics, e.MarginBounds)
        currentPage = currentPage + 1
        e.HasMorePages = currentPage <= maxPage
    End Sub

    Protected Overrides Sub OnQueryPageSettings(e As QueryPageSettingsEventArgs)
        MyBase.OnQueryPageSettings(e)
        e.PageSettings.PrinterSettings.MinimumPage = 0
        e.PageSettings.PrinterSettings.MaximumPage = maxPage
    End Sub
#End Region

    'Sub drawPageContent(e As PrintPageEventArgs)
    Sub drawPageContent(g As Graphics, r As Rectangle)
        Dim startIndex As Integer = (currentPage - 2) * MAX_PER_PAGE

        ' center this, larger bold font
        Using myFont As New Font(FONT_NAME, POINT_SIZE + 4, FontStyle.Bold)
            centerString(myFont, g, 60, "SERIAL NUMBERS SHEET", r)
            '            g.DrawString("SERIAL NUMBERS SHEET", myFont, Brushes.Black, New PointF(100, 100))
        End Using

        Using p = New Pen(Color.Black, 2)
            g.DrawRectangle(p, New Rectangle(140, 100, 80, 40))
            g.DrawRectangle(p, New Rectangle(310, 100, 100, 40))
            g.DrawRectangle(p, New Rectangle(500, 100, 80, 40))
            g.DrawRectangle(p, New Rectangle(670, 100, 110, 40))
            g.DrawRectangle(p, New Rectangle(500, 860, 80, 40))
        End Using

        Using myFont As New Font(FONT_NAME, POINT_SIZE - 2, FontStyle.Bold)
            ' small font
            g.DrawString("MODEL", myFont, Brushes.Black, New PointF(80, 110))
        End Using

        Using myFont As New Font(FONT_NAME, POINT_SIZE + 4, FontStyle.Bold)
            ' largest font, bold
            g.DrawString("PN", myFont, Brushes.Black, New PointF(270, 110))
        End Using
        Using myFont As New Font(FONT_NAME, POINT_SIZE + 4, FontStyle.Bold)
            ' largest font
            g.DrawString("LOT#", myFont, Brushes.Black, New PointF(440, 110))
        End Using

        g.DrawString("AUTO", afont, Brushes.Black, New PointF(230, 200))
        g.DrawString("BURST", afont, Brushes.Black, New PointF(350, 200))
        g.DrawString("SEMI", afont, Brushes.Black, New PointF(460, 200))

        Const S1 As String = "TOTAL PARTS IN THIS SERIAL NUMBER RANGE:"
        Dim sf = g.MeasureString(S1, afont)
        Const YMSG As Integer = 900
        g.DrawString(S1, afont, Brushes.Black, New PointF(60, YMSG))
        g.DrawString("THESE PARTS HAVE BEEN TRANSFERRED TO:", afont, Brushes.Black,
                              New PointF(70, 840))

        Dim sf2 = g.MeasureString(New String("M", 9), afont)
        '//--        Const VOFFSET As Integer = 150
        '//  --      Const HOFFSET As Integer = 20
        printSerialNumbersIn(g,
            New Rectangle(0, 0, 100, 100), Math.Ceiling(sf2.Height))
        '       Return startIndex
    End Sub
    Shared Sub drawDecorations(g As Graphics, r As Rectangle, rpage As Rectangle)
        'Shared Sub drawDecorations(e As PrintPageEventArgs)
        If showFrames Then _
            g.DrawRectangle(Pens.Red,
                New Rectangle(rpage.X, rpage.Y, rpage.Width, rpage.Height))
        If showGrid Then
            Using p1 As New Pen(Color.Silver, 2)
                For i As Integer = rpage.Left To rpage.Right Step 100
                    g.DrawLine(p1, i, rpage.Top, i, rpage.Bottom)
                Next i
                For i As Integer = rpage.Top To rpage.Bottom Step 100
                    g.DrawLine(p1, rpage.Left, i, rpage.Right, i)
                Next i
            End Using
            For i As Integer = rpage.Left To rpage.Right Step 10
                g.DrawLine(Pens.LightGray, i, rpage.Top, i, rpage.Bottom)
            Next i
            For i As Integer = rpage.Top To rpage.Bottom Step 10
                g.DrawLine(Pens.LightGray, rpage.Left, i, rpage.Right, i)
            Next i

        End If
        If showFrames Then _
            g.DrawRectangle(
                Pens.Black,
                New Rectangle(
                    r.X,
                    r.Y,
                    r.Width,
                    r.Height))
    End Sub
    Sub centerString(f As Font, g As Graphics, y As Integer, text As String, r As Rectangle)
        Dim sf As SizeF = g.MeasureString(text, f)

        g.DrawString(text, f, Brushes.Black, r.Left + (r.Width - sf.Width) / 2, y)
    End Sub
    Sub printSerialNumbersIn(g As Graphics, r As Rectangle, h1 As Single)
        Dim row As Integer, col As Integer

        Const HOFFSET As Integer = 40
        If showFrames Then g.DrawRectangle(Pens.Blue, r)

        Dim wmult As Integer, loffset As Integer


        wmult = (r.Width \ COLS) - 8
        Dim snFont As Font = New Font(FONT_NAME, POINT_SIZE, FontStyle.Bold)


        loffset = wmult \ 4 + r.Left
        For index As Integer = 1 To MAX_PER_PAGE
            If index + ((currentPage - 1) * MAX_PER_PAGE) > _sns.Count Then
                Continue For
            End If
            row = (index - 1) \ COLS
            col = (index - 1) - row * COLS
            g.DrawString(
                _sns((index + ((currentPage - 1) * MAX_PER_PAGE)) - 1),
                snFont,
                Brushes.Black,
                loffset + col * wmult,
                r.Top + row * h1)
        Next
    End Sub
    Friend Sub addSerialNumbers(prefix As String, maxSN As Integer)
        Const SN_LEN As Integer = 8
        Dim fmt As String

        Logger.log(MethodBase.GetCurrentMethod())
        fmt = New String("0", SN_LEN - 1)
        For I As Integer = 0 To maxSN - 1
            addSerialNumber(prefix + I.ToString(fmt))
        Next
    End Sub

    Friend Sub drawIn(g As Graphics, r As Rectangle)
        currentPage = 1
        drawDecorations(g, r, r)
        If showContent Then Me.drawPageContent(g, r)
    End Sub
End Class
