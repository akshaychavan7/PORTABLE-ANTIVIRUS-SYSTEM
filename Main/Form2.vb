Imports System.IO

Public Class Form2

    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        On Error Resume Next
        For Each SI In ListView1.SelectedItems
            SI.Remove()
        Next
    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click
        Try
            If TextBox1.Visible = True Then
            ListView1.Items.Add("." & TextBox1.Text)
        End If
        If MetroComboBox1.Visible = True Then
            ListView1.Items.Add(MetroComboBox1.SelectedItem)
        End If
        Dim itemI As ListViewItem
        Dim itemJ As ListViewItem
        Dim progress As Integer
        Dim count As Integer
        Dim ProgressDupCounter As Integer = ListView1.Items.Count
            For i As Integer = ListView1.Items.Count - 1 To 0 Step -1
                itemI = ListView1.Items(i)
                progress = progress + 1

                ' start one after hence +1
                For z As Integer = i + 1 To ListView1.Items.Count - 1 Step 1
                    itemJ = ListView1.Items(z)
                    If itemI.Text = itemJ.Text Then
                        'duplicate found, now delete duplicate
                        ListView1.Items.Remove(itemJ)
                        MsgBox("Already Exists")
                        count = count + 1
                        Exit For
                    End If
                Next z
            Next (i)

        Catch ex As Exception

        End Try
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click
        On Error Resume Next
        If Directory.Exists("C:\Portable Antivirus\Antivirus Config\") = False Then IO.Directory.CreateDirectory("C:\Portable Antivirus\Antivirus Config\")
        Dim Str As String = ""
        For Each Element_Items In ListView1.Items
            Str &= Element_Items.Text & vbNewLine
        Next
        File.WriteAllText("C:\Portable Antivirus\Antivirus Config\Excluded Extension.txt", Str)
        Form1.Load_Exclude_Files()
        TextBox1.Text = ""
    End Sub

    Private Sub TextBox1_GotFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBox1.GotFocus
        If TextBox1.Text = "Extension" Then
            TextBox1.Text = ""
        End If
    End Sub

    Private Sub TextBox1_LostFocus(ByVal sender As Object, ByVal e As System.EventArgs) Handles TextBox1.LostFocus
        If TextBox1.Text = "" Then
            TextBox1.Text = "Extension"
        End If
    End Sub

    Private Sub Form2_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load

        ListView1.Items.Clear()
        For Each S As String In Form1.Excludes_Files
            ListView1.Items.Add(S)
        Next

    End Sub


    Private Sub MetroLabel1_Click(sender As Object, e As EventArgs) Handles MetroLabel1.Click
        If MetroLabel1.Text = "Show list " Then
            TextBox1.Visible = False
            MetroComboBox1.Visible = True
        Else
            TextBox1.Visible = True
            MetroComboBox1.Visible = False
        End If
        If MetroComboBox1.Visible = False Then
            TextBox1.Visible = True
            MetroLabel1.Text = "Show list "
        Else
            TextBox1.Visible = False
            MetroLabel1.Text = "Add own Extension "
        End If
    End Sub

    Private Sub MetroLabel2_Click(sender As Object, e As EventArgs) Handles MetroLabel2.Click
        Dim a As String
        Using dialog As New OpenFileDialog
            Try
                If dialog.ShowDialog() <> DialogResult.OK Then Return
                a = dialog.FileName
                ListView1.Items.Add(a)
            Catch ex As Exception
            End Try
        End Using
        Dim itemI As ListViewItem
        Dim itemJ As ListViewItem
        Dim progress As Integer
        Dim count As Integer
        Dim ProgressDupCounter As Integer = ListView1.Items.Count
        For i As Integer = ListView1.Items.Count - 1 To 0 Step -1
            itemI = ListView1.Items(i)
            progress = progress + 1

            ' start one after hence +1
            For z As Integer = i + 1 To ListView1.Items.Count - 1 Step 1
                itemJ = ListView1.Items(z)
                If itemI.Text = itemJ.Text Then
                    'duplicate found, now delete duplicate
                    ListView1.Items.Remove(itemJ)
                    MsgBox("Already Exists")
                    count = count + 1
                    Exit For
                End If
            Next z
        Next (i)



    End Sub
End Class