<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form1))
        Me.GameSelector = New System.Windows.Forms.ComboBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.ImportButton = New System.Windows.Forms.Button()
        Me.TrackList = New System.Windows.Forms.ListView()
        Me.LoadedCol = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TrackCol = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.HotKeyCol = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.VolumeCol = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.Trimmed = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.TagsCol = CType(New System.Windows.Forms.ColumnHeader(), System.Windows.Forms.ColumnHeader)
        Me.StartButton = New System.Windows.Forms.Button()
        Me.ImportDialog = New System.Windows.Forms.OpenFileDialog()
        Me.ProgressBar1 = New System.Windows.Forms.ProgressBar()
        Me.WavWorker = New System.ComponentModel.BackgroundWorker()
        Me.PollRelayWorker = New System.ComponentModel.BackgroundWorker()
        Me.ChangeDirButton = New System.Windows.Forms.Button()
        Me.TrackContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.ContextDelete = New System.Windows.Forms.ToolStripMenuItem()
        Me.GoToToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextRefresh = New System.Windows.Forms.ToolStripMenuItem()
        Me.RemoveHotkeyToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.RenameToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ContextHotKey = New System.Windows.Forms.ToolStripMenuItem()
        Me.SetVolumeToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TrimToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.LoadToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PlayKeyButton = New System.Windows.Forms.Button()
        Me.StatusLabel = New System.Windows.Forms.Label()
        Me.SystemTrayIcon = New System.Windows.Forms.NotifyIcon(Me.components)
        Me.SystemTrayMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.SystemTrayMenu_Open = New System.Windows.Forms.ToolStripMenuItem()
        Me.SystemTrayMenu_StartStop = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.SystemTrayMenu_Exit = New System.Windows.Forms.ToolStripMenuItem()
        Me.YTButton = New System.Windows.Forms.Button()
        Me.ComboBoxVirtualInCable = New System.Windows.Forms.ComboBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.ComboBoxRegularMic = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.ComboBoxVirtualOutCable = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.TrackContextMenu.SuspendLayout()
        Me.SystemTrayMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'GameSelector
        '
        Me.GameSelector.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GameSelector.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.GameSelector.FormattingEnabled = True
        Me.GameSelector.Location = New System.Drawing.Point(103, 22)
        Me.GameSelector.Margin = New System.Windows.Forms.Padding(6)
        Me.GameSelector.MaxDropDownItems = 100
        Me.GameSelector.Name = "GameSelector"
        Me.GameSelector.Size = New System.Drawing.Size(794, 32)
        Me.GameSelector.TabIndex = 0
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(22, 28)
        Me.Label1.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(71, 25)
        Me.Label1.TabIndex = 1
        Me.Label1.Text = "Game:"
        '
        'ImportButton
        '
        Me.ImportButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.ImportButton.Location = New System.Drawing.Point(28, 850)
        Me.ImportButton.Margin = New System.Windows.Forms.Padding(6)
        Me.ImportButton.Name = "ImportButton"
        Me.ImportButton.Size = New System.Drawing.Size(99, 42)
        Me.ImportButton.TabIndex = 3
        Me.ImportButton.Text = "Import"
        Me.ImportButton.UseVisualStyleBackColor = True
        '
        'TrackList
        '
        Me.TrackList.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TrackList.AutoArrange = False
        Me.TrackList.Columns.AddRange(New System.Windows.Forms.ColumnHeader() {Me.LoadedCol, Me.TrackCol, Me.HotKeyCol, Me.VolumeCol, Me.Trimmed, Me.TagsCol})
        Me.TrackList.FullRowSelect = True
        Me.TrackList.HideSelection = False
        Me.TrackList.ImeMode = System.Windows.Forms.ImeMode.Off
        Me.TrackList.Location = New System.Drawing.Point(28, 72)
        Me.TrackList.Margin = New System.Windows.Forms.Padding(6)
        Me.TrackList.Name = "TrackList"
        Me.TrackList.Size = New System.Drawing.Size(1018, 702)
        Me.TrackList.TabIndex = 4
        Me.TrackList.UseCompatibleStateImageBehavior = False
        Me.TrackList.View = System.Windows.Forms.View.Details
        '
        'LoadedCol
        '
        Me.LoadedCol.Text = "Loaded"
        '
        'TrackCol
        '
        Me.TrackCol.Text = "Track"
        Me.TrackCol.Width = 137
        '
        'HotKeyCol
        '
        Me.HotKeyCol.Text = "Bind"
        '
        'VolumeCol
        '
        Me.VolumeCol.Text = "Volume"
        Me.VolumeCol.Width = 100
        '
        'Trimmed
        '
        Me.Trimmed.Text = "Trimmed"
        '
        'TagsCol
        '
        Me.TagsCol.Text = "Tags"
        Me.TagsCol.Width = 43
        '
        'StartButton
        '
        Me.StartButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.StartButton.Location = New System.Drawing.Point(176, 850)
        Me.StartButton.Margin = New System.Windows.Forms.Padding(6)
        Me.StartButton.Name = "StartButton"
        Me.StartButton.Size = New System.Drawing.Size(138, 42)
        Me.StartButton.TabIndex = 5
        Me.StartButton.Text = "Start"
        Me.StartButton.UseVisualStyleBackColor = True
        '
        'ImportDialog
        '
        Me.ImportDialog.Filter = "Media files|*.mp3;*.wav;*.aac;*.wma;*.m4a;*.mp4;*.wmv;*.avi;*.m4v;*.mov;|Audio fi" &
    "les|*.mp3;*.wav;*.aac;*.wma;*.m4a;|Video files|*.mp4;*.wmv;*.avi;*.m4v;*.mov;|Al" &
    "l files|*.*"
        Me.ImportDialog.Multiselect = True
        '
        'ProgressBar1
        '
        Me.ProgressBar1.Anchor = CType(((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ProgressBar1.Location = New System.Drawing.Point(28, 904)
        Me.ProgressBar1.Margin = New System.Windows.Forms.Padding(6)
        Me.ProgressBar1.Name = "ProgressBar1"
        Me.ProgressBar1.Size = New System.Drawing.Size(1021, 42)
        Me.ProgressBar1.Step = 1
        Me.ProgressBar1.TabIndex = 6
        '
        'WavWorker
        '
        Me.WavWorker.WorkerReportsProgress = True
        '
        'PollRelayWorker
        '
        Me.PollRelayWorker.WorkerReportsProgress = True
        Me.PollRelayWorker.WorkerSupportsCancellation = True
        '
        'ChangeDirButton
        '
        Me.ChangeDirButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.ChangeDirButton.Location = New System.Drawing.Point(911, 18)
        Me.ChangeDirButton.Margin = New System.Windows.Forms.Padding(6)
        Me.ChangeDirButton.Name = "ChangeDirButton"
        Me.ChangeDirButton.Size = New System.Drawing.Size(138, 42)
        Me.ChangeDirButton.TabIndex = 7
        Me.ChangeDirButton.Text = "Settings"
        Me.ChangeDirButton.UseVisualStyleBackColor = True
        '
        'TrackContextMenu
        '
        Me.TrackContextMenu.ImageScalingSize = New System.Drawing.Size(24, 24)
        Me.TrackContextMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ContextDelete, Me.GoToToolStripMenuItem, Me.ContextRefresh, Me.RemoveHotkeyToolStripMenuItem, Me.RenameToolStripMenuItem, Me.ContextHotKey, Me.SetVolumeToolStripMenuItem, Me.TrimToolStripMenuItem, Me.LoadToolStripMenuItem})
        Me.TrackContextMenu.Name = "TrackContextMenu"
        Me.TrackContextMenu.Size = New System.Drawing.Size(208, 328)
        '
        'ContextDelete
        '
        Me.ContextDelete.Name = "ContextDelete"
        Me.ContextDelete.Size = New System.Drawing.Size(207, 36)
        Me.ContextDelete.Text = "Delete"
        '
        'GoToToolStripMenuItem
        '
        Me.GoToToolStripMenuItem.Name = "GoToToolStripMenuItem"
        Me.GoToToolStripMenuItem.Size = New System.Drawing.Size(207, 36)
        Me.GoToToolStripMenuItem.Text = "Go To"
        '
        'ContextRefresh
        '
        Me.ContextRefresh.Name = "ContextRefresh"
        Me.ContextRefresh.Size = New System.Drawing.Size(207, 36)
        Me.ContextRefresh.Text = "Refresh"
        '
        'RemoveHotkeyToolStripMenuItem
        '
        Me.RemoveHotkeyToolStripMenuItem.Name = "RemoveHotkeyToolStripMenuItem"
        Me.RemoveHotkeyToolStripMenuItem.Size = New System.Drawing.Size(207, 36)
        Me.RemoveHotkeyToolStripMenuItem.Text = "Remove Bind"
        '
        'RenameToolStripMenuItem
        '
        Me.RenameToolStripMenuItem.Name = "RenameToolStripMenuItem"
        Me.RenameToolStripMenuItem.Size = New System.Drawing.Size(207, 36)
        Me.RenameToolStripMenuItem.Text = "Rename"
        '
        'ContextHotKey
        '
        Me.ContextHotKey.Name = "ContextHotKey"
        Me.ContextHotKey.Size = New System.Drawing.Size(207, 36)
        Me.ContextHotKey.Text = "Set Bind"
        '
        'SetVolumeToolStripMenuItem
        '
        Me.SetVolumeToolStripMenuItem.Name = "SetVolumeToolStripMenuItem"
        Me.SetVolumeToolStripMenuItem.Size = New System.Drawing.Size(207, 36)
        Me.SetVolumeToolStripMenuItem.Text = "Set Volume"
        '
        'TrimToolStripMenuItem
        '
        Me.TrimToolStripMenuItem.Name = "TrimToolStripMenuItem"
        Me.TrimToolStripMenuItem.Size = New System.Drawing.Size(207, 36)
        Me.TrimToolStripMenuItem.Text = "Trim"
        '
        'LoadToolStripMenuItem
        '
        Me.LoadToolStripMenuItem.Name = "LoadToolStripMenuItem"
        Me.LoadToolStripMenuItem.Size = New System.Drawing.Size(207, 36)
        Me.LoadToolStripMenuItem.Text = "Load"
        '
        'PlayKeyButton
        '
        Me.PlayKeyButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PlayKeyButton.Location = New System.Drawing.Point(697, 850)
        Me.PlayKeyButton.Margin = New System.Windows.Forms.Padding(6)
        Me.PlayKeyButton.Name = "PlayKeyButton"
        Me.PlayKeyButton.Size = New System.Drawing.Size(352, 42)
        Me.PlayKeyButton.TabIndex = 8
        Me.PlayKeyButton.Text = "Play key: """"{0}"""" (change)"
        Me.PlayKeyButton.UseVisualStyleBackColor = True
        '
        'StatusLabel
        '
        Me.StatusLabel.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.StatusLabel.AutoSize = True
        Me.StatusLabel.Location = New System.Drawing.Point(324, 860)
        Me.StatusLabel.Margin = New System.Windows.Forms.Padding(6, 0, 6, 0)
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusLabel.Size = New System.Drawing.Size(110, 25)
        Me.StatusLabel.TabIndex = 9
        Me.StatusLabel.Text = "Status: Idle"
        '
        'SystemTrayIcon
        '
        Me.SystemTrayIcon.ContextMenuStrip = Me.SystemTrayMenu
        Me.SystemTrayIcon.Icon = CType(resources.GetObject("SystemTrayIcon.Icon"), System.Drawing.Icon)
        Me.SystemTrayIcon.Text = "SLAM"
        '
        'SystemTrayMenu
        '
        Me.SystemTrayMenu.ImageScalingSize = New System.Drawing.Size(28, 28)
        Me.SystemTrayMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.SystemTrayMenu_Open, Me.SystemTrayMenu_StartStop, Me.ToolStripSeparator1, Me.SystemTrayMenu_Exit})
        Me.SystemTrayMenu.Name = "SystemTrayMenu"
        Me.SystemTrayMenu.Size = New System.Drawing.Size(138, 118)
        '
        'SystemTrayMenu_Open
        '
        Me.SystemTrayMenu_Open.Name = "SystemTrayMenu_Open"
        Me.SystemTrayMenu_Open.Size = New System.Drawing.Size(137, 36)
        Me.SystemTrayMenu_Open.Text = "Open"
        '
        'SystemTrayMenu_StartStop
        '
        Me.SystemTrayMenu_StartStop.Name = "SystemTrayMenu_StartStop"
        Me.SystemTrayMenu_StartStop.Size = New System.Drawing.Size(137, 36)
        Me.SystemTrayMenu_StartStop.Text = "Start"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(134, 6)
        '
        'SystemTrayMenu_Exit
        '
        Me.SystemTrayMenu_Exit.Name = "SystemTrayMenu_Exit"
        Me.SystemTrayMenu_Exit.Size = New System.Drawing.Size(137, 36)
        Me.SystemTrayMenu_Exit.Text = "Exit"
        '
        'YTButton
        '
        Me.YTButton.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.YTButton.Image = CType(resources.GetObject("YTButton.Image"), System.Drawing.Image)
        Me.YTButton.Location = New System.Drawing.Point(125, 850)
        Me.YTButton.Margin = New System.Windows.Forms.Padding(6)
        Me.YTButton.Name = "YTButton"
        Me.YTButton.Size = New System.Drawing.Size(40, 42)
        Me.YTButton.TabIndex = 10
        Me.YTButton.UseVisualStyleBackColor = True
        '
        'ComboBoxVirtualInCable
        '
        Me.ComboBoxVirtualInCable.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.ComboBoxVirtualInCable.FormattingEnabled = True
        Me.ComboBoxVirtualInCable.Location = New System.Drawing.Point(30, 809)
        Me.ComboBoxVirtualInCable.Name = "ComboBoxVirtualInCable"
        Me.ComboBoxVirtualInCable.Size = New System.Drawing.Size(335, 32)
        Me.ComboBoxVirtualInCable.TabIndex = 11
        '
        'Label2
        '
        Me.Label2.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(35, 780)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(94, 25)
        Me.Label2.TabIndex = 12
        Me.Label2.Text = "Virtual In:"
        '
        'ComboBoxRegularMic
        '
        Me.ComboBoxRegularMic.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.ComboBoxRegularMic.FormattingEnabled = True
        Me.ComboBoxRegularMic.Location = New System.Drawing.Point(715, 809)
        Me.ComboBoxRegularMic.Name = "ComboBoxRegularMic"
        Me.ComboBoxRegularMic.Size = New System.Drawing.Size(335, 32)
        Me.ComboBoxRegularMic.TabIndex = 13
        '
        'Label3
        '
        Me.Label3.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(719, 781)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(49, 25)
        Me.Label3.TabIndex = 14
        Me.Label3.Text = "Mic:"
        '
        'ComboBoxVirtualOutCable
        '
        Me.ComboBoxVirtualOutCable.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.ComboBoxVirtualOutCable.FormattingEnabled = True
        Me.ComboBoxVirtualOutCable.Location = New System.Drawing.Point(373, 809)
        Me.ComboBoxVirtualOutCable.Name = "ComboBoxVirtualOutCable"
        Me.ComboBoxVirtualOutCable.Size = New System.Drawing.Size(335, 32)
        Me.ComboBoxVirtualOutCable.TabIndex = 15
        '
        'Label4
        '
        Me.Label4.Anchor = System.Windows.Forms.AnchorStyles.Bottom
        Me.Label4.AutoSize = True
        Me.Label4.Location = New System.Drawing.Point(378, 780)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(110, 25)
        Me.Label4.TabIndex = 16
        Me.Label4.Text = "Virtual Out:"
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(11.0!, 24.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1071, 952)
        Me.Controls.Add(Me.Label4)
        Me.Controls.Add(Me.ComboBoxVirtualOutCable)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.ComboBoxRegularMic)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.ComboBoxVirtualInCable)
        Me.Controls.Add(Me.YTButton)
        Me.Controls.Add(Me.StatusLabel)
        Me.Controls.Add(Me.PlayKeyButton)
        Me.Controls.Add(Me.ChangeDirButton)
        Me.Controls.Add(Me.ProgressBar1)
        Me.Controls.Add(Me.StartButton)
        Me.Controls.Add(Me.TrackList)
        Me.Controls.Add(Me.ImportButton)
        Me.Controls.Add(Me.Label1)
        Me.Controls.Add(Me.GameSelector)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Margin = New System.Windows.Forms.Padding(6)
        Me.MinimumSize = New System.Drawing.Size(897, 684)
        Me.Name = "Form1"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Source Live Audio Mixer"
        Me.TrackContextMenu.ResumeLayout(False)
        Me.SystemTrayMenu.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents GameSelector As System.Windows.Forms.ComboBox
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents ImportButton As System.Windows.Forms.Button
    Friend WithEvents TrackList As System.Windows.Forms.ListView
    Friend WithEvents LoadedCol As System.Windows.Forms.ColumnHeader
    Friend WithEvents TrackCol As System.Windows.Forms.ColumnHeader
    Friend WithEvents TagsCol As System.Windows.Forms.ColumnHeader
    Friend WithEvents StartButton As System.Windows.Forms.Button
    Friend WithEvents ImportDialog As System.Windows.Forms.OpenFileDialog
    Friend WithEvents ProgressBar1 As System.Windows.Forms.ProgressBar
    Friend WithEvents WavWorker As System.ComponentModel.BackgroundWorker
    Friend WithEvents PollRelayWorker As System.ComponentModel.BackgroundWorker
    Friend WithEvents ChangeDirButton As System.Windows.Forms.Button
    Friend WithEvents TrackContextMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents ContextDelete As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContextRefresh As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents ContextHotKey As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents HotKeyCol As System.Windows.Forms.ColumnHeader
    Friend WithEvents RemoveHotkeyToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents GoToToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents PlayKeyButton As System.Windows.Forms.Button
    Friend WithEvents VolumeCol As System.Windows.Forms.ColumnHeader
    Friend WithEvents SetVolumeToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TrimToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents Trimmed As System.Windows.Forms.ColumnHeader
    Friend WithEvents StatusLabel As System.Windows.Forms.Label
    Friend WithEvents RenameToolStripMenuItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LoadToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents SystemTrayIcon As NotifyIcon
    Friend WithEvents SystemTrayMenu As ContextMenuStrip
    Friend WithEvents SystemTrayMenu_Open As ToolStripMenuItem
    Friend WithEvents SystemTrayMenu_StartStop As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents SystemTrayMenu_Exit As ToolStripMenuItem
    Friend WithEvents YTButton As Button
    Friend WithEvents ComboBoxVirtualInCable As ComboBox
    Friend WithEvents Label2 As Label
    Friend WithEvents ComboBoxRegularMic As ComboBox
    Friend WithEvents Label3 As Label
    Friend WithEvents ComboBoxVirtualOutCable As ComboBox
    Friend WithEvents Label4 As Label
End Class
