Public Class ImagePreviewWindow
    Private ReadOnly _theImage As String
    Private ReadOnly _title As String
    Private _fullSize As Boolean = True

    Public Sub New(title As String, Optional imgFile As String = Nothing)
        InitializeComponent()

        If imgFile IsNot Nothing Then
            _theImage = imgFile
            picPreview.ImageLocation = imgFile
            picPreview.Visible = True
        End If

        _title = title
        Me.SetTitle()
    End Sub

    Private Sub picPreview_Click(sender As System.Object, e As System.EventArgs) Handles picPreview.Click
        _fullSize = Not _fullSize
        If _fullSize Then
            picPreview.Dock = DockStyle.None
            picPreview.SizeMode = PictureBoxSizeMode.AutoSize
        Else
            picPreview.Dock = DockStyle.Fill
            picPreview.SizeMode = PictureBoxSizeMode.Zoom
        End If
        Me.SetTitle()
    End Sub

    Private Sub ImagePreview_Resize(sender As System.Object, e As System.EventArgs) Handles MyBase.ResizeEnd
        Me.SetTitle()
    End Sub

    Private Sub SetTitle()
        If _theImage Is Nothing Then
            Me.Text = _title
        Else
            Me.Text = String.Format("{0} ({1})",
                            _title,
                            IIf(_fullSize, "Original Size", "Sized to Fit"))
        End If
    End Sub
End Class