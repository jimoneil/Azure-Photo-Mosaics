Public Class AbstractNotificationForm
    Event RequestCompleted As Action(Of Object, RequestCompletedEventArgs)
    Public Sub OnRequestCompleted(sender As Object, e As RequestCompletedEventArgs)
        RaiseEvent RequestCompleted(sender, e)
    End Sub

    Event RequestStarted As Action(Of Object, RequestStartedEventArgs)
    Public Sub OnRequestStarted(sender As Object, e As RequestStartedEventArgs)
        RaiseEvent RequestStarted(sender, e)
    End Sub
End Class

Public Class RequestCompletedEventArgs
    Inherits System.EventArgs
    Private _imageUri As Uri
    Public ReadOnly Property ImageUri() As Uri
        Get
            Return _imageUri
        End Get
    End Property

    Private _requestId As Guid
    Public ReadOnly Property RequestId() As Guid
        Get
            Return _requestId
        End Get
    End Property

    Public Sub New(r As Guid, u As Uri)
        Me._requestId = r
        Me._imageUri = u
    End Sub
End Class

Public Class RequestStartedEventArgs
    Inherits System.EventArgs
    Private _imageUri As Uri
    Public ReadOnly Property ImageUri() As Uri
        Get
            Return _imageUri
        End Get
    End Property
    Private _imageSource As String
    Public ReadOnly Property ImageSource() As String
        Get
            Return _imageSource
        End Get
    End Property
    Private _tileSize As Byte
    Public ReadOnly Property TileSize() As Byte
        Get
            Return _tileSize
        End Get
    End Property
    Private _submissionDate As DateTime
    Public ReadOnly Property SubmissionDate() As DateTime
        Get
            Return _submissionDate
        End Get
    End Property
    Private _requestId As Guid
    Public ReadOnly Property RequestId() As Guid
        Get
            Return _requestId
        End Get
    End Property
    Public Sub New(r As Guid, s As String, u As Uri, z As Byte, t As DateTime)
        Me._requestId = r
        Me._imageUri = u
        Me._imageSource = s
        Me._tileSize = z
        Me._submissionDate = t
    End Sub
End Class