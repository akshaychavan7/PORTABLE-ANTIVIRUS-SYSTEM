Imports System.Security.Cryptography
Imports System.IO
Imports System.Text
Imports System.Net
Imports System.IO.Compression

Public Class Form1


    Public Hashes As New List(Of String) 'List Of MD5 Hashes
    Public Infected_Files As New List(Of String) ' List Of Infected Files

    Dim Exclude_Files As Boolean = True
    Public Excludes_Files As New List(Of String)

    Dim Only_Scan_Above As Boolean = True
    Dim Only_Scan_Above_Number As Integer = 50

    Dim AutoMaticDelete As Boolean = False

    Dim TE As Long = 0
    Dim Md5 As String
    Dim Files_Detected As New List(Of String)
    Dim Filedl As New List(Of String)
    Private Sub File_Scanner_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles File_Scanner.RunWorkerCompleted
        MsgBox("Succeeded")
        Button1.Enabled = True
        'To get scan type
        Dim type As String
        ' To get Location
        Dim loc As String

        If RadioButton1.Checked Then
            type = RadioButton1.Text
            loc = "Full System"
        ElseIf RadioButton2.Checked Then
            type = RadioButton2.Text
            loc = "Quick"
        Else
            type = RadioButton3.Text
            loc = FolderBrowserDialog1.SelectedPath
        End If
        If My.Computer.FileSystem.FileExists("C:\Portable Antivirus\Antivirus Config\Log.txt") = True Then
            Using sw As StreamWriter = File.AppendText("C:\Portable Antivirus\Antivirus Config\Log.txt")
                sw.WriteLine("Type: " & type)
                sw.WriteLine("Location: " & loc)
                sw.WriteLine("Date: " & Date.Now)
                sw.WriteLine("Files Detected " & Filedl.Count)
                If Filedl.Count > 0 Then
                    sw.WriteLine("Detected Files: ")
                    For Each EF As String In Filedl
                        sw.WriteLine(EF)
                    Next
                End If
                sw.WriteLine("*********************************************************************************************************")
            End Using
        Else
            My.Computer.FileSystem.CreateDirectory("C:\Portable Antivirus\Antivirus Config")
            Using sw As StreamWriter = File.AppendText("C:\Portable Antivirus\Antivirus Config\Log.txt")
                sw.WriteLine("Type: " & type)
                sw.WriteLine("Location: " & loc)
                sw.WriteLine("Date: " & Date.Now)
                sw.WriteLine("Files Detected " & Filedl.Count)
                If Filedl.Count > 0 Then

                    sw.WriteLine("Detected Files: ")
                    For Each EF As String In Filedl
                        sw.WriteLine(EF)
                    Next
                End If
                sw.WriteLine("*********************************************************************************************************")
            End Using
        End If
        MetroLabel3.Text = "-"
        MetroLabel4.Text = "-"
        Filedl.Clear()
    End Sub
    Private Sub LoadVirusHashes_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles LoadVirusHashes.DoWork

        Try
            Panel2.Enabled = False
            ReportToolStripMenuItem.Enabled = False
            ProgressBar3.Visible = True
            If File.Exists("Hashes\VirusDef.txt") Then
                Dim xread As System.IO.StreamReader
                xread = File.OpenText("Hashes\VirusDef.txt")
                Do Until xread.EndOfStream
                    Hashes.Add(xread.ReadLine)

                Loop
                xread.Close()
            Else
                File.Create("Hashes\VirusDef.txt")
            End If

            Label10.Text = Hashes.Count
        Catch ex As Exception

        End Try

    End Sub

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        If File_Scanner.IsBusy = False Then

            If RadioButton1.Checked Then

                File_Scanner.RunWorkerAsync("Full Scan")

            ElseIf RadioButton2.Checked Then

                File_Scanner.RunWorkerAsync("Quick Scan")

            ElseIf RadioButton3.Checked Then

                If FolderBrowserDialog1.ShowDialog = Windows.Forms.DialogResult.OK Then

                    File_Scanner.RunWorkerAsync(FolderBrowserDialog1.SelectedPath)
                End If

            End If

        Else
            MsgBox("Still Running")
        End If
    End Sub

    Sub Clean_Scanning()
        Label3.Text = "Scan:  -"
        Label4.Text = "Name: -"
        Label5.Text = "Location: -"
        Label6.Text = "Length: -"
        Label7.Text = "Extension: -"
        Label8.Text = "Time Elapsed: -"
        Label9.Text = "Files Detected: -"
        Label11.Text = ""
        ProgressBar1.Value = 0
        ProgressBar2.Value = 0
        ProgressBar1.Maximum = 0
        ProgressBar2.Maximum = 0
    End Sub

    Private Sub File_Scanner_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles File_Scanner.DoWork

        On Error Resume Next
        Dim Folders As New List(Of DirectoryInfo)

        Dim Files As New List(Of String)
        Dim Ex_File As Boolean = Exclude_Files
        Dim Ex_Files As New List(Of String)

        Dim SW As New Stopwatch
        Dim Path_Scan As String = e.Argument.ToString
        Ex_Files.AddRange(Excludes_Files)
        Clean_Scanning()
        Button1.Enabled = False
        SW.Start()
        NotifyIcon1.Text = "Scanning: Analyzing..."
        Label11.Text = "Analyzing..."
        If Path_Scan = "Full Scan" Then

            Label3.Text = "Scan: Full Scan"
            For Each Driver As DriveInfo In My.Computer.FileSystem.Drives

                Folders.AddRange(GetFolders(Driver.Name))
                If File_Scanner.CancellationPending = True Then Exit For

            Next

        ElseIf Path_Scan = "Quick Scan" Then
            Label3.Text = "Scan: Quick Scan"
            For Each Directory_Info As String In SPECIALDIRECTORIESSS()

                If Directory.Exists(Directory_Info) Then

                    Folders.AddRange(GetFolders(Directory_Info))

                End If
                If File_Scanner.CancellationPending = True Then Exit For
            Next
        Else

            Label3.Text = "Scan: Custom Scan"
            Folders.AddRange(GetFolders(Path_Scan))

        End If
        Label11.Text = "Scanning"
        ProgressBar2.Value = 0
        ProgressBar2.Maximum = Folders.Count

        For Each Element_Directory As DirectoryInfo In Folders
            Files.Clear()
            Files.AddRange(Directory.GetFiles(Element_Directory.FullName, "*.*", SearchOption.TopDirectoryOnly))


            For Each f In Ex_Files
                If Files.Contains(f) = True Then
                    Files.Remove(f)
                End If
            Next f



            ProgressBar1.Value = 0
            ProgressBar1.Maximum = Files.Count
            For Each Element_File As String In Files
                NotifyIcon1.Text = "Scanning: " & Math.Round(((ProgressBar2.Value / ProgressBar2.Maximum) * 100), 2) & "%"
                Dim F As FileInfo
                F = My.Computer.FileSystem.GetFileInfo(Element_File)

                If (Ex_File = True AndAlso Ex_Files.Contains(F.Extension) = False) OrElse Ex_File = False Then

                    '------- DESIGNING
                    Label4.Text = "Name: " & F.Name
                    Label5.Text = "Location: " & F.FullName
                    Label6.Text = "Length: " & GetBytes(F.Length)
                    Label7.Text = "Extension: " & GetExtension(F.Extension)
                    Label8.Text = "Time Elapsed: " & SW.Elapsed.Hours & ":" & SW.Elapsed.Minutes & ":" & SW.Elapsed.Seconds
                    Label9.Text = "Files Detected: " & Files_Detected.Count
                    '--------------
                    Md5 = GetMd5(Element_File)
                    If Hashes.Contains(Md5) Then

                        If Not Md5 = "" Then

                            If Infected_Files.Contains(Element_File) = False And Files_Detected.Contains(Element_File) = False Then

                                Files_Detected.Add(Element_File)
                                Filedl.Add(Element_File)

                            End If

                        End If

                    End If

                    ProgressBar1.Value += 1
                    If File_Scanner.CancellationPending = True Then Exit For
                    MetroLabel4.Text = Math.Round(((ProgressBar1.Value / ProgressBar1.Maximum) * 100), 2) & "%"
                End If
            Next

            ProgressBar2.Value += 1
            If File_Scanner.CancellationPending = True Then Exit For
            MetroLabel3.Text = Math.Round(((ProgressBar2.Value / ProgressBar2.Maximum) * 100), 2) & "%"
        Next

        SW.Stop()
        For Each EF As String In Files_Detected
            If Infected_Files.Contains(EF) = False Then
                Infected_Files.Add(EF)
            End If
        Next
        Files_Detected.Clear()
        Folders.Clear()


    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        If Directory.Exists("C:\Portable Antivirus\Antivirus Config\") = False Then IO.Directory.CreateDirectory("C:\Portable Antivirus\Antivirus Config\")
        Dim Str As String = ""
        For Each S As String In Infected_Files
            Str &= S & vbNewLine
        Next
        My.Computer.FileSystem.WriteAllText("C:\Portable Antivirus\Antivirus Config\Virus Infected List.txt", Str, False)
    End Sub

    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        CheckForIllegalCrossThreadCalls = False
        LoadVirusHashes.RunWorkerAsync()
        Load_Exclude_Files()
        FileSystemWatcher1.EnableRaisingEvents = True
    End Sub

    Public Sub Load_Virus_List()

        If File.Exists("C:\Portable Antivirus\Antivirus Config\Virus Infected List.txt") Then

            Dim SR As New StreamReader("C:\Portable Antivirus\Antivirus Config\Virus Infected List.txt")
            Dim Str() As String = Split(SR.ReadToEnd, vbNewLine)
            SR.Close()

            For Each S As String In Str
                If Infected_Files.Contains(S) = False Then
                    If File.Exists(S) Then
                        Infected_Files.Add(S)
                    End If
                End If
            Next

        End If

    End Sub

    Public Sub Load_Exclude_Files()
        If File.Exists("C:\Portable Antivirus\Antivirus Config\Excluded Extension.txt") Then
            Excludes_Files.Clear()
            Dim SR As New StreamReader("C:\Portable Antivirus\Antivirus Config\Excluded Extension.txt")
            While SR.EndOfStream = False
                Excludes_Files.Add(SR.ReadLine)
            End While
            SR.Close()
        Else
            Excludes_Files.Clear()
        End If
    End Sub

    Sub FSW(ByVal Path As String)

        On Error Resume Next
        Dim F As FileInfo = My.Computer.FileSystem.GetFileInfo(Path)
        If (Exclude_Files = True AndAlso Excludes_Files.Contains(F.Extension) = False) OrElse Exclude_Files = False Then
            Dim Md5 As String = GetMd5(Path)
            If Hashes.Contains(Md5) Then
                If Not Md5 = "" Then
                    If Infected_Files.Contains(Path) = False Then
                        Infected_Files.Add(Path)
                    End If
                End If
            End If
        End If

    End Sub
    Private Sub Timer1_Tick(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Timer1.Tick

        If File_Scanner.IsBusy = True Then

            Panel1.Visible = False
            Panel1.Dock = DockStyle.None
            Panel3.Dock = DockStyle.Fill
            Panel3.Visible = True

        Else

            Panel1.Visible = True
            Panel1.Dock = DockStyle.Fill
            Panel3.Dock = DockStyle.None
            Panel3.Visible = False

        End If

        ' If Me.WindowState = FormWindowState.Minimized Then
        'NotifyIcon1.Visible = True
        'Me.Visible = False
        '  Else
        'NotifyIcon1.Visible = False
        'Me.Visible = True
        'End If
        If File_Scanner.IsBusy = False Then NotifyIcon1.Text = "Antivirus"
        If LoadVirusHashes.IsBusy = False Then
            If Infected_Files.Count > 0 Then
                PictureBox1.Image = My.Resources.Warning
                Label2.Text = "Vulnerable"
                If File_Scanner.IsBusy = False Then NotifyIcon1.BalloonTipText = "Your computer is currently in danger."
                Label1.Text = "Your computer is currently in danger."
            ElseIf FileSystemWatcher1.EnableRaisingEvents = False Then
                PictureBox1.Image = My.Resources.Alert
                Label2.Text = "Not Protected"
                If File_Scanner.IsBusy = False Then NotifyIcon1.BalloonTipText = "Your computer is currently not protected."
                Label1.Text = "Your computer is currently not protected."
            Else
                PictureBox1.Image = My.Resources.Protection1
                Label2.Text = "Protected"
                If File_Scanner.IsBusy = False Then NotifyIcon1.BalloonTipText = "Your computer is currently protected."
                Label1.Text = "Your computer is currently protected."
            End If
        Else
            PictureBox1.Image = My.Resources.Protection1
            Label2.Text = "Protected"
            Label1.Text = "Your computer is currently protected."
            If File_Scanner.IsBusy = False Then NotifyIcon1.BalloonTipText = "Your computer is currently protected."
        End If

        If AutoMaticDelete = True And Automatic_Delete_File.IsBusy = False Then Automatic_Delete_File.RunWorkerAsync()
        If LoadVirusHashes.IsBusy = True Then FileSystemWatcher1.EnableRaisingEvents = False
        If FileSystemWatcher1.EnableRaisingEvents = True Then
            OnToolStripMenuItem1.Enabled = False
            OffToolStripMenuItem1.Enabled = True
        Else
            OnToolStripMenuItem1.Enabled = True
            OffToolStripMenuItem1.Enabled = False
        End If

        If Exclude_Files = False Then
            OnToolStripMenuItem.Enabled = True
            OffToolStripMenuItem.Enabled = False
        Else
            OnToolStripMenuItem.Enabled = False
            OffToolStripMenuItem.Enabled = True
        End If

        If AutoMaticDelete = True Then
            OnToolStripMenuItem2.Enabled = False
            OffToolStripMenuItem2.Enabled = True
        Else
            OnToolStripMenuItem2.Enabled = True
            OffToolStripMenuItem2.Enabled = False
        End If

        Label4.Width = (Me.Width - 46) / 2
        Label5.Width = (Me.Width - 46) / 2
        Label6.Width = (Me.Width - 46) / 2
        Label7.Width = (Me.Width - 46) / 2
        Label8.Width = (Me.Width - 46) / 2
        Label9.Width = (Me.Width - 46) / 2
        Label5.Left = 15 + Label5.Width
        Label7.Left = 15 + Label7.Width
        Label9.Left = 15 + Label9.Width
        ToolStripStatusLabel3.Text = "Infected Files: " & Infected_Files.Count

    End Sub



    Public Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Dim result As Integer = MessageBox.Show("Do you want to stop scanning", "caption", MessageBoxButtons.YesNo)
        If result = DialogResult.Yes Then
            If File_Scanner.IsBusy = True Then
                File_Scanner.CancelAsync()
                MsgBox("Scanning Interrupted")
            End If
        End If

        If result = DialogResult.No Then
            MsgBox("Scanning Resumed")
        End If

    End Sub

    Private Sub Automatic_Delete_File_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles Automatic_Delete_File.DoWork


        On Error Resume Next
        For i1 = 0 To 1 Step 0
            If AutoMaticDelete = True Then
                For i = Infected_Files.Count - 1 To 0 Step -1

                    If File.Exists(Infected_Files(i)) Then

                        My.Computer.FileSystem.DeleteFile(Infected_Files(i))

                    End If

                    If File.Exists(Infected_Files(i)) = False Then

                        Infected_Files.RemoveAt(i)

                    End If

                Next
            Else
                Exit For
            End If
        Next
    End Sub

    Private Sub LoadVirusHashes_RunWorkerCompleted(ByVal sender As Object, ByVal e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles LoadVirusHashes.RunWorkerCompleted

        ProgressBar3.Visible = False
        ReportToolStripMenuItem.Enabled = True
        Panel2.Enabled = True
        Automatic_Delete_File.RunWorkerAsync()
        FileSystemWatcher1.EnableRaisingEvents = True
        Try
            Dim watcher(25) As System.IO.FileSystemWatcher
            Dim ascii As Integer = 67
            Dim a As String = ""
            For i = 0 To 25

                watcher(i) = New System.IO.FileSystemWatcher
                watcher(i).EnableRaisingEvents = False
            Next
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        Load_Virus_List()

    End Sub

    Private Sub OnToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OnToolStripMenuItem1.Click
        Try
            If LoadVirusHashes.IsBusy = False Then
                FileSystemWatcher1.EnableRaisingEvents = True
                Dim watcher(25) As System.IO.FileSystemWatcher
                Dim ascii As Integer = 67
                Dim a As String = ""
                For i = 0 To 25

                    watcher(i) = New System.IO.FileSystemWatcher
                    a = ChrW(ascii) & ":\"
                    watcher(i).Path = a
                    watcher(i).EnableRaisingEvents = True
                    AddHandler watcher(i).Changed, AddressOf FileSystemWatcher1_Changed
                    AddHandler watcher(i).Created, AddressOf FileSystemWatcher1_Created
                    AddHandler watcher(i).Renamed, AddressOf FileSystemWatcher1_Renamed
                    ascii = ascii + 1
                Next
            End If
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub OffToolStripMenuItem1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OffToolStripMenuItem1.Click
        Try
            FileSystemWatcher1.EnableRaisingEvents = False
            Dim watcher(25) As System.IO.FileSystemWatcher
            Dim ascii As Integer = 67
            Dim a As String = ""
            For i = 0 To 25

                watcher(i) = New System.IO.FileSystemWatcher
                watcher(i).EnableRaisingEvents = False
            Next
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub OnToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OnToolStripMenuItem.Click
        Try
            Exclude_Files = True
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub OffToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OffToolStripMenuItem.Click
        Try
            Exclude_Files = False
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub OnToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OnToolStripMenuItem2.Click
        Try
            AutoMaticDelete = True
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub OffToolStripMenuItem2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OffToolStripMenuItem2.Click
        Try
            AutoMaticDelete = False
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Private Sub Button4_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button4.Click
        Dim F As String
        For Each T In ListView1.Items
            F = T.Text
            My.Computer.FileSystem.DeleteFile(F)
            If File.Exists(F) = False Then
                If Infected_Files.Contains(F) Then
                    Infected_Files.RemoveAt(Infected_Files.IndexOf(F))
                End If
            End If
        Next
        ListView1.Items.Clear()
        For Each IFile As String In Infected_Files
            ListView1.Items.Add(IFile)
        Next
    End Sub

    Private Sub Button3_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button3.Click

        Dim F As String

        For Each T In ListView1.SelectedItems

            F = T.Text
            My.Computer.FileSystem.DeleteFile(F)
            If File.Exists(F) = False Then
                If Infected_Files.Contains(F) Then
                    Infected_Files.RemoveAt(Infected_Files.IndexOf(F))
                End If
            End If

        Next

        ListView1.Items.Clear()
        For Each IFile As String In Infected_Files
            ListView1.Items.Add(IFile)
        Next
    End Sub

    Private Sub ReportToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ReportToolStripMenuItem.Click
        Panel4.Dock = DockStyle.Fill
        Panel4.Visible = True
        Panel1.Visible = False
        Panel3.Visible = False
        Panel6.Visible = False
        Panel7.Visible = False
        MetroPanel1.Visible = False
        MetroPanel2.Visible = False
        ListView1.Items.Clear()
        Try


            For Each IFile As String In Infected_Files
                If File.Exists(IFile) = True Then
                    ListView1.Items.Add(IFile)
                Else
                    Infected_Files.Remove(IFile)
                End If
            Next
        Catch ex As Exception

        End Try
    End Sub

    Private Sub HomeToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles HomeToolStripMenuItem.Click

        If File_Scanner.IsBusy = True Then

            Panel3.Dock = DockStyle.Fill
            Panel3.Visible = True
            Panel1.Visible = False
            Panel4.Visible = False
            Panel6.Visible = False
            Panel7.Visible = False
            MetroPanel1.Visible = False
            MetroPanel2.Visible = False

        Else

            Panel1.Dock = DockStyle.Fill
            Panel1.Visible = True
            Panel3.Visible = False
            Panel4.Visible = False
            Panel6.Visible = False
            Panel7.Visible = False
            MetroPanel1.Visible = False
            MetroPanel2.Visible = False

        End If

    End Sub

    Private Sub OptionToolStripMenuItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles OptionToolStripMenuItem.Click
        Form2.ShowDialog()
    End Sub

    Private Sub NotifyIcon1_MouseDoubleClick(ByVal sender As System.Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles NotifyIcon1.MouseDoubleClick
        Me.Visible = True
        Me.WindowState = FormWindowState.Normal
    End Sub

    Private Sub HideUnhideDataToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles HideUnhideDataToolStripMenuItem.Click
        If File_Scanner.IsBusy = True Then

            Panel3.Dock = DockStyle.Fill
            Panel3.Visible = True
            Panel1.Visible = False
            Panel4.Visible = False
            Panel6.Visible = False
            Panel7.Visible = False
            MetroPanel1.Visible = False
            MetroPanel2.Visible = False
        ElseIf ReportToolStripMenuItem.Selected = True Then
            Panel4.Dock = DockStyle.Fill
            Panel1.Visible = False
            Panel3.Visible = False
            Panel6.Visible = False
            Panel4.Visible = True
            Panel7.Visible = False
            MetroPanel1.Visible = False
            MetroPanel2.Visible = False
        Else
            Panel6.Dock = DockStyle.Fill
            Panel1.Visible = False
            Panel3.Visible = False
            Panel4.Visible = False
            Panel6.Visible = True
            Panel7.Visible = False
            MetroPanel1.Visible = False
            MetroPanel2.Visible = False
        End If
    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If Label12.Text = "Directory" Then
            MsgBox("Please Select directory")
        Else
            Dim result As Integer = MessageBox.Show("It will Unhide Data, Do you Want to continue", "caption", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                Dim sb As New System.Text.StringBuilder
                sb.AppendLine("attrib -h -r -s -a /s /d " + Label12.Text + "*.*")
                sb.AppendLine("attrib -a - s - h - r " + Label12.Text + "\ \* /d /s")
                sb.AppendLine("del " & Label12.Text & "*.inf")
                sb.AppendLine("rd /s /q " & Label12.Text & "autorun.inf")
                sb.AppendLine("del file.bat")
                IO.File.WriteAllText("file.bat", sb.ToString())
                Process.Start("file.bat")
            End If
        End If

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        If Label12.Text = "Directory" Then
            MsgBox("Please Select directory")
        Else
            Dim result As Integer = MessageBox.Show("It will hide Data, Do you Want to continue", "caption", MessageBoxButtons.YesNo)
            If result = DialogResult.Yes Then
                Dim sb1 As New System.Text.StringBuilder
                sb1.AppendLine("attrib +h +r +s " + Label12.Text + "*.*")
                sb1.AppendLine("del " & Label12.Text & "*.inf")
                sb1.AppendLine("rd /s /q " & Label12.Text & "autorun.inf")
                sb1.AppendLine("del file.bat")
                IO.File.WriteAllText("file.bat", sb1.ToString())
                    Process.Start("file.bat")
                End If
            End If


    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        FolderBrowserDialog1.ShowDialog()
        If FolderBrowserDialog1.ShowDialog().Cancel = True Then
            Label12.Text = "Directory"
        Else
            Label12.Text = FolderBrowserDialog1.SelectedPath
        End If
    End Sub

    Private Sub RemoveShorcutlnkVirusToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemoveShorcutlnkVirusToolStripMenuItem.Click
        If File_Scanner.IsBusy = True Then
            Panel3.Dock = DockStyle.Fill
            Panel3.Visible = True
            Panel1.Visible = False
            Panel4.Visible = False
            Panel6.Visible = False
            Panel7.Visible = False
            MetroPanel1.Visible = False
            MetroPanel2.Visible = False
        ElseIf ReportToolStripMenuItem.Selected = True Then
            Panel4.Dock = DockStyle.Fill
            Panel1.Visible = False
            Panel3.Visible = False
            Panel6.Visible = False
            Panel4.Visible = True
            Panel7.Visible = False
            MetroPanel1.Visible = False
            MetroPanel2.Visible = False
        Else
            Panel7.Dock = DockStyle.Fill
            Panel1.Visible = False
            Panel3.Visible = False
            Panel4.Visible = False
            Panel6.Visible = False
            Panel7.Visible = True
            MetroPanel1.Visible = False
            MetroPanel2.Visible = False
        End If
    End Sub

    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click
        ListBox1.Items.Clear()
        FolderBrowserDialog1.ShowDialog()
        If FolderBrowserDialog1.ShowDialog().Cancel = True Then
            Label13.Text = "Directory"
        Else
            Label13.Text = FolderBrowserDialog1.SelectedPath
        End If
        ListBox1.Items.Clear()
        Try
            For Each filename As String In IO.Directory.GetFiles(Label13.Text, "*", IO.SearchOption.AllDirectories)
                Dim fName As String = IO.Path.GetExtension(filename)
                If fName = ".lnk" Then
                    ListBox1.Items.Add(filename)

                End If

            Next
        Catch ex As Exception
        End Try

    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        If Label13.Text = "Directory" Then
            MsgBox("Please Select directory")
        Else
            If Me.ListBox1.SelectedIndex >= 0 Then
                ' If ListBox1.SelectedItem = True Then
                Try
                    My.Computer.FileSystem.DeleteFile(ListBox1.SelectedItem)
                    ListBox1.Items.Remove(ListBox1.SelectedItem)
                Catch ex As Exception

                End Try
            Else
                MsgBox("No such file")
            End If
        End If


    End Sub

    Private Sub MetroButton1_Click(sender As Object, e As EventArgs) Handles MetroButton1.Click
        Dim sb1 As New StringBuilder
        If Label13.Text = "Directory" Then
            MsgBox("Please Select directory")

        Else
            If Label13.Text IsNot "Directory" Then
                If Me.ListBox1.Items.Count <= 0 Then
                    MsgBox("No such file")
                Else

                    Dim result As Integer = MessageBox.Show("It will delete file, Do you Want to continue", "Caution", MessageBoxButtons.YesNo)
                    If result = DialogResult.Yes Then
                        Try
                            For Each filename As String In IO.Directory.GetFiles(Label13.Text, "*", IO.SearchOption.AllDirectories)
                                Dim fName As String = IO.Path.GetExtension(filename)
                                If fName = ".lnk" Then
                                    sb1.AppendLine("del " & filename)
                                    My.Computer.FileSystem.DeleteFile(filename)
                                    ListBox1.Items.Remove(filename)
                                End If
                            Next
                            IO.File.WriteAllText("file.bat", sb1.ToString())
                            Process.Start("file.bat")
                            ListBox1.Items.Clear()
                        Catch ex As Exception
                        End Try
                    End If

                End If
            End If
        End If




    End Sub

    Private Sub UpdateDefinitionFromDirectoryToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles UpdateDefinitionFromDirectoryToolStripMenuItem.Click
        Dim path As String = Application.StartupPath + "\Hashes\VirusDef.txt"
        Using dialog As New OpenFileDialog
            Try
                If dialog.ShowDialog() <> DialogResult.OK Then Return
                If System.IO.Path.GetFileName(dialog.FileName) = "VirusDef.txt" Then
                    If File.Exists(path) = True Then
                        Dim result As Integer = MessageBox.Show("Want to continue", "caption", MessageBoxButtons.YesNo)
                        If result = DialogResult.Yes Then
                            File.Replace(dialog.FileName, path, "back")
                            MsgBox("Updated Successfully")
                        End If

                        If result = DialogResult.No Then
                            MsgBox("Failed to Update")
                        End If
                    Else
                        File.Copy(dialog.FileName, path)
                    End If
                Else
                    MsgBox("Please choose right file")
                End If

            Catch ex As Exception

            End Try
        End Using
    End Sub

    Private Sub LogsToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LogsToolStripMenuItem.Click
        If File_Scanner.IsBusy = True Then
            Panel3.Dock = DockStyle.Fill
            Panel3.Visible = True
            Panel1.Visible = False
            Panel4.Visible = False
            Panel6.Visible = False
            Panel7.Visible = False
            MetroPanel1.Visible = False
            MetroPanel2.Visible = False
        ElseIf ReportToolStripMenuItem.Selected = True Then
            Panel4.Dock = DockStyle.Fill
            Panel1.Visible = False
            Panel3.Visible = False
            Panel6.Visible = False
            Panel4.Visible = True
            Panel7.Visible = False
            MetroPanel1.Visible = False
            MetroPanel2.Visible = False
        Else
            MetroPanel1.Dock = DockStyle.Fill
            Panel1.Visible = False
            Panel3.Visible = False
            Panel4.Visible = False
            Panel6.Visible = False
            Panel7.Visible = False
            MetroPanel1.Visible = True
            MetroPanel2.Visible = True

        End If
        Try
            Dim allLines As String() = File.ReadAllLines("C:\Portable Antivirus\Antivirus Config\Log.txt")
            ListView2.Items.Clear()
            For Each line As String In allLines
                ListView2.Items.Add(line)
            Next
        Catch ex As Exception

        End Try
    End Sub

    Private Sub MetroButton3_Click(sender As Object, e As EventArgs) Handles MetroButton3.Click
        Try
            My.Computer.FileSystem.DeleteFile("C:\Portable Antivirus\Antivirus Config\Log.txt")
            If My.Computer.FileSystem.FileExists("C:\Portable Antivirus\Antivirus Config\Log.txt") = False Then
                MsgBox("Succeeded")
                ListView2.Items.Clear()
            End If
        Catch ex As Exception

        End Try
    End Sub

    Private Sub FileSystemWatcher1_Changed(ByVal sender As System.Object, ByVal e As System.IO.FileSystemEventArgs) Handles FileSystemWatcher1.Changed
        'Dim rpath As String
        Try
            Md5 = GetMd5(e.FullPath)
            If Hashes.Contains(Md5) Then

                If Not Md5 = "" Then

                    If Infected_Files.Contains(e.FullPath) = False And Files_Detected.Contains(e.FullPath) = False Then

                        Files_Detected.Add(e.FullPath)

                    End If

                End If
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub FileSystemWatcher1_Created(ByVal sender As System.Object, ByVal e As System.IO.FileSystemEventArgs) Handles FileSystemWatcher1.Created

        Try
            Md5 = GetMd5(e.FullPath)
            If Hashes.Contains(Md5) Then

                If Not Md5 = "" Then

                    If Infected_Files.Contains(e.FullPath) = False And Files_Detected.Contains(e.FullPath) = False Then

                        Files_Detected.Add(e.FullPath)

                    End If

                End If
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub FileSystemWatcher1_Renamed(ByVal sender As System.Object, ByVal e As System.IO.RenamedEventArgs) Handles FileSystemWatcher1.Renamed
        Try
            Md5 = GetMd5(e.FullPath)
            If Hashes.Contains(Md5) Then

                If Not Md5 = "" Then

                    If Infected_Files.Contains(e.FullPath) = False And Files_Detected.Contains(e.FullPath) = False Then

                        Files_Detected.Add(e.FullPath)

                    End If

                End If
            End If
        Catch ex As Exception

        End Try

    End Sub

End Class
