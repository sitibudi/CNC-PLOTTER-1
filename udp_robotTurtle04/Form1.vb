Imports System.Net
Imports System.Net.Sockets
Imports System.Text

Public Class Form1
    Dim publisher As New Sockets.UdpClient(0)
    Dim subscriber As New Sockets.UdpClient(2265)
    Declare Sub Sleep Lib "kernel32.dll" (ByVal Milliseconds As Integer)
    Dim MyG As System.Drawing.Graphics
    Dim i, barisan, relX, relY, posX1, posY1, posX2, posY2, sudut, pena, arah As Integer
    Function sdt2rad(sdt As Integer) As Double
        Dim hsl As Double = Math.PI * sdt / 180
        sdt2rad = hsl
    End Function
    Private Sub segitiga(psdt As Integer, np As Byte)
        Dim xa, ya, xb, yb, xc, yc As Integer
        Dim msudut As Integer = psdt + 90
        xa = posX1 + (-5) * Math.Cos(sdt2rad(msudut)) ' - 0 * Math.Sin(sdt2rad(msudut))
        ya = posY1 + (-5) * Math.Sin(sdt2rad(msudut)) ' + 0 * Math.Cos(sdt2rad(msudut))
        xb = posX1 + (+5) * Math.Cos(sdt2rad(msudut)) ' - 0 * Math.Sin(sdt2rad(msudut))
        yb = posY1 + (+5) * Math.Sin(sdt2rad(msudut)) ' + 0 * Math.Cos(sdt2rad(msudut))
        xc = posX1 - ((-5) * Math.Sin(sdt2rad(msudut))) ' 0 * Math.Cos(sdt2rad(msudut)) - ((-5) * Math.Sin(sdt2rad(msudut)))
        yc = posY1 + ((-5) * Math.Cos(sdt2rad(msudut))) '0 * Math.Sin(sdt2rad(msudut)) + ((-5) * Math.Cos(sdt2rad(msudut)))
        Dim mypoint() As Drawing.Point =
             {
                 New Point(xa, ya),
                 New Point(xb, yb),
                 New Point(xc, yc),
                 New Point(xa, ya)
             }
        If pena = 0 Then
            MyG.DrawLines(Pens.WhiteSmoke, mypoint)
        Else
            MyG.DrawLines(Pens.Red, mypoint)
        End If
        'ListBox1.Items.Add(Str(xa) + "," + Str(ya) + " * " + Str(xb) + "," + Str(yb) + " * " + Str(xc) + "," + Str(yc))
    End Sub
    Private Sub penanaik()
        pena = 0
    End Sub
    Private Sub penaturun()
        pena = 1
    End Sub

    Private Sub maju(jarak As Integer)
        segitiga(sudut, 0) 'hapus segitga
        posX2 = posX1 + (jarak * Math.Cos(sdt2rad(arah)))
        posY2 = posY1 + (jarak * Math.Sin(sdt2rad(arah)))
        If pena = 1 Then
            MyG.DrawLine(Pens.DarkMagenta, posX1, posY1, posX2, posY2)
        Else
            'MyG.DrawLine(Pens.WhiteSmoke, posX1, posY1, posX2, posY2)
        End If
        posY1 = posY2
        posX1 = posX2
        segitiga(sudut, 1) 'gambar segitiga
        'Me.Refresh()
    End Sub
    Private Sub mundur(jarak As Integer)
        segitiga(sudut, 0) 'hapus segitga
        posX2 = posX1 - (jarak * Math.Cos(sdt2rad(arah)))
        posY2 = posY1 - (jarak * Math.Sin(sdt2rad(arah)))
        If pena = 1 Then
            MyG.DrawLine(Pens.DarkMagenta, posX1, posY1, posX2, posY2)
        Else
            'MyG.DrawLine(Pens.WhiteSmoke, posX1, posY1, posX2, posY2)
        End If
        posY1 = posY2
        posX1 = posX2
        segitiga(sudut, 1) 'gambar segitiga
        'Me.Refresh()
    End Sub
    Private Sub kiri(sdt As Integer)
        segitiga(sudut, 0) 'hapus segitga
        sudut = -90 - sdt
        arah = arah - sdt
        arah = arah Mod 360
        segitiga(sudut, 1) 'gambar segitga
    End Sub
    Private Sub kanan(sdt As Integer)
        segitiga(sudut, 0) 'hapus segitga
        sudut = -90 + sdt
        arah = arah + sdt
        arah = arah Mod 360
        segitiga(sudut, 1) 'gambar segitga
    End Sub

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As EventArgs) Handles MyBase.Load
        relX = 10 'koreksi koord X
        relY = 280 'koreksi koord Y
        posX1 = relX + 198
        posY1 = relY + 198
        sudut = -90
        arah = sudut
        pena = 0
        MyG = Me.CreateGraphics
        subscriber.Client.ReceiveTimeout = 100
        subscriber.Client.Blocking = False
        TextBox1.Text = "192.168.4.1"
        TextBox2.Text = "2265"
        TextBox3.Text = "penaturun()"
        TextBox5.Text = " "
        ListBox1.Items.Clear()
        Me.Refresh()
        Timer1.Start()
    End Sub

    Private Sub Button1_Click(ByVal sender As Object, ByVal e As EventArgs) Handles Button1.Click
        'If Len(Trim(TextBox3.Text)) > 0 And Mid(TextBox5.Text, 1, 3) = "ACK" Then
        '    publisher.Connect(TextBox1.Text, TextBox2.Text)
        '    Dim sendbytes() As Byte = Encoding.ASCII.GetBytes(TextBox3.Text)
        '    publisher.Send(sendbytes, sendbytes.Length)
        'End If
        MyG.Clear(Color.WhiteSmoke)
        MyG.DrawRectangle(Pens.Green, relX, relY, 400, 400)
        Sleep(250)
        penaturun()
        For i = 0 To 50
            For sd = 0 To 3
                maju(10)
                Sleep(10)
                'mundur(100)
                kanan(90)
                Sleep(25)
            Next sd
            kanan(60)
        Next i
        penanaik()
    End Sub
    Private Sub Timer1_Tick(ByVal sender As Object, ByVal e As EventArgs) Handles Timer1.Tick
        Try
            Dim ep As IPEndPoint = New IPEndPoint(IPAddress.Any, 2265)
            Dim rcvbytes() As Byte = subscriber.Receive(ep)
            TextBox4.Text = ep.Address.ToString()
            TextBox5.Text = Encoding.ASCII.GetString(rcvbytes)
            TextBox5.Refresh()
            'ListBox1.Items.Add(TextBox5.Text)
        Catch ex As Exception
            'Debug.Print(ex.Message)
        End Try
    End Sub
    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Dim baris = TextBox6.Lines()
        barisan = UBound(baris) - LBound(baris)
        ListBox1.Items.Clear()
        If barisan >= 0 And Mid(TextBox5.Text, 1, 3) = "ACK" Then
            Timer1.Stop()
            publisher.Connect(TextBox1.Text, TextBox2.Text)
            MyG.Clear(Color.WhiteSmoke)
            i = 0
            While i < barisan 'jika ada instruksi
                If Len(Trim(baris(i))) > 0 And Mid(TextBox5.Text, 1, 3) = "ACK" Then
                    ListBox1.Items.Add(Str(i + 1) & " ==> " & baris(i))
                    ListBox1.Refresh()
                    'kirim sintak
                    Dim sendbytes() As Byte = Encoding.ASCII.GetBytes(baris(i))
                    publisher.Send(sendbytes, sendbytes.Length)
                    While Not (Mid(TextBox5.Text, 1, 4) = "BUSY") 'tunggu sampai mendapat sinyal BUSY
                        Try
                            Dim ep As IPEndPoint = New IPEndPoint(IPAddress.Any, 2265)
                            Dim rcvbytes() As Byte = subscriber.Receive(ep)
                            TextBox4.Text = ep.Address.ToString()
                            TextBox5.Text = Encoding.ASCII.GetString(rcvbytes)
                            TextBox5.Refresh()
                        Catch ex As Exception
                            'Debug.Print(ex.Message)
                        End Try
                    End While
                    While (Mid(TextBox5.Text, 1, 4) = "BUSY")  'tunggu sampai sinyal tidak BUSY 
                        Try
                            Dim ep As IPEndPoint = New IPEndPoint(IPAddress.Any, 2265)
                            Dim rcvbytes() As Byte = subscriber.Receive(ep)
                            TextBox4.Text = ep.Address.ToString()
                            TextBox5.Text = Encoding.ASCII.GetString(rcvbytes)
                            TextBox5.Refresh()
                        Catch ex As Exception
                            'Debug.Print(ex.Message)
                        End Try
                    End While
                    ' terjemahkan sintak
                    Dim parts As String() = baris(i).Split(New Char() {"("c})
                    Dim part1 As String = parts(0)
                    Dim part2 As Integer = Val(parts(1))
                    If part1 = "maju" Then
                        maju(part2)
                    End If
                    If part1 = "mundur" Then
                        mundur(part2)
                    End If
                    If part1 = "kiri" Then
                        kiri(part2)
                    End If
                    If part1 = "kanan" Then
                        kanan(part2)
                    End If
                    If part1 = "penanaik" Then
                        penanaik()
                    End If
                    If part1 = "penaturun" Then
                        penaturun()
                    End If
                    i = i + 1
                Else
                    Exit While
                End If
            End While
            TextBox5.Text = " "
            TextBox5.Refresh()
            Timer1.Start()
        End If
    End Sub

    Private Sub Form1_Paint(sender As Object, e As PaintEventArgs) Handles Me.Paint
        MyG.DrawRectangle(Pens.Green, relX, relY, 400, 400)
        'MyG.DrawRectangle(Pens.Red, posX1, posY1, 4, 4)
    End Sub
End Class