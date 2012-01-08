Imports System.IO
Imports System.Drawing
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices

Public Module ImageUtilities

    Public Function BytesPerPixel(image As Bitmap) As Byte

        ' calculate bytes per pixel from image type, these types will get
        ' benefit of faster image processing
        Select Case image.PixelFormat
            Case Imaging.PixelFormat.Format24bppRgb
                BytesPerPixel = 3
            Case Imaging.PixelFormat.Format32bppArgb
                BytesPerPixel = 4
            Case Imaging.PixelFormat.Format32bppRgb
                BytesPerPixel = 4
            Case Imaging.PixelFormat.Format8bppIndexed
                BytesPerPixel = 1
            Case Else
                BytesPerPixel = 0
        End Select
    End Function

    Public Function GetPixelColor(origImage As Bitmap, x As Int32, y As Int32, Optional bmpData As BitmapData = Nothing) As Color

        If bmpData Is Nothing Or BytesPerPixel(origImage) = 0 Then
            GetPixelColor = origImage.GetPixel(x, y)
        Else

            ' provide alternative implementation to boost performance - relies on LockBits
            ' see http://www.bobpowell.net/lockingbits.htm
            If (bmpData.PixelFormat And PixelFormat.Indexed) > 0 Then
                GetPixelColor = origImage.Palette.Entries(Marshal.ReadByte(bmpData.Scan0, y * bmpData.Stride + x))
            Else
                Dim A, R, G, B As Byte
                Dim pixelOffset = (y * bmpData.Stride) + (x * BytesPerPixel(origImage))

                B = Marshal.ReadByte(bmpData.Scan0, pixelOffset)
                G = Marshal.ReadByte(bmpData.Scan0, pixelOffset + 1)
                R = Marshal.ReadByte(bmpData.Scan0, pixelOffset + 2)
                If (bmpData.PixelFormat And PixelFormat.Alpha) > 0 Then
                    A = Marshal.ReadByte(bmpData.Scan0, pixelOffset + 3)
                Else
                    A = 255
                End If

                GetPixelColor = Color.FromArgb(A, R, G, B)
            End If
        End If
    End Function

    Public Function CalculateColorDistance(c1 As Color, c2 As Color) As Double

        ' CInt cast to prevent overflow when doing arithmetic on byte field
        Dim rDiff As Int32 = CInt(c1.R) - c2.R
        Dim gDiff As Int32 = CInt(c1.G) - c2.G
        Dim bDiff As Int32 = CInt(c1.B) - c2.B

        CalculateColorDistance = Math.Sqrt(rDiff * rDiff + gDiff * gDiff + bDiff * bDiff)
    End Function

    Public Function CalculateAverageColor(rawBytes As Byte()) As Color
        Dim RgbCount As Tuple(Of Int32, Int32, Int32) = Tuple.Create(0, 0, 0)

        Using m = New MemoryStream(rawBytes)
            Using img As Bitmap = CType(Image.FromStream(m), Bitmap)

                ' don't let the calculation get out of hand; image should be rescaled
                ' prior to coming here            
                If (img.Width > 64) Or (img.Height > 64) Then Return Color.Fuchsia

                For r As Int32 = 0 To img.Width - 1
                    For c As Int32 = 0 To img.Height - 1
                        Dim pixel = img.GetPixel(r, c)
                        RgbCount = Tuple.Create(RgbCount.Item1 + pixel.R, RgbCount.Item2 + pixel.G, RgbCount.Item3 + pixel.B)
                    Next
                Next

                Dim d = img.Width * img.Height
                CalculateAverageColor = Color.FromArgb(CInt(RgbCount.Item1 / d),
                                                 CInt(RgbCount.Item2 / d),
                                                 CInt(RgbCount.Item3 / d))
            End Using
        End Using
    End Function

    Public Function GetThumbnail(imgBytes As Byte(), size As Byte) As Byte()

        Using thumbNail = New Bitmap(size, size)
            Using gr = Graphics.FromImage(thumbNail)
                Using msIn = New MemoryStream(imgBytes), img As Bitmap = CType(Image.FromStream(msIn), Bitmap)

                    gr.InterpolationMode = Drawing2D.InterpolationMode.HighQualityBicubic
                    gr.SmoothingMode = Drawing2D.SmoothingMode.AntiAlias
                    gr.PixelOffsetMode = Drawing2D.PixelOffsetMode.HighQuality

                    gr.DrawImage(img, 0, 0, size, size)

                    Using msOut = New MemoryStream(thumbNail.Width * thumbNail.Height)
                        thumbNail.Save(msOut, System.Drawing.Imaging.ImageFormat.Jpeg)
                        GetThumbnail = msOut.ToArray()
                    End Using
                End Using
            End Using
        End Using
    End Function

    Public Function GetImageSize(bytes As Byte()) As Size
        Dim imgSize As Size

        If bytes IsNot Nothing Then
            Using ms = New MemoryStream(bytes)
                Using img = Image.FromStream(ms)
                    imgSize = New Size(img.Width, img.Height)
                End Using
            End Using
        End If
        GetImageSize = imgSize
    End Function

    Public Sub SaveAsFile(bytes As Byte(), fileName As String)
        If bytes IsNot Nothing Then
            Using ms = New MemoryStream(bytes)
                Using img = Image.FromStream(ms)
                    img.Save(fileName)
                End Using
            End Using
        End If
    End Sub

    Public Function GetBytesForFile(fileName As String, ByRef dimension As Size) As Byte()
        If File.Exists(fileName) Then
            Using img = Image.FromFile(fileName)
                dimension.Width = img.Width
                dimension.Height = img.Height

                Using m = New MemoryStream()
                    img.Save(m, Imaging.ImageFormat.Jpeg)

                    GetBytesForFile = m.ToArray()
                End Using
            End Using
        Else
            GetBytesForFile = Nothing
        End If
    End Function

    ' leverages Iterator and Yield functionality for Visual Basic in the Async CTP
    ' see http://msdn.microsoft.com/en-us/vstudio/async.aspx
    'Public Iterator Function SliceAndDice(imgBytes As Byte(), numSlices As Byte) As IEnumerable(Of Byte())

    '    Using ms = New MemoryStream(imgBytes)
    '        Using originalImage = Image.FromStream(ms)

    '            ' calculate slice dimensions (all horizontal slices)
    '            Dim sliceWidth As Int32 = originalImage.Width
    '            Dim sliceHeight As Int32 = originalImage.Height / numSlices

    '            ' can't have more slices than height
    '            If sliceHeight = 0 Then
    '                sliceHeight = 1
    '                numSlices = originalImage.Height
    '            End If

    '            ' calculate heights of each slice
    '            Dim sliceHeights = New List(Of Int32)
    '            If numSlices > 1 Then
    '                sliceHeights = Enumerable.Repeat(Of Int32)(originalImage.Height / numSlices, numSlices - 1).ToList()
    '                sliceHeights.Add(originalImage.Height - ((numSlices - 1) * sliceHeight))
    '            Else
    '                sliceHeights.Add(originalImage.Height)
    '            End If

    '            ' divvy up the slices
    '            Dim yOffset As Int32 = 0
    '            For slice As Int32 = 0 To numSlices - 1
    '                Using imgSlice = New Bitmap(originalImage.Width, sliceHeights(slice))
    '                    Using gr = Graphics.FromImage(imgSlice)
    '                        gr.DrawImage(originalImage, New Rectangle(0, 0, originalImage.Width, sliceHeights(slice)),
    '                                0, yOffset, originalImage.Width, sliceHeights(slice), GraphicsUnit.Pixel)

    '                        Using ms2 = New MemoryStream()
    '                            imgSlice.Save(ms2, System.Drawing.Imaging.ImageFormat.Jpeg)

    '                            Yield ms2.ToArray()
    '                        End Using
    '                    End Using
    '                End Using

    '                yOffset = yOffset + sliceHeight
    '            Next
    '        End Using
    '    End Using
    'End Function

    ' pre-Async CTP implementation of iterators for Visual Basic
    Public Function SliceAndDice(imgBytes As Byte(), numSlices As Byte) As IEnumerable(Of Byte())
        Dim slices = New List(Of Byte())(numSlices)

        Using ms = New MemoryStream(imgBytes)
            Using originalImage = Image.FromStream(ms)

                ' calculate slice dimensions (all horizontal slices)
                Dim sliceWidth As Int32 = originalImage.Width
                Dim sliceHeight As Int32 = originalImage.Height / numSlices

                ' can't have more slices than height
                If sliceHeight = 0 Then
                    sliceHeight = 1
                    numSlices = originalImage.Height
                End If

                ' calculate heights of each slice
                Dim sliceHeights = New List(Of Int32)
                If numSlices > 1 Then
                    sliceHeights = Enumerable.Repeat(Of Int32)(originalImage.Height / numSlices, numSlices - 1).ToList()
                    sliceHeights.Add(originalImage.Height - ((numSlices - 1) * sliceHeight))
                Else
                    sliceHeights.Add(originalImage.Height)
                End If

                ' divvy up the slices
                Dim yOffset As Int32 = 0
                For slice As Int32 = 0 To numSlices - 1
                    Using imgSlice = New Bitmap(originalImage.Width, sliceHeights(slice))
                        Using gr = Graphics.FromImage(imgSlice)
                            gr.DrawImage(originalImage, New Rectangle(0, 0, originalImage.Width, sliceHeights(slice)),
                                    0, yOffset, originalImage.Width, sliceHeights(slice), GraphicsUnit.Pixel)

                            Using ms2 = New MemoryStream()
                                imgSlice.Save(ms2, System.Drawing.Imaging.ImageFormat.Jpeg)
                                slices.Add(ms2.ToArray())
                            End Using
                        End Using
                    End Using

                    yOffset = yOffset + sliceHeight
                Next
            End Using
        End Using

        SliceAndDice = slices
    End Function

    Public Function StitchAndMend(imageUriList As IEnumerable(Of Uri), imageIterator As Func(Of IEnumerable(Of Uri), IEnumerable(Of Byte())), tileSize As Byte, originalSize As Size) As Byte()

        Dim finalBytes As Byte()

        Using finalImage = New Bitmap(originalSize.Width * tileSize, originalSize.Height * tileSize)
            Using gr = Graphics.FromImage(finalImage)

                ' loop through each slice and write to the appropriate location in finalImage
                Dim sliceSize As Size
                Dim yOffset As Int32 = 0

                For Each sliceBytes As Byte() In imageIterator(imageUriList)
                    Using ms = New MemoryStream(sliceBytes)
                        Using imgSlice = Image.FromStream(ms)
                            sliceSize = New Size(imgSlice.Width, imgSlice.Height)

                            gr.DrawImage(imgSlice, 0, yOffset)
                        End Using
                    End Using

                    yOffset = yOffset + sliceSize.Height
                Next


                ' write final image to byte stream
                Using ms = New MemoryStream()
                    finalImage.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg)
                    finalBytes = ms.ToArray()
                End Using
            End Using
        End Using

        StitchAndMend = finalBytes
    End Function
End Module