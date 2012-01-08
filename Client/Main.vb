Imports System.ComponentModel
Module Main
    Sub Main()

        ' set up background task to register hosted service with service bus
        Dim worker As New BackgroundWorker()
        AddHandler worker.DoWork, Sub(s As Object, e As DoWorkEventArgs)
                                      Dim clientId = CStr(e.Argument)
                                      NotificationServiceEndpoint.Register(clientId)
                                  End Sub
        AddHandler worker.RunWorkerCompleted, Sub(s As Object, e As RunWorkerCompletedEventArgs)
                                                  If e.Error IsNot Nothing Then
                                                      Dim msg = e.Error.Message
                                                      If e.Error.InnerException IsNot Nothing Then
                                                          msg = String.Format("{0}{1}    ({2})", msg, Environment.NewLine, e.Error.InnerException.Message)
                                                      End If
                                                      MessageBox.Show(String.Format("Notification functionality not available:{0}{0}{1}", Environment.NewLine, msg),
                                                                      My.Application.Info.Title,
                                                                      MessageBoxButtons.OK,
                                                                      MessageBoxIcon.Information)
                                                  End If
                                              End Sub
        worker.RunWorkerAsync(UniqueUserId)

        ' create and open a main window
        Dim mainForm As New MainWindow(UniqueUserId)
        Application.Run(mainForm)

        ' shut down notification service
        Try
            NotificationServiceEndpoint.Unregister()
        Catch
            ' we're exiting program so if endpoint doesn't close, we don't really care :)
        End Try
    End Sub

    Function UniqueUserId() As String
        UniqueUserId = System.Security.Principal.WindowsIdentity.GetCurrent().User.Value
    End Function
End Module