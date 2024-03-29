﻿Module Scores
    Friend OrderByDesc As Boolean = True
    Friend Players As New Dictionary(Of String, PlayerEntry)
    Friend GlobalScoreboard As New List(Of GameRecord)
    Structure PlayerEntry
        Friend playerName As String
        Friend lastDiscoveredAt As Integer
        Friend howManyCasesDiscovered As Integer
        Friend howManyGames As Integer
        Friend cumulatedTime As Integer
        Friend GameRecords As List(Of GameRecord)
    End Structure
    Structure GameRecord
        Friend lastDiscoveredAt As Integer
        Friend howManyCasesDiscovered As Integer
        Friend performedBy As String
    End Structure
    Public Sub checkForSaveFile()
        Try
            If Not (System.IO.File.Exists(Settings.saveFilePath)) Then
                Dim file As IO.StreamWriter = System.IO.File.CreateText("save.msw")
                file.Close()
            End If
            Scores.loadScoreboard()
        Catch ex As Exception
            MsgBox(ex)
        End Try
    End Sub
    Public Sub newRecord(playerName As String, lastDiscoveredAt As Integer, howManyCasesDiscovered As Integer)
        Dim record As String = playerName & ":" & lastDiscoveredAt & ":" & howManyCasesDiscovered
        Scores.checkForSaveFile()
        Encryption.Write(saveFilePath, record)
    End Sub
    Public Function getBestGameForEachPlayer()
        Dim bestGames As New Dictionary(Of String, GameRecord)
        GlobalScoreboard = GlobalScoreboard.OrderByDescending(Function(x) x.howManyCasesDiscovered).ThenBy(Function(x) x.lastDiscoveredAt).ThenBy(Function(x) x.performedBy).ToList()

        For Each gr As GameRecord In GlobalScoreboard
            If Not (bestGames.ContainsKey(gr.performedBy)) Then
                bestGames.Add(gr.performedBy, gr)
            End If
        Next
        Return bestGames.Values.ToList
    End Function
    Public Function bestGameFrom(player As String)
        Dim bestGame As List(Of GameRecord) = getBestGameForEachPlayer()
        For Each game As GameRecord In bestGame
            If game.performedBy = player Then
                Return game
            End If
        Next
        Return Nothing
    End Function
    Public Sub LoadEntries()
        Try
            Players.Clear()
            GlobalScoreboard.Clear()
            Leaderboard.lst_leaderboard.Items.Clear()
            loadScoreboard()
        Catch ex As Exception
            MsgBox("Cannot load the scoreboard...")
            Leaderboard.Close()
        End Try

        Dim bestGame As List(Of GameRecord) = getBestGameForEachPlayer()
        bestGame = bestGame.OrderByDescending(Function(x) x.howManyCasesDiscovered).ThenBy(Function(x) x.lastDiscoveredAt).ToList()
        Try
            Dim place As Integer = 1
            For Each playerEntry As GameRecord In bestGame
                Dim rank As String
                If place = 1 Then
                    rank = "1st : "
                ElseIf place = 2 Then
                    rank = "2nd : "
                ElseIf place = 3 Then
                    rank = "3rd : "
                Else
                    rank = place & "th : "
                End If

                Leaderboard.lst_leaderboard.Items.Add(rank & playerEntry.performedBy & " has discovered " & playerEntry.howManyCasesDiscovered & " cases in " & playerEntry.lastDiscoveredAt & " seconds.")
                place += 1
            Next
        Catch e As Exception
            'MsgBox(e.Message)
        End Try
    End Sub
    Public Sub loadScoreboard()
        Leaderboard.lst_leaderboard.Refresh()
        Dim PText As String
        Try
            If (System.IO.File.Exists(Settings.saveFilePath)) Then
                If (System.IO.File.ReadAllText(Settings.saveFilePath).Length = 0) Then
                    Return
                Else
                    PText = Encryption.Decrypt(saveFilePath)
                End If
            Else
                MsgBox("No file founded, cannot load the scoreboard !")
                Leaderboard.Close()
                Return
            End If


            For Each line As String In PText.Split(Environment.NewLine)
                Try
                    Dim data As String() = line.Split(":")
                    data(0) = Replace(data(0), vbLf, "") ' remove the line break

                    Console.WriteLine(data(0) + ":" + CStr(data(1)) + ":" + CStr(data(2)))
                    If data.Length <> 3 Then
                        MsgBox("Invalid entry, data has been modified by the user")
                    End If
                    ' GameRecord template in the file : playerName:lastDiscoveredAt:howManyCasesDiscovered
                    If Not (Players.ContainsKey(data(0))) Then
                        Dim p As New PlayerEntry()
                        p.playerName = data(0)
                        p.lastDiscoveredAt = data(1)
                        p.cumulatedTime = data(1)
                        p.howManyCasesDiscovered = data(2)
                        p.howManyGames = 1

                        p.GameRecords = New List(Of GameRecord)
                        Dim gr As New GameRecord()
                        gr.lastDiscoveredAt = data(1)
                        gr.howManyCasesDiscovered = data(2)
                        gr.performedBy = data(0)
                        p.GameRecords.Add(gr)
                        GlobalScoreboard.Add(gr)

                        Players.Add(p.playerName, p)
                    Else
                        Dim tmp As PlayerEntry = Players.Item(data(0))
                        Players.Remove(data(0))
                        Dim gameRecord As New GameRecord

                        gameRecord.lastDiscoveredAt = data(1)
                        gameRecord.howManyCasesDiscovered = data(2)
                        gameRecord.performedBy = data(0)

                        tmp.cumulatedTime += gameRecord.lastDiscoveredAt
                        tmp.howManyCasesDiscovered += gameRecord.howManyCasesDiscovered
                        tmp.howManyGames += 1
                        tmp.GameRecords.Add(gameRecord)

                        GlobalScoreboard.Add(gameRecord)
                        Players.Add(data(0), tmp)
                    End If
                Catch ex As Exception
                    MsgBox("Invalid data, it has been modified by the user!")
                End Try
            Next
        Catch e As Exception

        End Try
    End Sub
End Module
