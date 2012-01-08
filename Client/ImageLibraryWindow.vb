Public Class ImageLibraryWindow

    Private _selectedLibrary As Uri
    Public Property SelectedLibrary() As Uri
        Get
            Return _selectedLibrary
        End Get
        Private Set(ByVal value As Uri)
            _selectedLibrary = value
        End Set
    End Property

    Public Sub New(clientId As String)
        InitializeComponent()

        Dim client = New AzureStorageBroker.StorageBrokerClient()
        AddHandler client.EnumerateImageLibrariesCompleted, Sub(s As Object, e As AzureStorageBroker.EnumerateImageLibrariesCompletedEventArgs)
                                                                Me.UseWaitCursor = False
                                                                If e.Error IsNot Nothing Then
                                                                    txtStatus.Text = e.Error.Message
                                                                    txtStatus.ScrollBars = ScrollBars.Vertical
                                                                Else
                                                                    dgvImageLibraryList.DataSource =
                                                                                       (From c In CType(e.Result, IEnumerable(Of KeyValuePair(Of Uri, String)))
                                                                                        Let ShortName = c.Key.AbsolutePath.Trim(New Char() {"/"})
                                                                                        Order By ShortName
                                                                                        Select New With {.Uri = c.Key,
                                                                                                         .Name = ShortName,
                                                                                                         .Description = c.Value}).ToList()

                                                                    dgvImageLibraryList.Columns("Uri").Visible = False
                                                                    dgvImageLibraryList.Columns("Name").Width = dgvImageLibraryList.Width / 3
                                                                    txtStatus.Text = If(dgvImageLibraryList.RowCount = 0, "No image libraries found", "Select desired image library and press OK")

                                                                    OK_Button.Enabled = dgvImageLibraryList.SelectedRows.Count > 0
                                                                End If
                                                            End Sub
        Me.UseWaitCursor = True
        client.EnumerateImageLibrariesAsync(clientId)
    End Sub

    Private Sub Cancel_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Cancel_Button.Click
        Me.SelectedLibrary = Nothing
        Me.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.Close()
    End Sub

    Private Sub OK_Button_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OK_Button.Click
        Me.SelectAndReturn()
    End Sub

    Private Sub dgvImageLibraryList_DoubleClick(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs)
        Me.SelectAndReturn()
    End Sub

    Private Sub dgvImageLibraryList_DoubleClick(sender As System.Object, e As System.EventArgs) Handles dgvImageLibraryList.DoubleClick
        Me.SelectAndReturn()
    End Sub

    Private Sub SelectAndReturn()
        If dgvImageLibraryList.SelectedRows.Count > 0 Then
            Me.SelectedLibrary = dgvImageLibraryList.Rows(dgvImageLibraryList.SelectedRows(0).Index).Cells("Uri").Value
            Me.DialogResult = System.Windows.Forms.DialogResult.OK
            Me.Close()
        End If
    End Sub
End Class
