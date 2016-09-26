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
    Const FONT_NAME_2 As String = "Calibri"
#End Region

#Region "fields"
    Public Shared showFrames As Boolean = False
    Public Shared showGrid As Boolean = False
    Public Shared showContent As Boolean = True

    '  Dim afont As Font
    Dim currentPage As Integer
    Dim maxPage As Integer
    Dim _sns As List(Of String) = Nothing
    Dim _partNum As String
    Dim _lotNum As Integer

    ReadOnly f10 As Font = New Font(FONT_NAME, 10)
    ReadOnly f10b As Font = New Font(FONT_NAME, 10, FontStyle.Bold)
    ReadOnly f12 As Font = New Font(FONT_NAME, 12, FontStyle.Bold)
    ReadOnly f14 As Font = New Font(FONT_NAME, 14)
    ReadOnly f14b As Font = New Font(FONT_NAME, 14, FontStyle.Bold)
    ReadOnly f16 As Font = New Font(FONT_NAME, 16, FontStyle.Bold)
    ReadOnly f18 As Font = New Font(FONT_NAME, 18, FontStyle.Bold)
    ReadOnly f20 As Font = New Font(FONT_NAME, 20)
    ReadOnly f20b As Font = New Font(FONT_NAME, 20, FontStyle.Bold Or FontStyle.Underline)

    Public ReadOnly f11 As Font = New Font(FONT_NAME_2, 11)

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
    Public Property partNum() As String
        Get
            Return _partNum
        End Get
        Set(ByVal value As String)
            _partNum = value
        End Set
    End Property
    Public Property lotNum() As Integer
        Get
            Return _lotNum
        End Get
        Set(ByVal value As Integer)
            _lotNum = value
        End Set
    End Property

#Region "methods"
    Sub init()
        '        afont = New Font(FONT_NAME, POINT_SIZE, FontStyle.Regular)
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
        If showContent Then drawPageContent(e.Graphics, e.MarginBounds, partNum, lotNum)
        currentPage = currentPage + 1
        e.HasMorePages = currentPage <= maxPage
    End Sub

    Protected Overrides Sub OnQueryPageSettings(e As QueryPageSettingsEventArgs)
        MyBase.OnQueryPageSettings(e)
        e.PageSettings.PrinterSettings.MinimumPage = 0
        e.PageSettings.PrinterSettings.MaximumPage = maxPage
    End Sub
#End Region

    Sub drawPageContent(g As Graphics, r As Rectangle, partNumber As String, lotNumber As Integer)
        Dim startIndex As Integer = (currentPage - 2) * MAX_PER_PAGE

        Using br As New SolidBrush(Color.Black)
            centerString(f20b, g, 40, "SERIAL NUMBERS SHEET", r)
            Using p = New Pen(br, 2)
                g.DrawRectangle(p, New Rectangle(140, 100, 80, 40))
                g.DrawRectangle(p, New Rectangle(310, 100, 100, 40))
                g.DrawRectangle(p, New Rectangle(500, 100, 80, 40))
                g.DrawRectangle(p, New Rectangle(500, 860, 80, 40))
            End Using
            g.DrawString("MODEL", f10, br, New PointF(80, 110))
            g.DrawString("PN", f18, br, New PointF(270, 110))
            g.DrawString("LOT#", f14, br, New PointF(440, 110))

            g.DrawString(partNumber, f10b, br, 320, 115) ' draw part number
            g.DrawString(lotNumber.ToString, f10b, br, 520, 115) ' draw lot number
            g.DrawString(_sns.Count.ToString, f20, br, 520, 865) ' draw lot number

            showFireType(g, br)
            printSerialNumbersIn(g, New Rectangle(0, 0, 100, 100), br)
            showSNInfo(g, br)
        End Using
    End Sub

    Sub showFireType(g As Graphics, br As Brush)
        Const yFireType As Integer = 180
        g.DrawString("AUTO", f12, br, New PointF(250, yFireType))
        g.DrawString("BURST", f12, br, New PointF(370, yFireType))
        g.DrawString("SEMI", f12, br, New PointF(480, yFireType))
    End Sub

    Sub showSNInfo(g As Graphics, br As Brush)
        Const S1 As String = "TOTAL PARTS IN THIS SERIAL NUMBER RANGE:"
        Const S2 As String = "THESE PARTS HAVE BEEN TRANSFERRED TO:"

        Dim sf = g.MeasureString(S1, f11)
        '        Const YMSG As Integer = 800
        g.DrawString(S1, f11, br, New PointF(50, 880))
        g.DrawString(S2, f11, br, New PointF(50, 930))
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

    Sub printSerialNumbersIn(g As Graphics, r As Rectangle, br As Brush)
        Const VOFFSET As Integer = 220
        Const NUM_ROWS = MAX_PER_PAGE \ COLS
        Dim row As Integer, col As Integer
        Dim snWidth As Integer, snIndex As Integer, sn As String, snHeight As Integer, sf As SizeF

        If showFrames Then g.DrawRectangle(Pens.Blue, r)

        Const WINC = 50
        Const HOFF = 100
        For index As Integer = 1 To MAX_PER_PAGE
            snIndex = (index + ((currentPage - 1) * MAX_PER_PAGE)) - 1
            sn = _sns(snIndex)
            If (index = 1) Then
                sf = g.MeasureString(sn, f12)
                snWidth = Convert.ToInt32(Math.Ceiling(sf.Width)) + WINC
                snHeight = Convert.ToInt32(Math.Ceiling(sf.Height)) + 2
                '                snHeight = Convert.ToInt32(Math.Floor(sf.Height))
            End If

            If index + ((currentPage - 1) * MAX_PER_PAGE) > _sns.Count Then
                Continue For
            End If
            row = (index - 1) Mod NUM_ROWS
            col = (index - 1) \ NUM_ROWS

            '         col = (index - 1) Mod COLS
            '        row = (index - 1) \ COLS

            g.DrawString(sn, f16, br,
                r.Left + col * snWidth + HOFF,
                VOFFSET + r.Top + row * snHeight)
        Next
    End Sub

    Friend Sub addSerialNumbers(prefix As String, maxSN As Integer, snLen As Integer)
        '        Const SN_LEN As Integer = 8
        Dim fmt As String

        Logger.log(MethodBase.GetCurrentMethod())
        fmt = New String("0", snLen - prefix.Length)
        For I As Integer = 0 To maxSN - 1
            addSerialNumber(prefix + I.ToString(fmt))
        Next
    End Sub

    Friend Sub drawIn(g As Graphics, r As Rectangle, partNumber As String, lotNumber As Integer)
        Me.partNum = partNumber
        Me.lotNum = lotNumber
        currentPage = 1
        drawDecorations(g, r, r)
        If showContent Then Me.drawPageContent(g, r, partNumber, lotNumber)
    End Sub
End Class
