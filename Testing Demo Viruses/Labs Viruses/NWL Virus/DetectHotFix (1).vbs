'Written by YANGYONG 2013-3-30

dim flagfile
arHotFixID = Array("KB2471472","KB982316","KB2378111","Q147222")
WScript.Echo  vbCrLf &  "Checking HotFixID: "  &  join(arHotFixID,",") &  vbCrLf

Function pwd()
	WScript.Echo  vbCrLf  & "Check Script Locate Directioy..."  &  vbCrLf
	Dim path
	Dim dir
	path = WScript.ScriptFullName
	dir = PathRemoveFileSpec( path )
	WScript.Echo "Script Location: "  & path
	WScript.Echo "Script Location Directory: " &  dir
	pwd = dir
End Function

Function PathRemoveFileSpec( strFileName )
  Dim iPos
  strFileName = Replace(strFileName, "/", "\")
  iPos = InStrRev(strFileName, "\")
  PathRemoveFileSpec = Left(strFileName, iPos)
End Function

Function DeleteFlgFile( flagfile )
	WScript.Echo  vbCrLf &  "Delete Flag File : " & flagfile  &  vbCrLf
	dim objFSO
	dim file
	Set objFSO = createObject("Scripting.FilesystemObject")
	Set file = objFSO.CreateTextFile(flagfile, True ) 'make sure file exist
	file.close
	objFSO.Deletefile flagfile, True 
End Function

Function WriteFlgFile( flagfile, hotfixid )
	WScript.Echo  vbCrLf &  "Write Flag File : " & flagfile  &  vbCrLf
	dim objFSO
	dim file
	Set objFSO = createObject("Scripting.FilesystemObject")
	Set file = objFSO.CreateTextFile(flagfile, True ) 
	file.WriteLine( hotfixid )
	file.close
End Function

flagfile = pwd & "\HOTFIX.FLG"
DeleteFlgFile( flagfile )

strComputer = "."
Set objSWbemServices = GetObject("winmgmts:\\" & strComputer & "\root\cimv2")

WScript.Echo  vbCrLf & "Check OS Version..."  &  vbCrLf
' 先判断主机系统信息：OS，SP补丁版本情况
Dim objOSInfo  
Dim intOSver,intOStype,intCurrentSP,version  
Set objOSInfo = objSWbemServices.ExecQuery("Select ServicePackMajorVersion,Version,OSType,Name,OSLanguage,CountryCode FROM Win32_OperatingSystem")
For Each colOSInfo In objOSInfo
   intCurrentSP = colOSInfo.ServicePackMajorVersion 'sp安全补丁版本
   intOSver = colOSInfo.Version     '操作系统版本号
   intOStype = colOSInfo.OSType     '操作系统类型
   strName = colOSInfo.Name
   strOSLanguage = colOSInfo.OSLanguage
   strCountryCode = colOSInfo.CountryCode

   WScript.Echo "OSType: " & intOStype & vbCrLf & _
		        "Version: " & intOSver & vbCrLf & _
				"ServicePackMajorVersion: " & intCurrentSP & vbCrLf & _
				"Name: " & strName & vbCrLf & _
				"OSLanguage: " & strOSLanguage & vbCrLf & _
				"CountryCode: " & strCountryCode & vbCrLf
Next

version = left(intOSver, 3 )
WScript.Echo "OS Version: " & version
If ( version = "6.1"  ) Then
	WScript.Echo "Current Version = 6.1"
	dim bExist
   	bExist = 0

	WScript.Echo  vbCrLf  & "Check HotFixID..."  &  vbCrLf
	Set colSWbemObjectSet = objSWbemServices.ExecQuery("select HotFixID from Win32_QuickFixEngineering")
	For Each objSWbemObject In colSWbemObjectSet
		'WScript.Echo objSWbemObject.HotFixID
		If ( objSWbemObject.HotFixID = arHotFixID(0) ) Then
			WScript.Echo objSWbemObject.HotFixID
			bExist = 1
		End If
	Next	
	If ( 0 = bExist ) Then
		WriteFlgFile flagfile, arHotFixID(0) 
	End If

Else
	WScript.Echo "Current Version <> 6.1"
	'WriteFlgFile flagfile, arHotFixID(0) 
End IF











