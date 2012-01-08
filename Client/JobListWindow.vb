Imports System.ComponentModel
Public Class JobListWindow
    Private _uniqueUserId As String

    Private Shared _instance As JobListWindow
    Public Shared Function GetInstance(uid As String)
        If _instance Is Nothing Then
            _instance = New JobListWindow(uid)
        End If
        _instance.BringToFront()
        GetInstance = _instance
    End Function

    Private Sub New(uid As String)
        InitializeComponent()
        _uniqueUserId = uid
    End Sub

    Private Sub JobListWindow_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load
        Dim client = New AzureStorageBroker.StorageBrokerClient()
        AddHandler client.GetJobsForClientCompleted, Sub(s As Object, e2 As AzureStorageBroker.GetJobsForClientCompletedEventArgs)
                                                         dgvJobs.UseWaitCursor = False
                                                         If e2.Error IsNot Nothing Then
                                                             MessageBox.Show(e2.Error.Message,
                                                                             My.Application.Info.Title,
                                                                             MessageBoxButtons.OK,
                                                                             MessageBoxIcon.Exclamation)
                                                         Else
                                                             JobEntryBindingSource.DataSource =
                                                                 New BindingList(Of JobViewData)(
                                                                        (From j In CType(e2.Result, IEnumerable(Of AzureStorageBroker.JobEntry))
                                                                        Select New JobViewData() With {.RequestId = j.RequestId,
                                                                                                     .OriginalUri = New Uri(j.SourceImageUri),
                                                                                                     .OriginalLocation = j.SourceImage,
                                                                                                     .TileSize = j.TileSize,
                                                                                                     .FinalUri = If(String.IsNullOrEmpty(j.FinalImageUri), Nothing, New Uri(j.FinalImageUri)),
                                                                                                     .SubmissionTime = j.StartTime.ToLocalTime()
                                                                                                      }).ToList())
                                                             dgvJobs_RowEnter(dgvJobs, New DataGridViewCellEventArgs(0, 0))
                                                         End If
                                                     End Sub
        dgvJobs.UseWaitCursor = True
        client.GetJobsForClientAsync(Me._uniqueUserId)
    End Sub

    Private Sub JobListWindow_FormClosed(sender As System.Object, e As System.Windows.Forms.FormClosedEventArgs) Handles MyBase.FormClosed
        _instance = Nothing
    End Sub

    Private Sub dgvJobs_CellContentClick(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvJobs.CellContentClick
        Dim frmPreviewImage As ImagePreviewWindow
        Select Case dgvJobs.Columns.Item(e.ColumnIndex).DataPropertyName
            Case "OriginalUriDisplay"
                frmPreviewImage = New ImagePreviewWindow("Original Image", dgvJobs.Item("OriginalUriColumn", e.RowIndex).Value.ToString())
                frmPreviewImage.Show()
            Case "FinalUriDisplay"
                frmPreviewImage = New ImagePreviewWindow("Generated Image", dgvJobs.Item("FinalUriColumn", e.RowIndex).Value.ToString())
                frmPreviewImage.Show()
        End Select
    End Sub

    Private Function FindEntry(requestId As Guid) As JobViewData
        Dim dataSrc = JobEntryBindingSource

        dataSrc.MoveFirst()
        For pos As Int32 = 0 To dataSrc.Count - 1
            Dim jobInfo = CType(dataSrc.Current, JobViewData)
            If jobInfo.RequestId = requestId Then
                Return jobInfo
            Else
                dataSrc.MoveNext()
            End If
        Next

        FindEntry = Nothing
    End Function

    Private Sub JobListWindow_RequestCompleted(sender As System.Object, e As PhotoMosaic.RequestCompletedEventArgs) Handles MyBase.RequestCompleted
        Dim jobInfo = Me.FindEntry(e.RequestId)
        If jobInfo IsNot Nothing Then
            jobInfo.FinalUri = e.ImageUri
        End If
    End Sub

    Private Sub JobListWindow_RequestStarted(sender As System.Object, e As PhotoMosaic.RequestStartedEventArgs) Handles MyBase.RequestStarted
        If Me.FindEntry(e.RequestId) Is Nothing Then

            JobEntryBindingSource.Add(New JobViewData() With {
                .RequestId = e.RequestId,
                .OriginalUri = e.ImageUri,
                .OriginalLocation = e.ImageSource,
                .TileSize = e.TileSize,
                .FinalUri = Nothing,
                .SubmissionTime = e.SubmissionDate.ToLocalTime()
            })
        End If
    End Sub

    Private Sub dgvJobs_RowEnter(sender As System.Object, e As System.Windows.Forms.DataGridViewCellEventArgs) Handles dgvJobs.RowEnter
        Dim requestId As Guid
        If dgvJobs.CurrentRow IsNot Nothing Then

            If Guid.TryParse(dgvJobs.Item("RequestIdColumn", e.RowIndex).Value.ToString(), requestId) Then

                Dim client = New AzureStorageBroker.StorageBrokerClient()
                AddHandler client.GetStatusEntriesForJobCompleted, Sub(s As Object, e2 As AzureStorageBroker.GetStatusEntriesForJobCompletedEventArgs)
                                                                       dgvStatus.UseWaitCursor = False
                                                                       If e2.Error IsNot Nothing Then
                                                                           MessageBox.Show(e2.Error.Message,
                                                                                           My.Application.Info.Title,
                                                                                           MessageBoxButtons.OK,
                                                                                           MessageBoxIcon.Exclamation)
                                                                       Else
                                                                           StatusEntryBindingSource.DataSource =
                                                                                     (From status In CType(e2.Result, IEnumerable(Of AzureStorageBroker.StatusEntry))
                                                                                      Select New With {.RequestId = status.RequestId,
                                                                                                       .Timestamp = status.Timestamp.ToLocalTime(),
                                                                                                       .Message = status.Message}).ToList()
                                                                       End If
                                                                   End Sub
                dgvStatus.UseWaitCursor = True
                client.GetStatusEntriesForJobAsync(Me._uniqueUserId, requestId)
            Else
                StatusEntryBindingSource.DataSource = Nothing
            End If
        End If
    End Sub

    Private Sub dgvStatus_HeaderMouseClick(sender As System.Object, e As System.Windows.Forms.DataGridViewCellMouseEventArgs) Handles dgvStatus.ColumnHeaderMouseClick
        If dgvJobs.CurrentRow IsNot Nothing Then
            dgvJobs_RowEnter(sender, New DataGridViewCellEventArgs(0, dgvJobs.CurrentRow.Index))
        End If
    End Sub
End Class

Public Class JobViewData
    Implements INotifyPropertyChanged

    Public Property RequestId As Guid
    Public Property OriginalUri As Uri
    Public Property OriginalLocation As String
    Public Property SubmissionTime As DateTime
    Public Property TileSize As Int32
    Public ReadOnly Property OriginalUriDisplay() As String
        Get
            Return "View original..."
        End Get
    End Property
    Public ReadOnly Property FinalUriDisplay() As String
        Get
            Return IIf(Me.FinalUri Is Nothing, "", "View mosaic...")
        End Get
    End Property
    Private _finalUri As Uri
    Public Property FinalUri() As Uri
        Get
            Return Me._finalUri
        End Get
        Set(ByVal value As Uri)
            If value <> Me._finalUri Then
                Me._finalUri = value
                NotifyPropertyChanged("FinalUriDisplay")
                NotifyPropertyChanged("FinalUri")
            End If
        End Set
    End Property

    Public Event PropertyChanged As PropertyChangedEventHandler Implements INotifyPropertyChanged.PropertyChanged
    Private Sub NotifyPropertyChanged(info As String)
        RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(info))
    End Sub
End Class