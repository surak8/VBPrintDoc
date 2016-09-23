Public Class SNReportView
    Inherits UserControl

    Public Sub New()
        InitializeComponent()
    End Sub

    Dim mpd As MyPrintDoc

    Sub InitializeComponent()
        ''      Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.Name = "riktest"
    End Sub
    Protected Overrides Sub OnCreateControl()
        MyBase.OnCreateControl()
        mpd = New MyPrintDoc()
        mpd.addSerialNumbers("RIK", 100)
    End Sub
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        Dim r As Rectangle

        r = New Rectangle(Point.Empty, New Size(Me.Bounds.Width - 1, Me.Bounds.Height - 1))
        If DesignMode Then
            e.Graphics.DrawRectangle(Pens.Red, r)
        End If
        If Not mpd Is Nothing Then
            mpd.drawIn(e.Graphics, r)
        End If
        ''        End If
        MyBase.OnPaint(e)
    End Sub
End Class