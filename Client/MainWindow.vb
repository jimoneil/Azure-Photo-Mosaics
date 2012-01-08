Imports System.Linq
Imports System.IO

Public Class MainWindow
    Const DEFAULT_TILE_SIZE As Byte = 16
    Private _originalImageSize As Size
    Private _selectedLibraryUri As Uri
    Private _clientId As String

    Public Sub New(cid As String)
        InitializeComponent()

        Me._clientId = cid
    End Sub

    Private Sub MainWindow_Load(sender As System.Object, e As System.EventArgs) Handles MyBase.Load

        nupTileDimension.Value = DEFAULT_TILE_SIZE
        picTile.Size = New Size(DEFAULT_TILE_SIZE, DEFAULT_TILE_SIZE)
        ofdPicture.InitialDirectory = Environment.SpecialFolder.MyPictures
        sfdMosaic.InitialDirectory = Environment.SpecialFolder.MyPictures

        Me.RefreshDisplay()
    End Sub

    Private Sub btnSelectOriginal_Click(sender As System.Object, e As System.EventArgs) Handles btnSelectOriginal.Click
        Select Case ofdPicture.ShowDialog()
            Case DialogResult.OK

                ' get (and save) image size to update UI elements that indicate how big resulting image will be
                Using originalImage = Image.FromFile(ofdPicture.FileName)
                    txtFileName.Text = ofdPicture.FileName
                    Me._originalImageSize = New Size(originalImage.Width, originalImage.Height)
                    Me.RefreshDisplay()
                End Using

            Case DialogResult.Cancel
            Case DialogResult.None
        End Select
    End Sub

    Private Sub btnViewOriginal_Click(sender As System.Object, e As System.EventArgs) Handles btnViewOriginal.Click
        Dim frmOriginalPreview = New ImagePreviewWindow("Image Preview", txtFileName.Text)
        frmOriginalPreview.ShowDialog()
    End Sub

    Private Sub btnSelectLibrary_Click(sender As System.Object, e As System.EventArgs) Handles btnSelectLibrary.Click

        Dim frmPickContainer = New ImageLibraryWindow(Me._clientId)
        Select Case frmPickContainer.ShowDialog()
            Case Windows.Forms.DialogResult.OK

                ' get the new container name
                Me._selectedLibraryUri = frmPickContainer.SelectedLibrary
                txtLibrary.Text = Me._selectedLibraryUri.AbsolutePath.Trim(New Char() {"/"})

                ' issue async request for all of the blobs in the container
                Dim client = New AzureStorageBroker.StorageBrokerClient()
                AddHandler client.EnumerateImagesCompleted, Sub(s As Object, e2 As AzureStorageBroker.EnumerateImagesCompletedEventArgs)
                                                                txtStatus.Text = String.Empty
                                                                If e2.Error IsNot Nothing Then
                                                                    MessageBox.Show(e2.Error.Message,
                                                                                    My.Application.Info.Title,
                                                                                    MessageBoxButtons.OK,
                                                                                    MessageBoxIcon.Exclamation)
                                                                Else
                                                                    lbImageLibrary.BackColor = SystemColors.Window
                                                                    lbImageLibrary.ValueMember = "FullUriString"
                                                                    lbImageLibrary.DisplayMember = "FriendlyName"
                                                                    lbImageLibrary.DataSource = (From b In e2.Result
                                                                         Order By b
                                                                         Select New With {.FullUriString = b,
                                                                                          .FriendlyName = System.Web.HttpUtility.UrlDecode(b.Substring(b.LastIndexOf("/") + 1))}).ToList()
                                                                End If
                                                                Me.RefreshDisplay()
                                                            End Sub

                ' change color of control while async retrieve is in progress
                lbImageLibrary.BackColor = SystemColors.ControlDark
                lbImageLibrary.DataSource = Nothing
                txtStatus.Text = "Enumerating list of images in library"
                client.EnumerateImagesAsync(frmPickContainer.SelectedLibrary)

            Case Windows.Forms.DialogResult.None
            Case Windows.Forms.DialogResult.Cancel
        End Select
    End Sub

    Private Sub btnCreateMosaic_Click(sender As System.Object, e As System.EventArgs) Handles btnCreateMosaic.Click

        ' submit request (if there's a file and a library!)
        Dim fileName As String = txtFileName.Text.Trim()
        If Not String.IsNullOrEmpty(fileName) And lbImageLibrary.Items.Count > 0 Then

            txtStatus.Text = "Submitting new request..."

            ' make call to service to submit new request            
            Dim client = New AzureJobBroker.JobBrokerClient()
            AddHandler client.SubmitImageForProcessingCompleted, Sub(s As Object, e2 As System.ComponentModel.AsyncCompletedEventArgs)
                                                                     If e2.Error IsNot Nothing Then
                                                                         txtStatus.Text = "New request could not be processed"
                                                                         MessageBox.Show(String.Format("New request could not be processed:{0}{0}{1}",
                                                                                                       Environment.NewLine,
                                                                                                        If(e2.Error.InnerException IsNot Nothing, e2.Error.InnerException.Message, e2.Error.Message)),
                                                                                          My.Application.Info.Title,
                                                                                          MessageBoxButtons.OK,
                                                                                          MessageBoxIcon.Exclamation)
                                                                     End If
                                                                 End Sub
            client.SubmitImageForProcessingAsync(
                        Main.UniqueUserId,
                        Guid.NewGuid(),
                        Me._selectedLibraryUri,
                        txtFileName.Text,
                        CByte(nupTileDimension.Value),
                        CByte(nupSlices.Value),
                        File.ReadAllBytes(fileName))
        End If
    End Sub

    Private Sub RefreshDisplay(Optional skipImageDownload As Boolean = False)

        ' update image size labels
        If Not String.IsNullOrEmpty(txtFileName.Text) Then
            lblOriginalSize.Text = String.Format("{0}x{1}", _originalImageSize.Width, _originalImageSize.Height)
            lblFinalSize.Text = String.Format("{0}x{1}", _originalImageSize.Width * nupTileDimension.Value,
                                                         _originalImageSize.Height * nupTileDimension.Value)
            btnViewOriginal.Enabled = True
            btnCreateMosaic.Enabled = lbImageLibrary.Items.Count > 0
        Else
            lblOriginalSize.Text = String.Empty
            lblFinalSize.Text = String.Empty

            btnViewOriginal.Enabled = False
            btnCreateMosaic.Enabled = False
        End If

        ' update label on group box to reflect size of image library
        If Not String.IsNullOrEmpty(txtLibrary.Text) Then
            grpImageLibrary.Text = String.Format("Image Library ({0} {1})",
                                                 lbImageLibrary.Items.Count,
                                                 IIf(lbImageLibrary.Items.Count = 1, "image", "images"))
        Else
            grpImageLibrary.Text = "Image Library"
        End If

        ' use direct blob access to show the tile
        If lbImageLibrary.SelectedIndex >= 0 And Not skipImageDownload Then
            picTile.ImageLocation = lbImageLibrary.SelectedValue
        Else
            picTile.ImageLocation = Nothing
        End If
    End Sub

    Private Sub lbImageLibrary_SelectedIndexChanged(sender As System.Object, e As System.EventArgs) Handles lbImageLibrary.SelectedIndexChanged
        Me.RefreshDisplay()
    End Sub

    Private Sub nupTileDimension_ValueChanged(sender As System.Object, e As System.EventArgs) Handles nupTileDimension.ValueChanged
        picTile.Size = New Size(nupTileDimension.Value, nupTileDimension.Value)
        Me.RefreshDisplay()
    End Sub

    Private Sub btnViewJobs_Click(sender As System.Object, e As System.EventArgs) Handles btnViewJobs.Click
        Dim frmJobList = JobListWindow.GetInstance(Main.UniqueUserId)
        frmJobList.Show()
    End Sub

    Private Sub MainWindow_RequestCompleted(sender As System.Object, e As PhotoMosaic.RequestCompletedEventArgs) Handles MyBase.RequestCompleted
        txtStatus.Text = String.Format("Request complete:{0}{0}{1}", Environment.NewLine, e.RequestId)
    End Sub

    Private Sub MainWindow_RequestStarted(sender As System.Object, e As PhotoMosaic.RequestStartedEventArgs) Handles MyBase.RequestStarted
        TextRenderer.MeasureText(e.ImageSource, txtStatus.Font, New Size(txtStatus.Width, 0), TextFormatFlags.ModifyString Or TextFormatFlags.PathEllipsis)
        txtStatus.Text = String.Format("Request started:{0}{0}Id:{1}{0}{2}", Environment.NewLine, e.RequestId, e.ImageSource)
    End Sub
End Class