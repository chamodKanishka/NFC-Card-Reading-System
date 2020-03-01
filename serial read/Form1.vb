Imports System.Data
Imports System.Data.SqlClient

Public Class Form1
    Dim RFID As String
    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Dim dt As TimeSpan = System.DateTime.Now.TimeOfDay
        MsgBox(dt)


    End Sub

    Private Sub Label1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Label1.Click

    End Sub

    Private Sub connectbtn_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles connectbtn.Click
        SerialPort1.Open()
        connectbtn.Enabled = False
        Button3.Enabled = True
        Button3.Visible = True
        connectbtn.Visible = False

    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        SerialPort1.Close()
        Button3.Enabled = False
        connectbtn.Enabled = True

    End Sub

    Dim In_or_Out As Integer = 0
    Private Sub SerialPort1_DataReceived(ByVal sender As Object, ByVal e As System.IO.Ports.SerialDataReceivedEventArgs) Handles SerialPort1.DataReceived
        RFID = SerialPort1.ReadLine

        Dim conn As New SqlConnection
        Dim cmd As New SqlCommand

        conn.ConnectionString = "Data Source=.\SQLEXPRESS;AttachDbFilename=C:\Users\User\Documents\Visual Studio 2008\Projects\serial read\serial read\Databasebus.mdf;Integrated Security=True;User Instance=True"
        conn.Open()

        cmd.Connection = conn
        cmd.CommandText = "SELECT * FROM Record WHERE CustomerID='" & RFID & "' AND count = 1"


        Dim reader As SqlDataReader = cmd.ExecuteReader()

        If reader.HasRows = True Then
            In_or_Out = 1 'Getting out of Bus
        Else
            In_or_Out = 0 'Getting in to the bus
        End If

        reader.Close()
        conn.Close()


        Dim conn2 As New SqlConnection
        Dim cmd2 As New SqlCommand

        conn2.ConnectionString = "Data Source=.\SQLEXPRESS;AttachDbFilename=C:\Users\User\Documents\Visual Studio 2008\Projects\serial read\serial read\Databasebus.mdf;Integrated Security=True;User Instance=True"
        conn2.Open()
        cmd2.Connection = conn2

        If In_or_Out = 0 Then
            cmd2.CommandText = "INSERT INTO Record (CustomerID,sTime,date,count) VALUES (@CustomerID,@sTime,@date,@count)"
            cmd2.Parameters.AddWithValue("CustomerID", RFID)
            cmd2.Parameters.AddWithValue("sTime", System.DateTime.Now.TimeOfDay)
            cmd2.Parameters.AddWithValue("date", System.DateTime.Now.Date)
            cmd2.Parameters.AddWithValue("count", 1)
            Try
                cmd2.ExecuteNonQuery()
            Catch ex As Exception
                MsgBox(ex.ToString())
            End Try
            conn2.Close()
        Else
            Dim cost As Integer = 123
            cmd2.CommandText = "UPDATE Record SET eTime=@eTime,cost=@cost,count=@count WHERE CustomerID='" & RFID & "' AND count = 1"
            cmd2.Parameters.AddWithValue("eTime", System.DateTime.Now.TimeOfDay)
            cmd2.Parameters.AddWithValue("cost", cost)
            cmd2.Parameters.AddWithValue("count", 2)
            Try
                cmd2.ExecuteNonQuery()
            Catch ex As Exception
                MsgBox(ex.ToString())
            End Try
            conn2.Close()
        End If

    End Sub
End Class
