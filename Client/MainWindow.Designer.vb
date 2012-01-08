<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class MainWindow
    Inherits PhotoMosaic.AbstractNotificationForm

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
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainWindow))
        Me.ofdPicture = New System.Windows.Forms.OpenFileDialog()
        Me.sfdMosaic = New System.Windows.Forms.SaveFileDialog()
        Me.fbdImageLibrary = New System.Windows.Forms.FolderBrowserDialog()
        Me.tipButtonJobs = New System.Windows.Forms.ToolTip(Me.components)
        Me.btnViewJobs = New System.Windows.Forms.Button()
        Me.txtStatus = New System.Windows.Forms.TextBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.nupSlices = New System.Windows.Forms.NumericUpDown()
        Me.picHiddenPreview = New System.Windows.Forms.PictureBox()
        Me.lblFinalSize = New System.Windows.Forms.Label()
        Me.lblOriginalSize = New System.Windows.Forms.Label()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.grpImageLibrary = New System.Windows.Forms.GroupBox()
        Me.txtLibrary = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.panelPreview = New System.Windows.Forms.Panel()
        Me.picTile = New System.Windows.Forms.PictureBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.nupTileDimension = New System.Windows.Forms.NumericUpDown()
        Me.btnSelectLibrary = New System.Windows.Forms.Button()
        Me.lbImageLibrary = New System.Windows.Forms.ListBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.btnViewOriginal = New System.Windows.Forms.Button()
        Me.btnCreateMosaic = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtFileName = New System.Windows.Forms.TextBox()
        Me.btnSelectOriginal = New System.Windows.Forms.Button()
        CType(Me.nupSlices, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.picHiddenPreview, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.grpImageLibrary.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.panelPreview.SuspendLayout()
        CType(Me.picTile, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.nupTileDimension, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'ofdPicture
        '
        Me.ofdPicture.Filter = "Image files|*.jpg;*.gif;*.png"
        '
        'sfdMosaic
        '
        Me.sfdMosaic.DefaultExt = "jpg"
        Me.sfdMosaic.Filter = "JPEG files|*.jpg"
        '
        'fbdImageLibrary
        '
        Me.fbdImageLibrary.Description = "Choose directory containing tile images"
        Me.fbdImageLibrary.RootFolder = System.Environment.SpecialFolder.MyPictures
        Me.fbdImageLibrary.ShowNewFolderButton = False
        '
        'btnViewJobs
        '
        Me.btnViewJobs.Font = New System.Drawing.Font("Webdings", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.btnViewJobs.Location = New System.Drawing.Point(133, 400)
        Me.btnViewJobs.Name = "btnViewJobs"
        Me.btnViewJobs.Size = New System.Drawing.Size(27, 27)
        Me.btnViewJobs.TabIndex = 12
        Me.btnViewJobs.Text = "L"
        Me.tipButtonJobs.SetToolTip(Me.btnViewJobs, "Show jobs submitted by current user")
        Me.btnViewJobs.UseVisualStyleBackColor = True
        '
        'txtStatus
        '
        Me.txtStatus.BorderStyle = System.Windows.Forms.BorderStyle.None
        Me.txtStatus.Location = New System.Drawing.Point(226, 358)
        Me.txtStatus.Multiline = True
        Me.txtStatus.Name = "txtStatus"
        Me.txtStatus.ReadOnly = True
        Me.txtStatus.Size = New System.Drawing.Size(268, 73)
        Me.txtStatus.TabIndex = 25
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Location = New System.Drawing.Point(20, 360)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(90, 13)
        Me.Label7.TabIndex = 9
        Me.Label7.Text = "Number of Slices:"
        '
        'nupSlices
        '
        Me.nupSlices.Location = New System.Drawing.Point(118, 358)
        Me.nupSlices.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.nupSlices.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.nupSlices.Name = "nupSlices"
        Me.nupSlices.Size = New System.Drawing.Size(37, 20)
        Me.nupSlices.TabIndex = 10
        Me.nupSlices.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'picHiddenPreview
        '
        Me.picHiddenPreview.BackColor = System.Drawing.Color.Fuchsia
        Me.picHiddenPreview.Location = New System.Drawing.Point(474, 413)
        Me.picHiddenPreview.Name = "picHiddenPreview"
        Me.picHiddenPreview.Size = New System.Drawing.Size(20, 20)
        Me.picHiddenPreview.TabIndex = 24
        Me.picHiddenPreview.TabStop = False
        Me.picHiddenPreview.Visible = False
        '
        'lblFinalSize
        '
        Me.lblFinalSize.AutoSize = True
        Me.lblFinalSize.Location = New System.Drawing.Point(333, 49)
        Me.lblFinalSize.Name = "lblFinalSize"
        Me.lblFinalSize.Size = New System.Drawing.Size(60, 13)
        Me.lblFinalSize.TabIndex = 7
        Me.lblFinalSize.Text = "1000x1000"
        '
        'lblOriginalSize
        '
        Me.lblOriginalSize.AutoSize = True
        Me.lblOriginalSize.Location = New System.Drawing.Point(190, 49)
        Me.lblOriginalSize.Name = "lblOriginalSize"
        Me.lblOriginalSize.Size = New System.Drawing.Size(48, 13)
        Me.lblOriginalSize.TabIndex = 5
        Me.lblOriginalSize.Text = "100x100"
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(272, 49)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(55, 13)
        Me.Label4.TabIndex = 6
        Me.Label4.Text = "Final Size:"
        '
        'grpImageLibrary
        '
        Me.grpImageLibrary.Controls.Add(Me.txtLibrary)
        Me.grpImageLibrary.Controls.Add(Me.Label6)
        Me.grpImageLibrary.Controls.Add(Me.GroupBox1)
        Me.grpImageLibrary.Controls.Add(Me.btnSelectLibrary)
        Me.grpImageLibrary.Controls.Add(Me.lbImageLibrary)
        Me.grpImageLibrary.Location = New System.Drawing.Point(12, 92)
        Me.grpImageLibrary.Name = "grpImageLibrary"
        Me.grpImageLibrary.Size = New System.Drawing.Size(482, 235)
        Me.grpImageLibrary.TabIndex = 8
        Me.grpImageLibrary.TabStop = False
        Me.grpImageLibrary.Text = "Image Library"
        '
        'txtLibrary
        '
        Me.txtLibrary.Location = New System.Drawing.Point(68, 30)
        Me.txtLibrary.Name = "txtLibrary"
        Me.txtLibrary.ReadOnly = True
        Me.txtLibrary.Size = New System.Drawing.Size(129, 20)
        Me.txtLibrary.TabIndex = 1
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Location = New System.Drawing.Point(24, 33)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(38, 13)
        Me.Label6.TabIndex = 0
        Me.Label6.Text = "Name:"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.panelPreview)
        Me.GroupBox1.Controls.Add(Me.Label3)
        Me.GroupBox1.Controls.Add(Me.nupTileDimension)
        Me.GroupBox1.Location = New System.Drawing.Point(300, 68)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(158, 147)
        Me.GroupBox1.TabIndex = 6
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Tessera Preview"
        '
        'panelPreview
        '
        Me.panelPreview.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.panelPreview.Controls.Add(Me.picTile)
        Me.panelPreview.Location = New System.Drawing.Point(47, 28)
        Me.panelPreview.MaximumSize = New System.Drawing.Size(64, 64)
        Me.panelPreview.Name = "panelPreview"
        Me.panelPreview.Size = New System.Drawing.Size(64, 64)
        Me.panelPreview.TabIndex = 0
        '
        'picTile
        '
        Me.picTile.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.picTile.Location = New System.Drawing.Point(-1, -1)
        Me.picTile.Name = "picTile"
        Me.picTile.Size = New System.Drawing.Size(16, 16)
        Me.picTile.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.picTile.TabIndex = 10
        Me.picTile.TabStop = False
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(18, 111)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(50, 13)
        Me.Label3.TabIndex = 1
        Me.Label3.Text = "Size (px):"
        '
        'nupTileDimension
        '
        Me.nupTileDimension.Location = New System.Drawing.Point(84, 107)
        Me.nupTileDimension.Maximum = New Decimal(New Integer() {64, 0, 0, 0})
        Me.nupTileDimension.Minimum = New Decimal(New Integer() {2, 0, 0, 0})
        Me.nupTileDimension.Name = "nupTileDimension"
        Me.nupTileDimension.Size = New System.Drawing.Size(45, 20)
        Me.nupTileDimension.TabIndex = 2
        Me.nupTileDimension.Value = New Decimal(New Integer() {16, 0, 0, 0})
        '
        'btnSelectLibrary
        '
        Me.btnSelectLibrary.Location = New System.Drawing.Point(206, 26)
        Me.btnSelectLibrary.Name = "btnSelectLibrary"
        Me.btnSelectLibrary.Size = New System.Drawing.Size(63, 27)
        Me.btnSelectLibrary.TabIndex = 2
        Me.btnSelectLibrary.Text = "Browse..."
        Me.btnSelectLibrary.UseVisualStyleBackColor = True
        '
        'lbImageLibrary
        '
        Me.lbImageLibrary.FormattingEnabled = True
        Me.lbImageLibrary.Location = New System.Drawing.Point(25, 68)
        Me.lbImageLibrary.Name = "lbImageLibrary"
        Me.lbImageLibrary.Size = New System.Drawing.Size(242, 147)
        Me.lbImageLibrary.TabIndex = 3
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(117, 49)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(68, 13)
        Me.Label1.TabIndex = 4
        Me.Label1.Text = "Original Size:"
        '
        'btnViewOriginal
        '
        Me.btnViewOriginal.Enabled = False
        Me.btnViewOriginal.Font = New System.Drawing.Font("Webdings", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(2, Byte))
        Me.btnViewOriginal.Location = New System.Drawing.Point(467, 14)
        Me.btnViewOriginal.Name = "btnViewOriginal"
        Me.btnViewOriginal.Size = New System.Drawing.Size(27, 27)
        Me.btnViewOriginal.TabIndex = 3
        Me.btnViewOriginal.Text = "L"
        Me.btnViewOriginal.UseVisualStyleBackColor = True
        '
        'btnCreateMosaic
        '
        Me.btnCreateMosaic.Enabled = False
        Me.btnCreateMosaic.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.btnCreateMosaic.Location = New System.Drawing.Point(15, 395)
        Me.btnCreateMosaic.Name = "btnCreateMosaic"
        Me.btnCreateMosaic.Size = New System.Drawing.Size(112, 36)
        Me.btnCreateMosaic.TabIndex = 11
        Me.btnCreateMosaic.Text = "Create Mosaic"
        Me.btnCreateMosaic.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(19, 21)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(77, 13)
        Me.Label2.TabIndex = 0
        Me.Label2.Text = "Original Image:"
        '
        'txtFileName
        '
        Me.txtFileName.Location = New System.Drawing.Point(101, 17)
        Me.txtFileName.Name = "txtFileName"
        Me.txtFileName.ReadOnly = True
        Me.txtFileName.Size = New System.Drawing.Size(291, 20)
        Me.txtFileName.TabIndex = 1
        '
        'btnSelectOriginal
        '
        Me.btnSelectOriginal.Location = New System.Drawing.Point(401, 14)
        Me.btnSelectOriginal.Name = "btnSelectOriginal"
        Me.btnSelectOriginal.Size = New System.Drawing.Size(63, 27)
        Me.btnSelectOriginal.TabIndex = 2
        Me.btnSelectOriginal.Text = "Browse..."
        Me.btnSelectOriginal.UseVisualStyleBackColor = True
        '
        'MainWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(509, 448)
        Me.Controls.Add(Me.txtStatus)
        Me.Controls.Add(Me.Label7)
        Me.Controls.Add(Me.nupSlices)
        Me.Controls.Add(Me.picHiddenPreview)
        Me.Controls.Add(Me.btnViewJobs)
        Me.Controls.Add(Me.lblFinalSize)
        Me.Controls.Add(Me.lblOriginalSize)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.grpImageLibrary)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.btnViewOriginal)
        Me.Controls.Add(Me.btnCreateMosaic)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.txtFileName)
        Me.Controls.Add(Me.btnSelectOriginal)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "MainWindow"
        Me.Text = "Photo Mosaics Generator - TO THE CLOUD!"
        CType(Me.nupSlices, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.picHiddenPreview, System.ComponentModel.ISupportInitialize).EndInit()
        Me.grpImageLibrary.ResumeLayout(False)
        Me.grpImageLibrary.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox1.PerformLayout()
        Me.panelPreview.ResumeLayout(False)
        CType(Me.picTile, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.nupTileDimension, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ofdPicture As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnSelectOriginal As System.Windows.Forms.Button
    Friend WithEvents txtFileName As System.Windows.Forms.TextBox
    Friend WithEvents lbImageLibrary As System.Windows.Forms.ListBox
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents picTile As System.Windows.Forms.PictureBox
    Friend WithEvents nupTileDimension As System.Windows.Forms.NumericUpDown
    Friend WithEvents btnCreateMosaic As System.Windows.Forms.Button
    Friend WithEvents lblFinalSize As System.Windows.Forms.Label
    Friend WithEvents lblOriginalSize As System.Windows.Forms.Label
    Friend WithEvents btnViewOriginal As System.Windows.Forms.Button
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents panelPreview As System.Windows.Forms.Panel
    Friend WithEvents Label4 As System.Windows.Forms.Label
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents sfdMosaic As System.Windows.Forms.SaveFileDialog
    Friend WithEvents btnSelectLibrary As System.Windows.Forms.Button
    Friend WithEvents fbdImageLibrary As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents grpImageLibrary As System.Windows.Forms.GroupBox
    Friend WithEvents btnViewJobs As System.Windows.Forms.Button
    Friend WithEvents picHiddenPreview As System.Windows.Forms.PictureBox
    Friend WithEvents txtLibrary As System.Windows.Forms.TextBox
    Friend WithEvents Label6 As System.Windows.Forms.Label
    Friend WithEvents nupSlices As System.Windows.Forms.NumericUpDown
    Friend WithEvents Label7 As System.Windows.Forms.Label
    Friend WithEvents txtStatus As System.Windows.Forms.TextBox
    Friend WithEvents tipButtonJobs As System.Windows.Forms.ToolTip

End Class
