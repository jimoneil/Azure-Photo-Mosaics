<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class ImagePreviewWindow
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ImagePreviewWindow))
        Me.picPreview = New System.Windows.Forms.PictureBox()
        CType(Me.picPreview, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'picPreview
        '
        Me.picPreview.ErrorImage = Global.PhotoMosaic.My.Resources.Resources.noimage
        Me.picPreview.InitialImage = Global.PhotoMosaic.My.Resources.Resources.loadimage
        Me.picPreview.Location = New System.Drawing.Point(0, -1)
        Me.picPreview.Name = "picPreview"
        Me.picPreview.Size = New System.Drawing.Size(150, 150)
        Me.picPreview.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize
        Me.picPreview.TabIndex = 0
        Me.picPreview.TabStop = False
        Me.picPreview.Visible = False
        '
        'ImagePreviewWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.AutoScroll = True
        Me.ClientSize = New System.Drawing.Size(784, 562)
        Me.Controls.Add(Me.picPreview)
        Me.DoubleBuffered = True
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "ImagePreviewWindow"
        Me.Text = "Image Preview"
        CType(Me.picPreview, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents picPreview As System.Windows.Forms.PictureBox
End Class
