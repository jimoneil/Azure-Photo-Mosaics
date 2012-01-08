Imports System.IO
Imports System.Drawing
Imports System.Drawing.Imaging
Imports Utilities
Imports Microsoft.ApplicationServer.Caching
Imports CloudDAL

Public Class ImageLibrary
    ' 'cache' stats
    Public Property ImageRequests As Int32 = 0
    Public Property ImageRetrieves As Int32 = 0
    Public Property ColorValueRequests As Int32 = 0
    Public Property ColorValueRetrieves As Int32 = 0

    ' relevant only when using Blob storage caching
    Private _thumbnailContainer As Uri = Nothing
    Public ReadOnly Property ThumbnailContainer As Uri
        Get
            Return _thumbnailContainer
        End Get
    End Property

    ' relevant only when using AppFabric Caching
    Private _cache As DataCache = Nothing
    Public ReadOnly Property Cache() As DataCache
        Get
            Return _cache
        End Get
    End Property

    Private _container As Uri
    Public ReadOnly Property Container As Uri
        Get
            Return _container
        End Get
    End Property

    Private _tileAccessor As BlobAccessor
    Public ReadOnly Property TileAccessor As BlobAccessor
        Get
            Return _tileAccessor
        End Get
    End Property

    Private _tiles As List(Of Tile) = New List(Of Tile)
    Public ReadOnly Property Tiles() As IEnumerable(Of Tile)
        Get
            Return _tiles.AsEnumerable()
        End Get
    End Property

    Private _tileSize As Byte
    Public ReadOnly Property TileSize As Byte
        Get
            Return _tileSize
        End Get
    End Property

    Private Function InitializeAppFabricCache() As DataCache
        Dim theCache As DataCache

        Try
            theCache = New DataCacheFactory().GetDefaultCache()
        Catch ex As Exception
            Trace.TraceWarning("Cache could not be instantiated: {0}", ex.Message)
            theCache = Nothing
        End Try

        InitializeAppFabricCache = theCache
    End Function


    Public Sub New(accessor As BlobAccessor, container As Uri, size As Byte, Optional cacheType As CachingMechanism = CachingMechanism.InRole)
        _container = container
        _tileAccessor = accessor
        _tileSize = size

        If cacheType = CachingMechanism.AppFabric Then _cache = Me.InitializeAppFabricCache()
        If cacheType = CachingMechanism.BlobStorage Then _thumbnailContainer = accessor.CreateThumbnailContainer(container, size)

        For Each imgUri As Uri In accessor.RetrieveImageUris(container)
            _tiles.Add(TileFactory.CreateTile(Me, imgUri, cacheType))
        Next
    End Sub


    Public Function Mosaicize(imgBytesIn As Byte(), tileSize As Byte, Optional cb As Action(Of String) = Nothing) As Byte()

        Dim imgBytesOut As Byte()

        ' read the original image stream
        Using msIn = New MemoryStream(imgBytesIn)
            Using origImage As Bitmap = CType(Image.FromStream(msIn), Bitmap)


                ' create a new bitmap of requisite size
                Using mosaicImage As Bitmap = New Bitmap(origImage.Width * tileSize, origImage.Height * tileSize)

                    Dim gr As Graphics = Nothing
                    Try
                        gr = Graphics.FromImage(mosaicImage)

                        Dim pixelCount = origImage.Width * origImage.Height
                        Dim pixelsComplete = 0
                        Dim options = 0
                        Dim maxOptions = 0
                        Dim pctDone = 0

                        ' lock the image bytes for performance boost
                        Dim bmpData As BitmapData = Nothing
                        If ImageUtilities.BytesPerPixel(origImage) > 0 Then
                            bmpData = origImage.LockBits(New Rectangle(0, 0, origImage.Width, origImage.Height),
                                                                   ImageLockMode.ReadOnly,
                                                                   origImage.PixelFormat)
                        End If


                        ' loop through all the pixels
                        For origX As Int32 = 0 To origImage.Width - 1
                            Dim destX = origX * tileSize
                            For origY As Int32 = 0 To origImage.Height - 1
                                Dim destY = origY * tileSize

                                ' calculate 'color distances' from pixel in image to each tile and order the results
                                Dim pixelX = origX
                                Dim pixelY = origY
                                Dim tileOptions = From t In Me.Tiles
                                    Let d = ImageUtilities.CalculateColorDistance(
                                                 ImageUtilities.GetPixelColor(origImage, pixelX, pixelY, bmpData),
                                                 t.ColorValue)
                                    Order By d
                                    Select New With {.Tile = t, .Distance = d}


                                ' build a list of candidates that are 'close' in color
                                Dim candidates As List(Of Tile) = New List(Of Tile)
                                Dim closest As Double = tileOptions.First.Distance * 1.15
                                For Each t In tileOptions
                                    If (t.Distance <= closest) Then
                                        candidates.Add(t.Tile)
                                    Else
                                        Exit For
                                    End If
                                Next

                                ' pick a random tile from the candidates
                                Dim chosenTile As Tile = candidates.Skip(New Random(Environment.TickCount).Next(candidates.Count)).First()
                                If candidates.Count > 1 Then
                                    options = options + 1
                                    maxOptions = Math.Max(maxOptions, candidates.Count)
                                End If

                                ' write the mosaic-ized image
                                Using msOut = New MemoryStream(chosenTile.ImageBytes)
                                    Using imgOut = Image.FromStream(msOut)
                                        gr.DrawImage(imgOut, destX, destY)
                                    End Using
                                End Using

                                ' invoke callback function returning a string message
                                If cb IsNot Nothing Then
                                    pixelsComplete = pixelsComplete + 1
                                    Dim pctDoneNow = ((pixelsComplete * 100) \ pixelCount)
                                    If (pctDoneNow Mod 10 = 0) And (pctDoneNow > pctDone) Then
                                        pctDone = pctDoneNow
                                        cb(String.Format("{2}% Complete ({0}/{1} pixels)    [{3} - {4}]",
                                                         pixelsComplete, pixelCount, pctDone, options, maxOptions))
                                    End If
                                End If
                            Next
                        Next

                        If bmpData IsNot Nothing Then origImage.UnlockBits(bmpData)

                        Using msOut = New MemoryStream()
                            mosaicImage.Save(msOut, Imaging.ImageFormat.Jpeg)
                            imgBytesOut = msOut.ToArray()
                        End Using
                    Finally
                        gr.Dispose()
                    End Try
                End Using
            End Using
        End Using

        Return imgBytesOut
    End Function
End Class