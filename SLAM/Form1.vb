Imports NAudio 'Modified Version which does not write "extraSize"
Imports NAudio.Wave
Imports System.IO
Imports System.Text.RegularExpressions
Imports System.Threading
Imports SLAM.XmlSerialization
Imports SLAM.SourceGame
Imports System.Management
Imports System.Net.Http
Imports NReco.VideoConverter
Imports NAudio.Wave.SampleProviders
Imports NAudio.CoreAudioApi



Public Class Form1

    Dim enumerator As New MMDeviceEnumerator()

    Dim Games As New List(Of SourceGame)
    Dim running As Boolean = False
    Dim ClosePending As Boolean = False
    Dim SteamAppsPath As String
    Dim status As Integer = IDLE

    Const IDLE = -1
    Const SEARCHING = -2
    Const WORKING = -3

    Private currentWaveOut As WaveOut
    Private virtualMicDeviceInIndex As Integer = -1

    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load

        PopulateAudioDeviceDropdowns()


        RefreshPlayKey()
        If My.Settings.PlayKey = My.Settings.RelayKey Then
            My.Settings.RelayKey = "="
            My.Settings.Save()
        End If

        If My.Settings.UpdateCheck Then
            CheckForUpdate()
        End If

        Dim cs2 As New SourceGame
        cs2.name = "Counter-Strike 2"
        cs2.id = 730
        cs2.directory = "common\Counter-Strike Global Offensive\" 'CS2 uses the same directory as CS:GO
        cs2.ToCfg = "game\csgo\cfg\" 'CS2 uses the same cfg folder as CS:GO
        cs2.libraryname = "game\csgo\" 'CS2 uses a different library folder
        cs2.exename = "cs2"
        cs2.samplerate = 22050 'CS2 uses a different sample rate than CS:GO
        cs2.blacklist.AddRange({"attack", "attack2", "autobuy", "back", "buy", "buyammo1", "buyammo2", "buymenu", "callvote", "cancelselect", "cheer", "compliment", "coverme", "drop", "duck", "enemydown", "enemyspot", "fallback", "followme", "forward", "getout", "go", "holdpos", "inposition", "invnext", "invprev", "jump", "lastinv", "messagemode", "messagemode2", "moveleft", "moveright", "mute", "negative", "quit", "radio1", "radio2", "radio3", "rebuy", "regroup", "reload", "reporting_in", "reporting_in_team_only", "roger", "sectorclear", "showscores", "slot1", "slot10", "slot2", "slot3", "slot4", "slot5", "slot6", "slot7", "slot8", "slot9", "speed", "sticktog", "takepoint", "takingfire", "teammenu", "thanks", "toggleconsole", "use", "voicerecord"})
        cs2.VoiceFadeOut = False 'CS2 does not fade out voice chat
        Games.Add(cs2)

        For Each Game In Games
            GameSelector.Items.Add(Game.name)
        Next

        If GameSelector.Items.Contains(My.Settings.LastGame) Then
            GameSelector.Text = GameSelector.Items(GameSelector.Items.IndexOf(My.Settings.LastGame)).ToString
        Else
            GameSelector.Text = GameSelector.Items(0).ToString
        End If

        ReloadTracks(GetCurrentGame)
        RefreshTrackList()

        If My.Settings.StartEnabled Then
            StartPoll()
        End If

        If My.Settings.StartMinimized Then
            WindowState = FormWindowState.Minimized
        End If
    End Sub

    Private Sub WaveCreator(File As String, outputFile As String, Game As SourceGame)
        Dim reader As New MediaFoundationReader(File)


        Dim outFormat = New WaveFormat(Game.samplerate, Game.bits, Game.channels)

        Dim resampler = New MediaFoundationResampler(reader, outFormat)

        resampler.ResamplerQuality = 60

        WaveFileWriter.CreateWaveFile(outputFile, resampler)

        resampler.Dispose()
    End Sub

    Private Sub FFMPEG_WaveCreator(File As String, outputFile As String, Game As SourceGame)
        Dim convert As New FFMpegConverter()
        convert.ExtractFFmpeg()

        Dim command As String = String.Format("-i ""{0}"" -n -f wav -flags bitexact -map_metadata -1 -vn -acodec pcm_s16le -ar {1} -ac {2} ""{3}""", Path.GetFullPath(File), Game.samplerate, Game.channels, Path.GetFullPath(outputFile))
        convert.Invoke(command)
    End Sub

    Private Sub FFMPEG_ConvertAndTrim(inpath As String, outpath As String, samplerate As Integer, channels As Integer, starttrim As Double, length As Double, volume As Double)
        Dim convert As New FFMpegConverter()
        convert.ExtractFFmpeg()

        Dim trimstring As String
        If length > 0 Then
            trimstring = String.Format("-ss {0} -t {1} ", starttrim.ToString("F5", Globalization.CultureInfo.InvariantCulture), length.ToString("F5", Globalization.CultureInfo.InvariantCulture))
        End If

        Dim command As String = String.Format("-i ""{0}"" -n -f wav -flags bitexact -map_metadata -1 -vn -acodec pcm_s16le -ar {1} -ac {2} {3}-af ""volume={4}"" ""{5}""", Path.GetFullPath(inpath), samplerate, channels, trimstring, volume.ToString("F5", Globalization.CultureInfo.InvariantCulture), Path.GetFullPath(outpath))
        convert.Invoke(command)
    End Sub

    Private Sub GameSelector_SelectedIndexChanged(sender As Object, e As EventArgs) Handles GameSelector.SelectedIndexChanged
        ReloadTracks(GetCurrentGame)
        RefreshTrackList()
        My.Settings.LastGame = GameSelector.Text
        My.Settings.Save()
    End Sub

    Private Sub ImportButton_Click(sender As Object, e As EventArgs) Handles ImportButton.Click
        If (My.Settings.UseFFMPEG = True And File.Exists("NReco.VideoConverter.dll")) Or (My.Settings.UseFFMPEG = False And File.Exists("NAudio.dll")) Then
            DisableInterface()
            If ImportDialog.ShowDialog() = DialogResult.OK Then
                ProgressBar1.Maximum = ImportDialog.FileNames.Count
                Dim WorkerPassthrough() As Object = {GetCurrentGame(), ImportDialog.FileNames, False}
                WavWorker.RunWorkerAsync(WorkerPassthrough)
            Else
                EnableInterface()
            End If

        Else
            MessageBox.Show("You are missing NAudio.dll or NReco.VideoConverter.dll! Cannot import without it!", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub YTButton_Click(sender As Object, e As EventArgs) Handles YTButton.Click
        If File.Exists("NAudio.dll") AndAlso File.Exists("Newtonsoft.Json.dll") AndAlso File.Exists("NReco.VideoConverter.dll") AndAlso File.Exists("YoutubeExtractor.dll") Then
            DisableInterface()
            Dim YTImporter As New YTImport
            If YTImporter.ShowDialog() = DialogResult.OK Then
                ProgressBar1.Maximum = 1
                Dim WorkerPassthrough() As Object = {GetCurrentGame(), New String() {YTImporter.file}, True}
                WavWorker.RunWorkerAsync(WorkerPassthrough)
            Else
                EnableInterface()
            End If

        Else
            MessageBox.Show("You are missing either NAudio.dll, Newtonsoft.Json.dll, NReco.VideoConverter.dll, or YoutubeExtractor.dll! Cannot import from YouTube without them!", "Missing File(s)", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub WavWorker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles WavWorker.DoWork
        Dim Game As SourceGame = e.Argument(0)
        Dim Files() As String = e.Argument(1)
        Dim DeleteSource As Boolean = e.Argument(2)
        Dim FailedFiles As New List(Of String)

        For Each File In Files

            Try
                Dim OutFile As String = Path.Combine(Game.libraryname, Path.GetFileNameWithoutExtension(File) & ".wav")

                If My.Settings.UseFFMPEG Then
                    FFMPEG_WaveCreator(File, OutFile, Game)
                Else
                    WaveCreator(File, OutFile, Game)
                End If


                If DeleteSource Then
                    IO.File.Delete(File)
                End If
            Catch ex As Exception
                LogError(ex)
                FailedFiles.Add(File)
            End Try
            WavWorker.ReportProgress(0)
        Next

        e.Result = FailedFiles

    End Sub

    Private Sub WavWorker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles WavWorker.ProgressChanged
        ProgressBar1.PerformStep()
        ReloadTracks(GetCurrentGame)
        RefreshTrackList()
    End Sub

    Private Sub WavWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles WavWorker.RunWorkerCompleted
        ProgressBar1.Value = 0
        Dim MsgBoxText As String = "Conversion complete!"
        Dim FailedFiles As New List(Of String)

        For Each FilePath In e.Result
            FailedFiles.Add(Path.GetFileName(FilePath))
        Next

        If FailedFiles.Count > 0 Then
            MsgBoxText = MsgBoxText & " However, the following files failed to convert: " & String.Join(", ", FailedFiles)
        End If

        ReloadTracks(GetCurrentGame)
        RefreshTrackList()
        MsgBox(MsgBoxText)
        EnableInterface()
    End Sub

    Private Function GetCurrentGame() As SourceGame
        For Each Game In Games
            If Game.name = GameSelector.SelectedItem.ToString Then
                Return Game
            End If
        Next
        Return Nothing 'Null if nothing found
    End Function

    Private Sub ReloadTracks(Game As SourceGame)
        If IO.Directory.Exists(Game.libraryname) Then

            Game.tracks.Clear()
            For Each File In System.IO.Directory.GetFiles(Game.libraryname)

                If Game.FileExtension = Path.GetExtension(File) Then
                    Dim track As New track
                    track.name = Path.GetFileNameWithoutExtension(File)
                    Game.tracks.Add(track)
                End If

            Next

            CreateTags(Game)
            LoadTrackKeys(Game)
            SaveTrackKeys(Game) 'To prune hotkeys from non-existing tracks

        Else
            System.IO.Directory.CreateDirectory(Game.libraryname)
        End If
    End Sub

    Private Sub RefreshTrackList()
        TrackList.Items.Clear()

        Dim Game As SourceGame = GetCurrentGame()

        For Each Track In Game.tracks

            Dim trimmed As String = ""
            If Track.endpos > 0 Then
                trimmed = "Yes"
            End If

            TrackList.Items.Add(New ListViewItem({"False", Track.name, Track.hotkey, Track.volume & "%", trimmed, """" & String.Join(""", """, Track.tags) & """"}))
        Next


        TrackList.AutoResizeColumn(0, ColumnHeaderAutoResizeStyle.HeaderSize)
        TrackList.AutoResizeColumn(1, ColumnHeaderAutoResizeStyle.ColumnContent)
        TrackList.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.HeaderSize)
        TrackList.AutoResizeColumn(3, ColumnHeaderAutoResizeStyle.HeaderSize)
        TrackList.AutoResizeColumn(4, ColumnHeaderAutoResizeStyle.HeaderSize)
        TrackList.AutoResizeColumn(5, ColumnHeaderAutoResizeStyle.ColumnContent)
    End Sub

    Private Sub StartButton_Click(sender As Object, e As EventArgs) Handles StartButton.Click
        If running Then
            StopPoll()
        Else
            StartPoll()
            If Not My.Settings.NoHint Then
                If MessageBox.Show("Don't forget to type ""exec slam"" in console! Click ""Cancel"" if you don't ever want to see this message again.", "SLAM", MessageBoxButtons.OKCancel) = Windows.Forms.DialogResult.Cancel Then
                    My.Settings.NoHint = True
                    My.Settings.Save()
                End If
            End If
        End If
    End Sub

    Private Sub StartPoll()
        running = True
        StartButton.Text = "Stop"
        SystemTrayMenu_StartStop.Text = "Stop"
        DisableInterface()
        StartButton.Enabled = True
        TrackList.Enabled = True
        SetVolumeToolStripMenuItem.Enabled = True
        If PollRelayWorker.IsBusy <> True Then
            PollRelayWorker.RunWorkerAsync(GetCurrentGame)
        End If
    End Sub

    Private Sub StopPoll()
        running = False
        StartButton.Text = "Start"
        SystemTrayMenu_StartStop.Text = "Start"
        EnableInterface()
        PollRelayWorker.CancelAsync()
    End Sub

    Private Sub CreateCfgFiles(Game As SourceGame, SteamappsPath As String)
        Dim GameDir As String = Path.Combine(SteamappsPath, Game.directory)
        Dim GameCfgFolder As String = Path.Combine(GameDir, Game.ToCfg)
        Dim virtualMicOutName As String = "CABLE Output (VB-Audio Virtual Cable)"
        Dim regularMicName As String = "Lapel Mic (Wireless microphone)"

        If Not String.IsNullOrEmpty(My.Settings.VirtualMicOutDeviceName) Then
            ' Extract just the device name part (after the colon and space)
            Dim parts = My.Settings.VirtualMicOutDeviceName.Split(New Char() {":"c}, 2)
            If parts.Length = 2 Then virtualMicOutName = parts(1).Trim()
        End If
        If Not String.IsNullOrEmpty(My.Settings.RegularMicDeviceName) Then
            Dim parts = My.Settings.RegularMicDeviceName.Split(New Char() {":"c}, 2)
            If parts.Length = 2 Then regularMicName = parts(1).Trim()
        End If

        If Not IO.Directory.Exists(GameCfgFolder) Then
            Throw New System.Exception("Steamapps folder is incorrect. Disable ""override folder detection"", or select a correct folder.")
        End If

        ' Collect all lines for slam.cfg
        Dim slamLines As New List(Of String)
        Debug.Print($"voice_device_override {virtualMicOutName};")
        Debug.Print($"voice_device_override {regularMicName};")
        slamLines.Add("alias slam_play slam_updatecfg;slam_play_on;")
        slamLines.Add($"alias slam_play_on ""alias slam_play slam_play_off; slam_updatecfg; voice_loopback 1; voice_device_override {virtualMicOutName}; +voicerecord""")
        slamLines.Add($"alias slam_play_off ""alias slam_play slam_play_on; -voicerecord; voice_loopback 0; voice_device_override {regularMicName};""")
        slamLines.Add("alias slam_updatecfg ""host_writeconfig""")
        If My.Settings.HoldToPlay Then
            slamLines.Add(String.Format("alias +slam_hold_play slam_play_on"))
            slamLines.Add(String.Format("alias -slam_hold_play slam_play_off"))
            slamLines.Add(String.Format("bind {0} +slam_hold_play", My.Settings.PlayKey))
        Else
            slamLines.Add(String.Format("bind {0} slam_play", My.Settings.PlayKey))
        End If
        slamLines.Add("alias slam_curtrack ""exec slam_curtrack.cfg""")
        slamLines.Add("alias slam_saycurtrack ""exec slam_saycurtrack.cfg""")
        slamLines.Add("alias slam_sayteamcurtrack ""exec slam_sayteamcurtrack.cfg""")

        For Each Track In Game.tracks
            Dim index As String = Game.tracks.IndexOf(Track)
            slamLines.Add(String.Format("alias {0} ""bind {1} {0};""", index + 1, My.Settings.RelayKey))
            For Each TrackTag In Track.tags
                slamLines.Add(String.Format("alias {0} ""bind {1} {2};""", TrackTag, My.Settings.RelayKey, index + 1))
            Next
            If Not String.IsNullOrEmpty(Track.hotkey) Then
                slamLines.Add(String.Format("bind {0} ""bind {1} {2}; slam_updatecfg; slam_play_on; """, Track.hotkey, My.Settings.RelayKey, index + 1))
            End If
        Next

        Dim CfgData As String = "voice_modenable 1; con_enable 1"
        If Game.VoiceFadeOut Then
            CfgData = CfgData + "; voice_fadeouttime 0.0"
        End If
        slamLines.Add(CfgData)

        ' Write lines in chunks of 200
        Dim fileIndex As Integer = 1
        Dim totalLines As Integer = slamLines.Count
        Dim writtenLines As Integer = 0
        Dim nextCfgName As String = ""

        While writtenLines < totalLines
            Dim linesToWrite = Math.Min(200, totalLines - writtenLines)
            Dim fileName As String = If(fileIndex = 1, "slam.cfg", $"slam{fileIndex}.cfg")
            Dim filePath As String = Path.Combine(GameCfgFolder, fileName)
            Using writer As New StreamWriter(filePath, False)
                For i = 0 To linesToWrite - 1
                    writer.WriteLine(slamLines(writtenLines + i))
                Next
                writtenLines += linesToWrite
                ' If there are more lines, add exec for next file
                If writtenLines < totalLines Then
                    nextCfgName = $"slam{fileIndex + 1}"
                End If
            End Using
            fileIndex += 1
        End While

        ' slam_tracklist.cfg (unchanged)
        Using slam_tracklist_cfg As StreamWriter = New StreamWriter(GameCfgFolder & "slam_tracklist.cfg")
            slam_tracklist_cfg.WriteLine("echo ""You can select tracks either by typing a tag, or their track number.""")
            slam_tracklist_cfg.WriteLine("echo ""--------------------Tracks--------------------""")
            For Each Track In Game.tracks
                Dim index As String = Game.tracks.IndexOf(Track)
                If My.Settings.WriteTags Then
                    slam_tracklist_cfg.WriteLine("echo ""{0}. {1} [{2}]""", index + 1, Track.name, "'" & String.Join("', '", Track.tags) & "'")
                Else
                    slam_tracklist_cfg.WriteLine("echo ""{0}. {1}""", index + 1, Track.name)
                End If
            Next
            slam_tracklist_cfg.WriteLine("echo ""----------------------------------------------""")
        End Using
    End Sub

    Private Function LoadTrack(ByVal Game As SourceGame, ByVal index As Integer) As Boolean
        Dim Track As track
        If Game.tracks.Count > index Then
            Track = Game.tracks(index)

            Try
                ' Stop any currently playing audio
                If currentWaveOut IsNot Nothing Then
                    currentWaveOut.Stop()
                    currentWaveOut.Dispose()
                    currentWaveOut = Nothing
                End If

                Dim trackfile As String = Game.libraryname & Track.name & Game.FileExtension
                If File.Exists(trackfile) Then

                    ' Find virtual microphone device if not already found
                    If virtualMicDeviceInIndex = -1 Then
                        FindVirtualMicDevice()
                    End If

                    If virtualMicDeviceInIndex = -1 Then
                        MessageBox.Show("Virtual microphone device not found! Please install a virtual audio cable.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                        Return False
                    End If

                    Debug.WriteLine($"Using device index: {virtualMicDeviceInIndex}")

                    ' Load and prepare audio
                    Dim audioReader As WaveStream

                    Debug.WriteLine($"Loading track: {trackfile}")

                    If Track.volume = 100 And Track.startpos <= 0 And Track.endpos <= 0 Then
                        ' Simple case - just play the file as-is
                        audioReader = New AudioFileReader(trackfile)
                        Debug.WriteLine($"Simple playback - Format: {audioReader.WaveFormat}")
                    Else
                        ' Complex case - apply volume, trimming, etc.
                        Dim WaveFloat As New WaveChannel32(New WaveFileReader(trackfile))

                        If Not Track.volume = 100 Then
                            WaveFloat.Volume = (Track.volume / 100) ^ 6
                            Debug.WriteLine($"Applied volume: {Track.volume}%")
                        End If

                        If Not Track.startpos = Track.endpos And Track.endpos > 0 Then
                            Dim bytes((Track.endpos - Track.startpos) * 4) As Byte
                            WaveFloat.Position = Track.startpos * 4
                            WaveFloat.Read(bytes, 0, (Track.endpos - Track.startpos) * 4)
                            WaveFloat = New WaveChannel32(New RawSourceWaveStream(New MemoryStream(bytes), WaveFloat.WaveFormat))
                            Debug.WriteLine($"Applied trimming: {Track.startpos} to {Track.endpos}")
                        End If

                        WaveFloat.PadWithZeroes = False
                        audioReader = WaveFloat
                        Debug.WriteLine($"Complex playback - Format: {audioReader.WaveFormat}")
                    End If

                    ' Play to virtual microphone device
                    currentWaveOut = New WaveOut()
                    currentWaveOut.DeviceNumber = virtualMicDeviceInIndex

                    Try
                        currentWaveOut.Init(audioReader)
                        currentWaveOut.Play()
                        Debug.WriteLine("Audio playback started successfully")

                        ' Add event handler to know when playback stops
                        AddHandler currentWaveOut.PlaybackStopped, New EventHandler(Of NAudio.Wave.StoppedEventArgs)(AddressOf OnPlaybackStopped)

                    Catch ex As Exception
                        Debug.WriteLine($"Playback error: {ex.Message}")
                        LogError(ex)
                        Return False
                    End Try

                    ' Update track info displays (keep existing functionality)
                    Dim GameCfgFolder As String = Path.Combine(SteamAppsPath, Game.directory, Game.ToCfg)
                    Using slam_curtrack As StreamWriter = New StreamWriter(GameCfgFolder & "slam_curtrack.cfg")
                        slam_curtrack.WriteLine("echo ""[SLAM] Track name: {0}""", Track.name)
                    End Using
                    Using slam_saycurtrack As StreamWriter = New StreamWriter(GameCfgFolder & "slam_saycurtrack.cfg")
                        slam_saycurtrack.WriteLine("say ""[SLAM] Track name: {0}""", Track.name)
                    End Using
                    Using slam_sayteamcurtrack As StreamWriter = New StreamWriter(GameCfgFolder & "slam_sayteamcurtrack.cfg")
                        slam_sayteamcurtrack.WriteLine("say_team ""[SLAM] Track name: {0}""", Track.name)
                    End Using

                End If

            Catch ex As Exception
                LogError(ex)
                Return False
            End Try

        Else
            Return False
        End If

        Return True
    End Function

    Private Function recog(ByVal str As String, ByVal key As String) As String
        'Debug.WriteLine($"Searching for key: '{key}' in config")
        'Debug.WriteLine($"Config content length: {str.Length}")

        Dim newPattern As String = String.Format("""{0}""\s+""(.*?)""", key)
        'Debug.WriteLine($"Trying new pattern: {newPattern}")
        Dim newMatch As Match = Regex.Match(str, newPattern, RegexOptions.IgnoreCase)
        If newMatch.Success Then
            'Debug.WriteLine($"New format match found: {newMatch.Groups(1).ToString()}")
            Return newMatch.Groups(1).ToString()
        End If

        'Debug.WriteLine("No match found for either pattern")
        Return String.Empty
    End Function

    Private Sub PollRelayWorker_DoWork(sender As Object, e As System.ComponentModel.DoWorkEventArgs) Handles PollRelayWorker.DoWork
        PollRelayWorker.ReportProgress(SEARCHING) 'Report that SLAM is searching.

        Dim Game As SourceGame = e.Argument
        Dim GameDir As String = Game.directory & Game.exename & ".exe"

        SteamAppsPath = vbNullString
        Dim UserDataPath As String = vbNullString

        Try
            If Not My.Settings.OverrideFolders Then

                Do While Not PollRelayWorker.CancellationPending

                    Dim GameProcess As String = GetFilepath(Game.exename)
                    If Not String.IsNullOrEmpty(GameProcess) AndAlso GameProcess.EndsWith(GameDir) Then
                        SteamAppsPath = GameProcess.Remove(GameProcess.Length - GameDir.Length)
                    End If

                    Dim SteamProcess As String = GetFilepath("Steam")
                    If Not String.IsNullOrEmpty(SteamProcess) Then
                        UserDataPath = SteamProcess.Remove(SteamProcess.Length - "Steam.exe".Length) & "userdata\"
                    End If

                    If IO.Directory.Exists(SteamAppsPath) Then
                        If Not Game.id = 0 Then

                            If IO.Directory.Exists(UserDataPath) Then
                                Exit Do
                            End If

                        Else
                            Exit Do
                        End If
                    End If

                    Thread.Sleep(Game.PollInterval)
                Loop
            Else
                SteamAppsPath = My.Settings.steamapps
                If IO.Directory.Exists(My.Settings.userdata) Then
                    UserDataPath = My.Settings.userdata
                Else
                    Throw New System.Exception("Userdata folder does not exist. Disable ""override folder detection"", or select a correct folder.")
                End If
            End If

            If Not String.IsNullOrEmpty(SteamAppsPath) Then
                CreateCfgFiles(Game, SteamAppsPath)
            End If

        Catch ex As Exception
            LogError(ex)
            e.Result = ex
            Return
        End Try


        PollRelayWorker.ReportProgress(WORKING) 'Report that SLAM is working.

        Do While Not PollRelayWorker.CancellationPending
            Try
                Dim GameFolder As String = Path.Combine(SteamAppsPath, Game.directory)
                Dim GameCfg As String


                GameCfg = UserDataCFG(Game, UserDataPath)

                'Debug.WriteLine("Polling for game config: " & GameCfg)

                If File.Exists(GameCfg) Then
                    Debug.WriteLine("Game Config Exists")

                    Dim RelayCfg As String
                    Using reader As StreamReader = New StreamReader(GameCfg)
                        RelayCfg = reader.ReadToEnd
                    End Using

                    Dim command As String = recog(RelayCfg, My.Settings.RelayKey)

                    Debug.WriteLine("Command: " & command)

                    If Not String.IsNullOrEmpty(command) Then
                        ' Play audio to virtual microphone instead of copying file
                        If IsNumeric(command) Then
                            If LoadTrack(Game, Convert.ToInt32(command) - 1) Then
                                PollRelayWorker.ReportProgress(Convert.ToInt32(command) - 1)
                            End If
                        End If
                        File.Delete(GameCfg)
                    End If
                End If

                Thread.Sleep(Game.PollInterval)

            Catch ex As Exception
                If Not ex.HResult = -2147024864 Then
                    LogError(ex)
                    e.Result = ex
                    Return
                End If
            End Try
        Loop

        ' Clean up audio when stopping
        If currentWaveOut IsNot Nothing Then
            currentWaveOut.Stop()
            currentWaveOut.Dispose()
            currentWaveOut = Nothing
        End If

        If Not String.IsNullOrEmpty(SteamAppsPath) Then
            DeleteCFGs(Game, SteamAppsPath)
        End If

    End Sub

    Public Function UserDataCFG(Game As SourceGame, UserdataPath As String) As String
        If IO.Directory.Exists(UserdataPath) Then
            For Each userdir As String In System.IO.Directory.GetDirectories(UserdataPath)
                Dim CFGPath As String = Path.Combine(userdir, Game.id.ToString) & "\local\cfg\cs2_user_keys_0_slot0.vcfg"
                If File.Exists(CFGPath) Then
                    Return CFGPath
                End If
            Next
        End If
        Return vbNullString
    End Function

    Private Function GetFilepath(ProcessName As String) As String

        Dim wmiQueryString As String = "Select * from Win32_Process Where Name = """ & ProcessName & ".exe"""

        Using searcher = New ManagementObjectSearcher(wmiQueryString)
            Using results = searcher.Get()

                Dim Process As ManagementObject = results.Cast(Of ManagementObject)().FirstOrDefault()
                If Process IsNot Nothing Then
                    Dim exePath = Process("ExecutablePath")
                    ' Check Process("ExecutablePath") for null before calling ToString.
                    ' Fixes error that occurs if you start steam / csgo while SLAM is searching.
                    Dim procPath = If(exePath IsNot Nothing, exePath.ToString(), vbNullString)
                    If Not String.IsNullOrWhiteSpace(procPath) Then
                        Return Process("ExecutablePath").ToString
                    End If
                End If

            End Using
        End Using

        Return Nothing
    End Function

    Private Sub PollRelayWorker_ProgressChanged(sender As Object, e As System.ComponentModel.ProgressChangedEventArgs) Handles PollRelayWorker.ProgressChanged
        Select Case e.ProgressPercentage
            Case SEARCHING
                status = SEARCHING
                StatusLabel.Text = "Status: Searching..."
            Case WORKING
                status = WORKING
                StatusLabel.Text = "Status: Working."
            Case Else
                DisplayLoaded(e.ProgressPercentage)
        End Select

    End Sub

    Private Sub PollRelayWorker_RunWorkerCompleted(sender As Object, e As System.ComponentModel.RunWorkerCompletedEventArgs) Handles PollRelayWorker.RunWorkerCompleted
        If running Then
            StopPoll()
        End If

        status = IDLE
        StatusLabel.Text = "Status: Idle."
        RefreshTrackList()

        If Not IsNothing(e.Result) Then 'Result is always an exception
            MessageBox.Show(e.Result.Message & " See errorlog.txt for more info.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

        If ClosePending Then
            Me.Close()
        End If
    End Sub

    Private Sub CreateTags(ByVal Game As SourceGame)
        Dim NameWords As New Dictionary(Of String, Integer)

        Dim index As Integer
        For Each Track In Game.tracks
            Dim Words As List(Of String) = Track.name.Split({" "c, "."c, "-"c, "_"c}).ToList

            For Each Word In Words

                If Not IsNumeric(Word) And Not Game.blacklist.Contains(Word.ToLower) And Word.Length < 32 Then
                    If NameWords.ContainsKey(Word) Then
                        NameWords.Remove(Word)
                    Else
                        NameWords.Add(Word, index)
                    End If
                End If

            Next
            index += 1
        Next

        For Each Tag As KeyValuePair(Of String, Integer) In NameWords
            Game.tracks(Tag.Value).tags.Add(Tag.Key)
        Next
    End Sub

    Private Sub EnableInterface()
        For Each Control In Me.Controls
            Control.Enabled = True
        Next
    End Sub

    Private Sub DisableInterface()
        For Each Control In Me.Controls
            Control.Enabled = False
        Next
    End Sub

    Private Sub DisplayLoaded(ByVal track As Integer)
        For i As Integer = 0 To TrackList.Items.Count - 1
            TrackList.Items(i).SubItems(0).Text = "False"
        Next
        TrackList.Items(track).SubItems(0).Text = "True"
    End Sub

    Private Sub LoadTrackKeys(ByVal Game As SourceGame)
        Dim SettingsList As New List(Of track)
        Dim SettingsFile As String = Path.Combine(Game.libraryname, "TrackSettings.xml")

        If File.Exists(SettingsFile) Then
            Dim XmlFile As String
            Using reader As StreamReader = New StreamReader(SettingsFile)
                XmlFile = reader.ReadToEnd
            End Using
            SettingsList = Deserialize(Of List(Of track))(XmlFile)
        End If


        For Each Track In Game.tracks
            For Each SetTrack In SettingsList
                If Track.name = SetTrack.name Then
                    'Please tell me that there is a better way to do the following...
                    Track.hotkey = SetTrack.hotkey
                    Track.volume = SetTrack.volume
                    Track.startpos = SetTrack.startpos
                    Track.endpos = SetTrack.endpos
                End If
            Next
        Next

    End Sub

    Private Sub SaveTrackKeys(ByVal Game As SourceGame)
        Dim SettingsList As New List(Of track)
        Dim SettingsFile As String = Path.Combine(Game.libraryname, "TrackSettings.xml")

        For Each Track In Game.tracks
            If Not String.IsNullOrEmpty(Track.hotkey) Or Not Track.volume = 100 Or Track.endpos > 0 Then

                SettingsList.Add(Track)

            End If
        Next

        If SettingsList.Count > 0 Then
            Using writer As StreamWriter = New StreamWriter(SettingsFile)
                writer.Write(Serialize(SettingsList))
            End Using
        Else
            If File.Exists(SettingsFile) Then
                File.Delete(SettingsFile)
            End If
        End If

    End Sub

    Private Sub TrackList_MouseClick(sender As Object, e As MouseEventArgs) Handles TrackList.MouseClick
        If e.Button = MouseButtons.Right Then
            If TrackList.FocusedItem.Bounds.Contains(e.Location) Then

                For Each Control In TrackContextMenu.Items 'everything invisible
                    Control.visible = False
                Next

                SetVolumeToolStripMenuItem.Visible = True 'always visible
                ContextRefresh.Visible = True

                If TrackList.SelectedItems.Count > 1 Then
                    If Not running Then 'visible when multiple selected AND is not running
                        ContextDelete.Visible = True
                    End If

                Else
                    If running Then
                        TrimToolStripMenuItem.Visible = True 'visible when only one selected AND is running
                        If status = WORKING Then
                            LoadToolStripMenuItem.Visible = True
                        End If
                    Else
                        For Each Control In TrackContextMenu.Items 'visible when only one selected AND is not running (all)
                            Control.visible = True
                        Next
                        LoadToolStripMenuItem.Visible = False
                    End If

                End If
                'Maybe I should have used a case... Maybe...

            End If



            TrackContextMenu.Show(Cursor.Position)
        End If
    End Sub

    Private Sub TrackList_MouseDoubleClick(sender As Object, e As MouseEventArgs) Handles TrackList.MouseDoubleClick
        If TrackList.FocusedItem.Bounds.Contains(e.Location) AndAlso status = WORKING Then
            LoadTrack(GetCurrentGame, TrackList.SelectedItems(0).Index)
            DisplayLoaded(TrackList.SelectedItems(0).Index)
        End If
    End Sub

    Private Sub ContextRefresh_Click(sender As Object, e As EventArgs) Handles ContextRefresh.Click
        ReloadTracks(GetCurrentGame)
        RefreshTrackList()
    End Sub

    Private Sub ContextDelete_Click(sender As Object, e As EventArgs) Handles ContextDelete.Click
        Dim game As SourceGame = GetCurrentGame()

        Dim SelectedNames As New List(Of String)
        For Each item In TrackList.SelectedItems
            SelectedNames.Add(item.SubItems(1).Text)
        Next

        If MessageBox.Show(String.Format("Are you sure you want to delete {0}?", String.Join(", ", SelectedNames)), "Delete Track?", MessageBoxButtons.YesNo) = Windows.Forms.DialogResult.Yes Then

            For Each item In SelectedNames
                Dim FilePath As String = Path.Combine(game.libraryname, item & game.FileExtension)

                If File.Exists(FilePath) Then
                    Try
                        File.Delete(FilePath)
                    Catch ex As Exception
                        LogError(ex)
                        MsgBox(String.Format("Failed to delete {0}.", FilePath))
                    End Try
                End If
            Next

        End If

        ReloadTracks(GetCurrentGame)
        RefreshTrackList()
    End Sub

    Private Sub ContextHotKey_Click(sender As Object, e As EventArgs) Handles ContextHotKey.Click
        Dim SelectKeyDialog As New SelectKey
        Dim SelectedIndex = TrackList.SelectedItems(0).Index
        If SelectKeyDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            Dim Game = GetCurrentGame()



            Dim KeyIsFree As Boolean = True
            For Each track In Game.tracks
                If track.hotkey = SelectKeyDialog.ChosenKey Then 'Checking to see if any other track is already using this key
                    KeyIsFree = False
                End If
            Next

            If KeyIsFree Then
                Game.tracks(SelectedIndex).hotkey = SelectKeyDialog.ChosenKey
                SaveTrackKeys(GetCurrentGame)
                ReloadTracks(GetCurrentGame)
                RefreshTrackList()
            Else
                MessageBox.Show(String.Format("""{0}"" has already been assigned!", SelectKeyDialog.ChosenKey), "Invalid Key")
            End If


        End If
    End Sub

    Private Sub RemoveHotkeyToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RemoveHotkeyToolStripMenuItem.Click
        For Each SelectedIndex In TrackList.SelectedItems
            Dim Game = GetCurrentGame()
            Game.tracks(SelectedIndex.index).hotkey = vbNullString
            SaveTrackKeys(GetCurrentGame)
            ReloadTracks(GetCurrentGame)

        Next
        RefreshTrackList()
    End Sub

    Private Sub GoToToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GoToToolStripMenuItem.Click
        Dim Games As SourceGame = GetCurrentGame()
        Dim FilePath As String = Path.Combine(Games.libraryname, Games.tracks(TrackList.SelectedItems(0).Index).name & Games.FileExtension)


        Dim Args As String = String.Format("/Select, ""{0}""", FilePath)
        Dim pfi As New ProcessStartInfo("Explorer.exe", Args)

        System.Diagnostics.Process.Start(pfi)
    End Sub

    Private Sub SetVolumeToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SetVolumeToolStripMenuItem.Click
        Dim SetVolumeDialog As New SetVolume

        If SetVolumeDialog.ShowDialog = Windows.Forms.DialogResult.OK Then

            For Each index In TrackList.SelectedIndices
                GetCurrentGame.tracks(index).volume = SetVolumeDialog.Volume
            Next
            SaveTrackKeys(GetCurrentGame)
            ReloadTracks(GetCurrentGame)
            RefreshTrackList()

        End If

    End Sub

    Private Sub TrimToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TrimToolStripMenuItem.Click
        If File.Exists("NAudio.dll") Then

            Dim Game As SourceGame = GetCurrentGame()
            Dim TrimDialog As New TrimForm

            TrimDialog.WavFile = Path.Combine(Game.libraryname, Game.tracks(TrackList.SelectedIndices(0)).name & Game.FileExtension)
            TrimDialog.startpos = Game.tracks(TrackList.SelectedIndices(0)).startpos
            TrimDialog.endpos = Game.tracks(TrackList.SelectedIndices(0)).endpos


            If TrimDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
                Game.tracks(TrackList.SelectedIndices(0)).startpos = TrimDialog.startpos
                Game.tracks(TrackList.SelectedIndices(0)).endpos = TrimDialog.endpos
                SaveTrackKeys(GetCurrentGame)
                ReloadTracks(GetCurrentGame)
                RefreshTrackList()
            End If

        Else
            MessageBox.Show("You are missing NAudio.dll! Cannot trim without it!", "Missing File", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

    Private Sub RenameToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles RenameToolStripMenuItem.Click
        Dim Game As SourceGame = GetCurrentGame()
        Dim RenameDialog As New RenameForm
        Dim SelectedTrack As SourceGame.track = GetCurrentGame.tracks(TrackList.SelectedIndices(0))

        RenameDialog.filename = SelectedTrack.name

        If RenameDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            Try

                FileSystem.Rename(Game.libraryname & SelectedTrack.name & Game.FileExtension, Game.libraryname & RenameDialog.filename & Game.FileExtension)
                GetCurrentGame.tracks(TrackList.SelectedIndices(0)).name = RenameDialog.filename

                SaveTrackKeys(GetCurrentGame)
                ReloadTracks(GetCurrentGame)
                RefreshTrackList()

            Catch ex As Exception
                Select Case ex.HResult
                    Case -2147024809
                        MessageBox.Show("""" & RenameDialog.filename & """ contains invalid characters.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

                    Case -2146232800
                        MessageBox.Show("A track with that name already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)

                    Case Else
                        MessageBox.Show(ex.Message & " See errorlog.txt for more info.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                End Select

            End Try
        End If
    End Sub

    Private Async Sub CheckForUpdate()
        Dim UpdateText As String

        Dim NeatVersion As String = My.Application.Info.Version.ToString.Remove(My.Application.Info.Version.ToString.LastIndexOf("."))

        Try

            Using client As New HttpClient
                Dim UpdateTextTask As Task(Of String) = client.GetStringAsync("http://slam.flankers.net/updates.php?version=" & NeatVersion)
                UpdateText = Await UpdateTextTask
            End Using

        Catch ex As Exception
            Return
        End Try

        Dim NewVersion As New Version("0.0.0.0") 'generic
        Dim UpdateURL As String = UpdateText.Split()(1)
        If Version.TryParse(UpdateText.Split()(0), NewVersion) Then
            If My.Application.Info.Version.CompareTo(NewVersion) < 0 Then
                If MessageBox.Show(String.Format("An update ({0}) is available! Click ""OK"" to be taken to the download page.", NewVersion.ToString), "SLAM Update", MessageBoxButtons.OKCancel) = Windows.Forms.DialogResult.OK Then
                    Process.Start(UpdateURL)
                End If
            End If
        End If
    End Sub

    Private Sub PlayKeyButton_Click(sender As Object, e As EventArgs) Handles PlayKeyButton.Click
        Dim SelectKeyDialog As New SelectKey
        If SelectKeyDialog.ShowDialog = Windows.Forms.DialogResult.OK Then
            If Not SelectKeyDialog.ChosenKey = My.Settings.RelayKey Then
                My.Settings.PlayKey = SelectKeyDialog.ChosenKey
                My.Settings.Save()
                RefreshPlayKey()
            Else
                MessageBox.Show("Play key and relay key can not be the same!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If
        End If
    End Sub

    Private Sub RefreshPlayKey()
        PlayKeyButton.Text = String.Format("Play key: ""{0}"" (change)", My.Settings.PlayKey)
    End Sub

    Public Sub LogError(ByVal ex As Exception)
        If My.Settings.LogError Then
            Using log As StreamWriter = New StreamWriter("errorlog.txt", True)
                log.WriteLine("--------------------{0} UTC--------------------", DateTime.Now.ToUniversalTime)
                log.WriteLine(ex.ToString)
            End Using
        End If
    End Sub

    Private Sub ChangeDirButton_Click(sender As Object, e As EventArgs) Handles ChangeDirButton.Click
        SettingsForm.ShowDialog()
    End Sub

    Private Sub DeleteCFGs(ByVal Game As SourceGame, ByVal SteamappsPath As String)
        Dim GameDir As String = Path.Combine(SteamappsPath, Game.directory)
        Dim GameCfgFolder As String = Path.Combine(GameDir, Game.ToCfg)
        Dim SlamFiles() As String = {"slam.cfg", "slam_tracklist.cfg", "slam_relay.cfg", "slam_curtrack.cfg", "slam_saycurtrack.cfg", "slam_sayteamcurtrack.cfg"}
        Dim voicefile As String = Path.Combine(SteamappsPath, Game.directory) & "voice_input.wav"


        Try
            If File.Exists(voicefile) Then
                File.Delete(voicefile)
            End If

            For Each FileName In SlamFiles

                If File.Exists(GameCfgFolder & FileName) Then
                    File.Delete(GameCfgFolder & FileName)
                End If

            Next

        Catch ex As Exception
            LogError(ex)
        End Try

    End Sub

    Private Sub Form1_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If running Then
            StopPoll()
            ClosePending = True
            e.Cancel = True
        End If
    End Sub

    Private Sub LoadToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles LoadToolStripMenuItem.Click
        LoadTrack(GetCurrentGame, TrackList.SelectedItems(0).Index)
        DisplayLoaded(TrackList.SelectedItems(0).Index)
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        If My.Settings.MinimizeToSysTray Then
            If WindowState = FormWindowState.Minimized Then
                SystemTrayIcon.Visible = True
                SystemTrayIcon.BalloonTipIcon = ToolTipIcon.Info
                SystemTrayIcon.BalloonTipTitle = "SLAM"
                SystemTrayIcon.BalloonTipText = "Minimized to tray"
                SystemTrayIcon.ShowBalloonTip(50000)
                Hide()
                ShowInTaskbar = False
            End If
        End If
    End Sub

    Private Sub SystemTrayIcon_DoubleClick(sender As Object, e As EventArgs) Handles SystemTrayIcon.DoubleClick
        Show()
        ShowInTaskbar = True
        WindowState = FormWindowState.Normal
        SystemTrayIcon.Visible = False
    End Sub

    Private Sub SystemTrayMenu_OpenHandler(sender As Object, e As EventArgs) Handles SystemTrayMenu_Open.Click
        Show()
        ShowInTaskbar = True
        WindowState = FormWindowState.Normal
        SystemTrayIcon.Visible = False
    End Sub

    Private Sub SystemTrayMenu_StartStopHandler(sender As Object, e As EventArgs) Handles SystemTrayMenu_StartStop.Click
        If running Then
            StopPoll()
        Else
            StartPoll()
        End If
    End Sub

    Private Sub SystemTrayMenu_ExitHandler(sender As Object, e As EventArgs) Handles SystemTrayMenu_Exit.Click
        If running Then
            StopPoll()
            ClosePending = True
        Else
            Me.Close()
        End If
    End Sub

    Private Sub FindVirtualMicDevice()
        virtualMicDeviceInIndex = -1

        For i As Integer = 0 To WaveOut.DeviceCount - 1
            Try
                Dim deviceInfo = WaveOut.GetCapabilities(i)
                Dim deviceName As String = deviceInfo.ProductName.ToLower()

                ' Look for common virtual audio cable names
                If deviceName.Contains("CABLE Input") Or
                   deviceName.Contains("voicemeeter") Then
                    virtualMicDeviceInIndex = i
                    Debug.WriteLine($"Found virtual mic device: {deviceInfo.ProductName} at index {i}")
                    Exit For
                End If
            Catch ex As Exception
                ' Skip devices that can't be accessed
                Continue For
            End Try
        Next

        ' If no virtual device found, show available devices for user to choose
        If virtualMicDeviceInIndex = -1 Then
            ShowAudioDeviceSelector()
        End If
    End Sub

    Private Sub TestAudioPlayback()
        Try
            If virtualMicDeviceInIndex = -1 Then
                FindVirtualMicDevice()
            End If

            If virtualMicDeviceInIndex >= 0 Then
                ' Generate a test tone
                Dim testSignal = New SignalGenerator()
                testSignal.Gain = 0.2
                testSignal.Frequency = 440
                testSignal.Type = SignalGeneratorType.Sin

                Dim testOut = New WaveOut()
                testOut.DeviceNumber = virtualMicDeviceInIndex
                testOut.Init(testSignal)
                testOut.Play()

                MessageBox.Show("Playing test tone for 3 seconds. You should hear a beep in your virtual microphone.", "Test Audio")

                Threading.Thread.Sleep(3000)
                testOut.Stop()
                testOut.Dispose()

                MessageBox.Show("Test complete. Did you hear the tone?", "Test Results")
            Else
                MessageBox.Show("No virtual microphone device selected!", "Error")
            End If
        Catch ex As Exception
            MessageBox.Show($"Test failed: {ex.Message}", "Error")
            LogError(ex)
        End Try
    End Sub

    Private Sub OnPlaybackStopped(sender As Object, e As NAudio.Wave.StoppedEventArgs)
        Debug.WriteLine("Playback stopped")
        If e.Exception IsNot Nothing Then
            Debug.WriteLine($"Playback stopped with error: {e.Exception.Message}")
            LogError(e.Exception)
        End If
    End Sub

    ' Add this function to let user manually select audio device
    Private Sub ShowAudioDeviceSelector()
        Dim deviceList As New List(Of String)

        For i As Integer = 0 To WaveOut.DeviceCount - 1
            Try
                Dim deviceInfo = WaveOut.GetCapabilities(i)
                deviceList.Add($"{i}: {deviceInfo.ProductName}")
                Debug.WriteLine($"Device {i}: {deviceInfo.ProductName}")
            Catch ex As Exception
                deviceList.Add($"{i}: [Unavailable]")
            End Try
        Next

        Dim selectedDevice As String = InputBox("Virtual microphone not found automatically. " & vbCrLf &
                                               "Available audio devices:" & vbCrLf &
                                               String.Join(vbCrLf, deviceList) & vbCrLf & vbCrLf &
                                               "Enter the number of your virtual microphone device:",
                                               "Select Audio Device", "0")

        If IsNumeric(selectedDevice) Then
            Dim deviceIndex As Integer = Convert.ToInt32(selectedDevice)
            If deviceIndex >= 0 And deviceIndex < WaveOut.DeviceCount Then
                virtualMicDeviceInIndex = deviceIndex
                ' Save this setting for future use
                My.Settings.VirtualMicInDevice = deviceIndex
                My.Settings.Save()
                Debug.WriteLine($"Selected device index: {deviceIndex}")
            End If
        End If
    End Sub

    Private Sub PopulateAudioDeviceDropdowns()
        ' Populate output devices (for virtual cable) sound gets played INTO This
        ComboBoxVirtualInCable.Items.Clear()
        For i As Integer = 0 To WaveOut.DeviceCount - 1
            Dim deviceInfo = WaveOut.GetCapabilities(i)
            ComboBoxVirtualInCable.Items.Add($"{i}: {deviceInfo.ProductName}")
        Next
        If My.Settings.VirtualMicInDevice >= 0 AndAlso My.Settings.VirtualMicInDevice < ComboBoxVirtualInCable.Items.Count Then
            ComboBoxVirtualInCable.SelectedIndex = My.Settings.VirtualMicInDevice
        End If

        ' Populate input devices (for real mic) sound comes OUT of this
        ComboBoxVirtualOutCable.Items.Clear()
        ComboBoxRegularMic.Items.Clear()

        For Each device In enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active)
            Debug.WriteLine($"Virtual Out Device: {device.FriendlyName}")
            ComboBoxVirtualOutCable.Items.Add($"{device.FriendlyName}")
            ComboBoxRegularMic.Items.Add($"{device.FriendlyName}")
        Next

        If My.Settings.VirtualMicOutDevice >= 0 AndAlso My.Settings.VirtualMicOutDevice < ComboBoxVirtualOutCable.Items.Count Then
            ComboBoxVirtualOutCable.SelectedIndex = My.Settings.VirtualMicOutDevice
        End If
        If My.Settings.RegularMicDevice >= 0 AndAlso My.Settings.RegularMicDevice < ComboBoxRegularMic.Items.Count Then
            ComboBoxRegularMic.SelectedIndex = My.Settings.RegularMicDevice
        End If
    End Sub

    Private Sub ComboBoxVirtualInCable_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxVirtualInCable.SelectedIndexChanged
        My.Settings.VirtualMicInDevice = ComboBoxVirtualInCable.SelectedIndex
        virtualMicDeviceInIndex = ComboBoxVirtualInCable.SelectedIndex
        My.Settings.Save()
        Debug.Print($"Selected device index: {My.Settings.VirtualMicInDevice}")

    End Sub

    Private Sub ComboBoxVirtualOutCable_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxVirtualOutCable.SelectedIndexChanged
        My.Settings.VirtualMicOutDevice = ComboBoxVirtualOutCable.SelectedIndex
        My.Settings.VirtualMicOutDeviceName = ComboBoxVirtualOutCable.SelectedItem.ToString()

        Debug.Print($"Selected device index: {My.Settings.VirtualMicOutDevice}")
        Debug.Print($"Selected device name: {My.Settings.VirtualMicOutDeviceName}")
        My.Settings.Save()
    End Sub

    Private Sub ComboBoxRegularMic_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxRegularMic.SelectedIndexChanged
        My.Settings.RegularMicDevice = ComboBoxRegularMic.SelectedIndex
        My.Settings.RegularMicDeviceName = ComboBoxRegularMic.SelectedItem.ToString()
        Debug.Print($"Selected device index: {My.Settings.RegularMicDevice}")
        Debug.Print($"Selected device name: {My.Settings.RegularMicDeviceName}")
        My.Settings.Save()
    End Sub

End Class
