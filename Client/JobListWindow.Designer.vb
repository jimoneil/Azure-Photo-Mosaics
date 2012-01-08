<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class JobListWindow
    Inherits PhotoMosaic.AbstractNotificationForm

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle2 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle3 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle4 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle5 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle6 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim DataGridViewCellStyle7 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(JobListWindow))
        Me.dgvJobs = New System.Windows.Forms.DataGridView()
        Me.RequestIdColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.OriginalUriDisplay = New System.Windows.Forms.DataGridViewLinkColumn()
        Me.OriginalLocationColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.FinalUriDisplayColumn = New System.Windows.Forms.DataGridViewLinkColumn()
        Me.TileSizeColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.SubmissionTimeColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.FinalUriColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.OriginalUriColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.JobEntryBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        Me.contJobs = New System.Windows.Forms.SplitContainer()
        Me.dgvStatus = New System.Windows.Forms.DataGridView()
        Me.TimestampDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.MessageDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.RoleIdDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.RequestId = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.PartitionKeyDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.RowKeyDataGridViewTextBoxColumn = New System.Windows.Forms.DataGridViewTextBoxColumn()
        Me.StatusEntryBindingSource = New System.Windows.Forms.BindingSource(Me.components)
        CType(Me.dgvJobs, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.JobEntryBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.contJobs, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.contJobs.Panel1.SuspendLayout()
        Me.contJobs.Panel2.SuspendLayout()
        Me.contJobs.SuspendLayout()
        CType(Me.dgvStatus, System.ComponentModel.ISupportInitialize).BeginInit()
        CType(Me.StatusEntryBindingSource, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'dgvJobs
        '
        Me.dgvJobs.AllowUserToAddRows = False
        Me.dgvJobs.AllowUserToDeleteRows = False
        Me.dgvJobs.AllowUserToOrderColumns = True
        Me.dgvJobs.AllowUserToResizeRows = False
        DataGridViewCellStyle1.BackColor = System.Drawing.Color.Azure
        Me.dgvJobs.AlternatingRowsDefaultCellStyle = DataGridViewCellStyle1
        Me.dgvJobs.AutoGenerateColumns = False
        Me.dgvJobs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill
        Me.dgvJobs.CausesValidation = False
        DataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control
        DataGridViewCellStyle2.Font = New System.Drawing.Font("Microsoft Sans Serif", 8.25!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText
        DataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.[True]
        Me.dgvJobs.ColumnHeadersDefaultCellStyle = DataGridViewCellStyle2
        Me.dgvJobs.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.RequestIdColumn, Me.OriginalUriDisplay, Me.OriginalLocationColumn, Me.FinalUriDisplayColumn, Me.TileSizeColumn, Me.SubmissionTimeColumn, Me.FinalUriColumn, Me.OriginalUriColumn})
        Me.dgvJobs.DataSource = Me.JobEntryBindingSource
        Me.dgvJobs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvJobs.Location = New System.Drawing.Point(0, 0)
        Me.dgvJobs.MultiSelect = False
        Me.dgvJobs.Name = "dgvJobs"
        Me.dgvJobs.ReadOnly = True
        Me.dgvJobs.RowHeadersVisible = False
        Me.dgvJobs.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvJobs.ShowEditingIcon = False
        Me.dgvJobs.Size = New System.Drawing.Size(779, 195)
        Me.dgvJobs.TabIndex = 0
        '
        'RequestIdColumn
        '
        Me.RequestIdColumn.DataPropertyName = "RequestId"
        Me.RequestIdColumn.HeaderText = "Request ID"
        Me.RequestIdColumn.MinimumWidth = 210
        Me.RequestIdColumn.Name = "RequestIdColumn"
        Me.RequestIdColumn.ReadOnly = True
        Me.RequestIdColumn.ToolTipText = "Unique request ID"
        '
        'OriginalUriDisplay
        '
        Me.OriginalUriDisplay.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.OriginalUriDisplay.DataPropertyName = "OriginalUriDisplay"
        DataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.OriginalUriDisplay.DefaultCellStyle = DataGridViewCellStyle3
        Me.OriginalUriDisplay.FillWeight = 1.0!
        Me.OriginalUriDisplay.HeaderText = "Original Image"
        Me.OriginalUriDisplay.MinimumWidth = 100
        Me.OriginalUriDisplay.Name = "OriginalUriDisplay"
        Me.OriginalUriDisplay.ReadOnly = True
        Me.OriginalUriDisplay.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.OriginalUriDisplay.ToolTipText = "View original image (stored in Windows Azure)"
        '
        'OriginalLocationColumn
        '
        Me.OriginalLocationColumn.DataPropertyName = "OriginalLocation"
        Me.OriginalLocationColumn.HeaderText = "Original Location"
        Me.OriginalLocationColumn.MinimumWidth = 150
        Me.OriginalLocationColumn.Name = "OriginalLocationColumn"
        Me.OriginalLocationColumn.ReadOnly = True
        Me.OriginalLocationColumn.ToolTipText = "Local source of original image"
        '
        'FinalUriDisplayColumn
        '
        Me.FinalUriDisplayColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.FinalUriDisplayColumn.DataPropertyName = "FinalUriDisplay"
        DataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.FinalUriDisplayColumn.DefaultCellStyle = DataGridViewCellStyle4
        Me.FinalUriDisplayColumn.FillWeight = 1.0!
        Me.FinalUriDisplayColumn.HeaderText = "Mosaic Image"
        Me.FinalUriDisplayColumn.MinimumWidth = 100
        Me.FinalUriDisplayColumn.Name = "FinalUriDisplayColumn"
        Me.FinalUriDisplayColumn.ReadOnly = True
        Me.FinalUriDisplayColumn.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.FinalUriDisplayColumn.ToolTipText = "View generated mosaic image (from Windows Azure)"
        '
        'TileSizeColumn
        '
        Me.TileSizeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.TileSizeColumn.DataPropertyName = "TileSize"
        DataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.TileSizeColumn.DefaultCellStyle = DataGridViewCellStyle5
        Me.TileSizeColumn.FillWeight = 1.0!
        Me.TileSizeColumn.HeaderText = "Tile Size"
        Me.TileSizeColumn.MinimumWidth = 60
        Me.TileSizeColumn.Name = "TileSizeColumn"
        Me.TileSizeColumn.ReadOnly = True
        Me.TileSizeColumn.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.TileSizeColumn.ToolTipText = "Tile size used for generated image (in pixels)"
        Me.TileSizeColumn.Width = 60
        '
        'SubmissionTimeColumn
        '
        Me.SubmissionTimeColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.SubmissionTimeColumn.DataPropertyName = "SubmissionTime"
        Me.SubmissionTimeColumn.FillWeight = 1.0!
        Me.SubmissionTimeColumn.HeaderText = "Submission Time"
        Me.SubmissionTimeColumn.MinimumWidth = 135
        Me.SubmissionTimeColumn.Name = "SubmissionTimeColumn"
        Me.SubmissionTimeColumn.ReadOnly = True
        Me.SubmissionTimeColumn.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.SubmissionTimeColumn.ToolTipText = "Date and time original image was submitted for processing"
        Me.SubmissionTimeColumn.Width = 135
        '
        'FinalUriColumn
        '
        Me.FinalUriColumn.DataPropertyName = "FinalUri"
        Me.FinalUriColumn.FillWeight = 33.75635!
        Me.FinalUriColumn.HeaderText = "FinalUri"
        Me.FinalUriColumn.Name = "FinalUriColumn"
        Me.FinalUriColumn.ReadOnly = True
        Me.FinalUriColumn.Visible = False
        '
        'OriginalUriColumn
        '
        Me.OriginalUriColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None
        Me.OriginalUriColumn.DataPropertyName = "OriginalUri"
        DataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        Me.OriginalUriColumn.DefaultCellStyle = DataGridViewCellStyle6
        Me.OriginalUriColumn.FillWeight = 1.0!
        Me.OriginalUriColumn.HeaderText = "OriginalUri"
        Me.OriginalUriColumn.MinimumWidth = 100
        Me.OriginalUriColumn.Name = "OriginalUriColumn"
        Me.OriginalUriColumn.ReadOnly = True
        Me.OriginalUriColumn.Resizable = System.Windows.Forms.DataGridViewTriState.[False]
        Me.OriginalUriColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable
        Me.OriginalUriColumn.ToolTipText = "View original image (stored in Windows Azure)"
        Me.OriginalUriColumn.Visible = False
        '
        'JobEntryBindingSource
        '
        Me.JobEntryBindingSource.AllowNew = False
        Me.JobEntryBindingSource.DataSource = GetType(PhotoMosaic.JobViewData)
        '
        'contJobs
        '
        Me.contJobs.Dock = System.Windows.Forms.DockStyle.Fill
        Me.contJobs.Location = New System.Drawing.Point(0, 0)
        Me.contJobs.Name = "contJobs"
        Me.contJobs.Orientation = System.Windows.Forms.Orientation.Horizontal
        '
        'contJobs.Panel1
        '
        Me.contJobs.Panel1.Controls.Add(Me.dgvJobs)
        '
        'contJobs.Panel2
        '
        Me.contJobs.Panel2.Controls.Add(Me.dgvStatus)
        Me.contJobs.Size = New System.Drawing.Size(779, 391)
        Me.contJobs.SplitterDistance = 195
        Me.contJobs.TabIndex = 1
        '
        'dgvStatus
        '
        Me.dgvStatus.AllowUserToAddRows = False
        Me.dgvStatus.AllowUserToDeleteRows = False
        Me.dgvStatus.AutoGenerateColumns = False
        Me.dgvStatus.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.dgvStatus.Columns.AddRange(New System.Windows.Forms.DataGridViewColumn() {Me.TimestampDataGridViewTextBoxColumn, Me.MessageDataGridViewTextBoxColumn, Me.RoleIdDataGridViewTextBoxColumn, Me.RequestId, Me.PartitionKeyDataGridViewTextBoxColumn, Me.RowKeyDataGridViewTextBoxColumn})
        Me.dgvStatus.DataSource = Me.StatusEntryBindingSource
        Me.dgvStatus.Dock = System.Windows.Forms.DockStyle.Fill
        Me.dgvStatus.Location = New System.Drawing.Point(0, 0)
        Me.dgvStatus.Name = "dgvStatus"
        Me.dgvStatus.ReadOnly = True
        Me.dgvStatus.RowHeadersVisible = False
        Me.dgvStatus.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.dgvStatus.Size = New System.Drawing.Size(779, 192)
        Me.dgvStatus.TabIndex = 0
        '
        'TimestampDataGridViewTextBoxColumn
        '
        Me.TimestampDataGridViewTextBoxColumn.DataPropertyName = "Timestamp"
        DataGridViewCellStyle7.Format = "G"
        DataGridViewCellStyle7.NullValue = Nothing
        Me.TimestampDataGridViewTextBoxColumn.DefaultCellStyle = DataGridViewCellStyle7
        Me.TimestampDataGridViewTextBoxColumn.HeaderText = "Timestamp"
        Me.TimestampDataGridViewTextBoxColumn.MinimumWidth = 120
        Me.TimestampDataGridViewTextBoxColumn.Name = "TimestampDataGridViewTextBoxColumn"
        Me.TimestampDataGridViewTextBoxColumn.ReadOnly = True
        Me.TimestampDataGridViewTextBoxColumn.ToolTipText = "Timestamp of status event"
        Me.TimestampDataGridViewTextBoxColumn.Width = 120
        '
        'MessageDataGridViewTextBoxColumn
        '
        Me.MessageDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill
        Me.MessageDataGridViewTextBoxColumn.DataPropertyName = "Message"
        Me.MessageDataGridViewTextBoxColumn.HeaderText = "Status Message (click header to refresh for selected job)"
        Me.MessageDataGridViewTextBoxColumn.Name = "MessageDataGridViewTextBoxColumn"
        Me.MessageDataGridViewTextBoxColumn.ReadOnly = True
        Me.MessageDataGridViewTextBoxColumn.ToolTipText = "Status message"
        '
        'RoleIdDataGridViewTextBoxColumn
        '
        Me.RoleIdDataGridViewTextBoxColumn.DataPropertyName = "RoleId"
        Me.RoleIdDataGridViewTextBoxColumn.HeaderText = "RoleId"
        Me.RoleIdDataGridViewTextBoxColumn.Name = "RoleIdDataGridViewTextBoxColumn"
        Me.RoleIdDataGridViewTextBoxColumn.ReadOnly = True
        Me.RoleIdDataGridViewTextBoxColumn.Visible = False
        '
        'RequestId
        '
        Me.RequestId.DataPropertyName = "RequestId"
        Me.RequestId.HeaderText = "RequestId"
        Me.RequestId.Name = "RequestId"
        Me.RequestId.ReadOnly = True
        Me.RequestId.Visible = False
        '
        'PartitionKeyDataGridViewTextBoxColumn
        '
        Me.PartitionKeyDataGridViewTextBoxColumn.DataPropertyName = "PartitionKey"
        Me.PartitionKeyDataGridViewTextBoxColumn.HeaderText = "PartitionKey"
        Me.PartitionKeyDataGridViewTextBoxColumn.Name = "PartitionKeyDataGridViewTextBoxColumn"
        Me.PartitionKeyDataGridViewTextBoxColumn.ReadOnly = True
        Me.PartitionKeyDataGridViewTextBoxColumn.Visible = False
        '
        'RowKeyDataGridViewTextBoxColumn
        '
        Me.RowKeyDataGridViewTextBoxColumn.DataPropertyName = "RowKey"
        Me.RowKeyDataGridViewTextBoxColumn.HeaderText = "RowKey"
        Me.RowKeyDataGridViewTextBoxColumn.Name = "RowKeyDataGridViewTextBoxColumn"
        Me.RowKeyDataGridViewTextBoxColumn.ReadOnly = True
        Me.RowKeyDataGridViewTextBoxColumn.Visible = False
        '
        'StatusEntryBindingSource
        '
        Me.StatusEntryBindingSource.AllowNew = False
        Me.StatusEntryBindingSource.DataSource = GetType(PhotoMosaic.AzureStorageBroker.StatusEntry)
        '
        'JobListWindow
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.ClientSize = New System.Drawing.Size(779, 391)
        Me.Controls.Add(Me.contJobs)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "JobListWindow"
        Me.Text = "Submitted Jobs"
        CType(Me.dgvJobs, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.JobEntryBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.contJobs.Panel1.ResumeLayout(False)
        Me.contJobs.Panel2.ResumeLayout(False)
        CType(Me.contJobs, System.ComponentModel.ISupportInitialize).EndInit()
        Me.contJobs.ResumeLayout(False)
        CType(Me.dgvStatus, System.ComponentModel.ISupportInitialize).EndInit()
        CType(Me.StatusEntryBindingSource, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents dgvJobs As System.Windows.Forms.DataGridView
    Friend WithEvents JobEntryBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents RequestIdColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents OriginalUriDisplay As System.Windows.Forms.DataGridViewLinkColumn
    Friend WithEvents OriginalLocationColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents FinalUriDisplayColumn As System.Windows.Forms.DataGridViewLinkColumn
    Friend WithEvents TileSizeColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents SubmissionTimeColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents FinalUriColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents OriginalUriColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents contJobs As System.Windows.Forms.SplitContainer
    Friend WithEvents StatusEntryBindingSource As System.Windows.Forms.BindingSource
    Friend WithEvents dgvStatus As System.Windows.Forms.DataGridView
    Friend WithEvents TimestampDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents MessageDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents RoleIdDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents RequestId As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents PartitionKeyDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn
    Friend WithEvents RowKeyDataGridViewTextBoxColumn As System.Windows.Forms.DataGridViewTextBoxColumn

End Class
