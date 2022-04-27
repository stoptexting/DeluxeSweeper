﻿Imports System.IO
Module Encryption
    ' Credits : https://github.com/Ammaar9/FileEncDecVer1 (Pas de notre niveau actuel)
    Dim SecurityLayer As New Security
    Public Sub Encrypt(filePath As String)
        Try
            Dim PText As String = My.Computer.FileSystem.ReadAllText(filePath)
            Dim CText As String = SecurityLayer.doEncrypt(PText)
            My.Computer.FileSystem.WriteAllText(filePath, CText, False)
            MsgBox("File has been encrypted successfully.")
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Sub

    Public Function Decrypt(filePath As String)
        Try
            Dim CText As String = My.Computer.FileSystem.ReadAllText(filePath)
            Dim PText As String = SecurityLayer.doDecrypt(CText)
            'My.Computer.FileSystem.WriteAllText(filePath, PText, False)
            MsgBox("File has been decrypted successfully.")
            Return PText
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Function

    ' Our function
    Public Function Write(filePath As String, message As String)
        Try
            Dim PText As String = My.Computer.FileSystem.ReadAllText(Encryption.Decrypt(filePath))
            Dim CText As String = SecurityLayer.doEncrypt(PText + +Environment.NewLine + message)
            My.Computer.FileSystem.WriteAllText(filePath, CText, False)
            MsgBox("Message written successfully")
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
    End Function
End Module
