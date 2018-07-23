Imports System.Security.Cryptography
Imports System.IO.Compression
Imports System.Text
Imports System.IO
Imports Microsoft.Win32

Module Module1

    Public Function GetContentUrl(ByVal url As String) As String
        Return New System.Net.WebClient().DownloadString(url)
    End Function

    'To Get A Extension Name Sample ".txt" will become Text Document
    Public Function GetExtension(ByVal value As String) As String
        On Error GoTo Danger
        Dim RegK As RegistryKey
        Dim T1, T2 As String
        RegK = Registry.ClassesRoot.OpenSubKey(value, False)
        T1 = RegK.GetValue("")
        RegK = Registry.ClassesRoot.OpenSubKey(T1, False)
        T2 = RegK.GetValue("")
        Return T2
        GoTo Safe
Danger:
        If value = "" Then Return "Unknown" Else Return value
Safe:
    End Function

    ' To Get The Shortcut Byte Of A File Sample 5Mb 10GB
    Public Function GetBytes(ByVal value As Decimal) As String

        Dim V As Decimal = value
        Dim T1 As Integer = 0
        Dim BytesList As New List(Of String)
        BytesList.Add("B")
        BytesList.Add("KB")
        BytesList.Add("MB")
        BytesList.Add("GB")
        BytesList.Add("TB")
        BytesList.Add("PB")
        BytesList.Add("EB")
        BytesList.Add("ZB")
        BytesList.Add("YB")
        If value >= 1024 Then
            Do While V >= 1024
                V /= 1024
                T1 += 1
            Loop
        End If

        Return Math.Round(V, 2) & BytesList(T1)

    End Function

    Public Function GetMd5(ByVal Path As String) As String

        On Error Resume Next

        Dim Md5 As New MD5CryptoServiceProvider
        Dim F As New FileStream(Path, FileMode.Open, FileAccess.Read, FileShare.Read, 8192)
        Md5.ComputeHash(F)

        'Dim Hash As Byte() = 
        Dim Buff As New StringBuilder
        Dim HashByte As Byte

        For Each HashByte In Md5.Hash
            Buff.Append(String.Format("{0:X2}", HashByte))
        Next

        Return Buff.ToString.ToLower

    End Function

    Public Function GetFolders(ByVal Path As String) As List(Of DirectoryInfo)

        On Error Resume Next
        Dim L As New List(Of DirectoryInfo)
        Dim D As New DirectoryInfo(Path)
        L.Add(D)
        For Each DD As DirectoryInfo In D.GetDirectories("*.*", SearchOption.TopDirectoryOnly)
            L.AddRange(GetFolders(DD.FullName))
        Next
        Return L

    End Function

    Public Function SPECIALDIRECTORIESSS() As List(Of String)
        On Error Resume Next
        Dim Folders_Special As New List(Of String)
        Dim Folders As New List(Of DirectoryInfo)
        Dim D As DirectoryInfo
        ' Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData))
        ' Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData))
        ' Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory))
        ' Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments))
        ' Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic))
        ' Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.CommonPictures))
        ' Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.CommonProgramFiles))
        ' Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu))
        'Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.CommonTemplates))
        'Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos))
        Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.Desktop))
        Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.Favorites))
        ' Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.Fonts))
        Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData))
        Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments))
        Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyMusic))
        Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures))
        ' Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos))
        ' Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.Resources))
        ' Folders_Special.Add(Environment.GetFolderPath(Environment.SpecialFolder.Windows))
        For Each Fol As String In Folders_Special
            D = New DirectoryInfo(Fol)
            Folders.Add(D)
        Next
        Return Folders_Special
    End Function

End Module
