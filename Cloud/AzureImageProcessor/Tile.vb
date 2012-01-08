Imports System.Drawing
Imports Utilities
Imports System.Runtime.Serialization
Imports System.Runtime.InteropServices

Public Enum CachingMechanism
    None = 0
    BlobStorage = 1
    AppFabric = 2
    InRole = 3
End Enum

Public Class TileFactory
    Public Shared Function CreateTile(library As ImageLibrary, uri As Uri, cacheType As CachingMechanism) As Tile
        Select Case cacheType
            Case CachingMechanism.InRole
                Return New InMemoryTile(library, uri)
            Case CachingMechanism.None
                Return New UncachedTile(library, uri)
            Case CachingMechanism.BlobStorage
                Return New BlobStorageTile(library, uri)
            Case CachingMechanism.AppFabric
                Return New CachedTile(library, uri)
            Case Else
                Return New InMemoryTile(library, uri)
        End Select
    End Function
End Class

<DataContract()>
Public Class CacheableTile
    <DataMember()>
    Public Property TileUri As Uri

    <DataMember()>
    Public Property ImageBytes As Byte()
End Class

Public MustInherit Class Tile

    Protected Property Library As ImageLibrary
    Protected Property TileUri As Uri

    Protected _imageBytes As Byte() = Nothing
    Public MustOverride ReadOnly Property ImageBytes As Byte()

    Private _colorValue As Color = Color.Empty
    Public ReadOnly Property ColorValue As Color
        Get
            Me.Library.ColorValueRequests += 1
            If _colorValue = Color.Empty Then
                Me.Library.ColorValueRetrieves += 1
                _colorValue = ImageUtilities.CalculateAverageColor(Me.ImageBytes)
            End If
            Return _colorValue
        End Get
    End Property

    Public Sub New(l As ImageLibrary, t As Uri)
        Me.TileUri = t
        Me.Library = l
    End Sub
End Class

' CachingMechanism.None
Public Class UncachedTile
    Inherits Tile

    Public Overrides ReadOnly Property ImageBytes As Byte()
        Get
            Me.Library.ImageRequests += 1
            Me.Library.ImageRetrieves += 1

            Dim fullImageBytes As Byte() = Me.Library.TileAccessor.RetrieveImage(Me.TileUri)
            Return ImageUtilities.GetThumbnail(fullImageBytes, Me.Library.TileSize)
        End Get
    End Property

    Public Sub New(l As ImageLibrary, t As Uri)
        MyBase.New(l, t)
    End Sub
End Class

' CachingMechanism.BlobStorage
Public Class BlobStorageTile
    Inherits Tile

    Public Overrides ReadOnly Property ImageBytes As Byte()
        Get
            Me.Library.ImageRequests += 1

            Dim sizedTileUri As Uri = New Uri(String.Format("{0}/{1}", Me.Library.ThumbnailContainer, Me.TileUri.Segments(2)))
            Dim thumbnailBytes As Byte() = Nothing
            Dim fullImageBytes As Byte() = Nothing

            If Not Me.Library.TileAccessor.TryRetrieveImage(sizedTileUri, thumbnailBytes) Then

                Me.Library.ImageRetrieves += 1
                fullImageBytes = Me.Library.TileAccessor.RetrieveImage(Me.TileUri)
                thumbnailBytes = ImageUtilities.GetThumbnail(fullImageBytes, Me.Library.TileSize)

                Me.Library.TileAccessor.StoreImage(thumbnailBytes, sizedTileUri)
            End If
            Return thumbnailBytes
        End Get
    End Property

    Public Sub New(l As ImageLibrary, t As Uri)
        MyBase.New(l, t)
    End Sub
End Class

' CachingMechanism.AppFabric
Public Class CachedTile
    Inherits Tile

    Public Overrides ReadOnly Property ImageBytes As Byte()
        Get

            Dim cachedTile As CacheableTile

            Me.Library.ImageRequests += 1
            Try
                cachedTile = CType(Me.Library.Cache.Get(Me.TileUri.ToString()), CacheableTile)
            Catch
                cachedTile = Nothing
            End Try

            If (cachedTile Is Nothing) Then
                Me.Library.ImageRetrieves += 1
                Trace.TraceInformation(String.Format("Cache miss {0}", Me.TileUri.ToString()))

                Dim fullImageBytes As Byte() = Me.Library.TileAccessor.RetrieveImage(Me.TileUri)

                cachedTile = New CacheableTile() With {
                        .TileUri = Me.TileUri,
                        .ImageBytes = ImageUtilities.GetThumbnail(fullImageBytes, Me.Library.TileSize)
                    }

                Me.Library.Cache.Put(Me.TileUri.ToString(), cachedTile)
            Else
                Trace.TraceInformation(String.Format("Cache hit {0}", Me.TileUri.ToString()))
            End If

            Return cachedTile.ImageBytes
        End Get
    End Property

    Public Sub New(l As ImageLibrary, t As Uri)
        MyBase.New(l, t)
    End Sub
End Class

' CachingMechanism.InRole
Public Class InMemoryTile
    Inherits Tile

    Public Overrides ReadOnly Property ImageBytes As Byte()
        Get
            Me.Library.ImageRequests += 1

            If _imageBytes Is Nothing Then
                Me.Library.ImageRetrieves += 1

                Dim fullImageBytes As Byte() = Me.Library.TileAccessor.RetrieveImage(Me.TileUri)
                _imageBytes = ImageUtilities.GetThumbnail(fullImageBytes, Me.Library.TileSize)
            End If
            Return _imageBytes
        End Get
    End Property

    Public Sub New(l As ImageLibrary, t As Uri)
        MyBase.New(l, t)
    End Sub
End Class




