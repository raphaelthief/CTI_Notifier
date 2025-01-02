Imports System.Net
Imports System.Xml
Imports System.Globalization

Public Class Form1


#Region "Declarations"

    '################################################################################################################################################
    '################################################------------ ADD YOUR RSS LIST HERE ------------################################################
    '################################################################################################################################################

    ' List of RSS feeds with a boolean to show the description or not (True / False)
    Dim RssFeeds As New Dictionary(Of String, Boolean) From {
        {"https://www.exploit-db.com/rss.xml", True},
        {"https://feeds.feedburner.com/HaveIBeenPwnedLatestBreaches", False},
        {"https://red.flag.domains/index.xml", True},
        {"https://www.zerodayinitiative.com/rss/upcoming/", True},
        {"https://api.msrc.microsoft.com/update-guide/rss", True},
        {"https://www.cert.ssi.gouv.fr/alerte/feed/", True},
        {"https://leak-lookup.com/rss", True}
    }


    '################################################################################################################################################
    '##############################################------------ CHANGE TIME IF YOU WANT TO ------------##############################################
    '################################################################################################################################################

    ' Declaration of variables
    Private SeenEntries As New HashSet(Of String)
    Private FeedQueue As New Queue(Of KeyValuePair(Of String, Boolean)) ' Processing queue
    Private WithEvents RssTimer As New Timer With {.Interval = 15000} ' 15 second interval
    Private WithEvents PauseTimer As New Timer With {.Interval = 3600000} ' 1 hour interval 

#End Region


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        InitializeFeedQueue()

        ' Configure the notification icon
        Dim contextMenu As New ContextMenuStrip()
        Dim exitItem As New ToolStripMenuItem("Close", Nothing, AddressOf ExitProgram)
        contextMenu.Items.Add(exitItem)
        NotifyIcon1.ContextMenuStrip = contextMenu

        ' Start the Timer to process RSS feeds
        RssTimer.Start()

        ' Minimize the application to the system tray
        Me.WindowState = FormWindowState.Minimized
        Me.ShowInTaskbar = False
        Hide()
    End Sub


#Region "RSS & timer"

    ' Initialize or reset the feed queue
    Private Sub InitializeFeedQueue()
        FeedQueue.Clear()
        For Each feed In RssFeeds
            FeedQueue.Enqueue(feed)
        Next
    End Sub

    ' Event triggered by the Timer every 15 seconds
    Private Sub RssTimer_Tick(sender As Object, e As EventArgs) Handles RssTimer.Tick
        If FeedQueue.Count > 0 Then
            Dim currentFeed = FeedQueue.Dequeue()
            ProcessRssFeed(currentFeed.Key, currentFeed.Value)
        End If

        ' Check if all feeds have been processed
        If FeedQueue.Count = 0 Then
            RssTimer.Stop()  ' Stop the 15 second Timer
            PauseTimer.Start()  ' Start the 1 hour pause Timer
        End If
    End Sub

    ' Event triggered after 1 hour of pause
    Private Sub PauseTimer_Tick(sender As Object, e As EventArgs) Handles PauseTimer.Tick
        PauseTimer.Stop()  ' Stop the pause Timer
        InitializeFeedQueue()  ' Reinitialize the feed queue
        RssTimer.Start()  ' Restart the 15-second Timer
    End Sub


    Private Sub ProcessRssFeed(url As String, showDescription As Boolean)
        Try
            Dim acceptedFormats As String() = {
            "ddd, dd MMM yyyy HH:mm:ss K", ' Standard RSS format (English/French)
            "ddd, dd MMM yyyy HH:mm:ss 'Z'", ' UTC format (Z)
            "ddd, dd MMM yyyy HH:mm K",    ' Format without seconds (rare)
            "yyyy-MM-ddTHH:mm:ssZ"         ' ISO 8601 format (ex : 2024-06-17T15:30:00Z)
        }

            Dim rssReader As XmlReader = XmlReader.Create(url)
            Dim rssFeed As New XmlDocument()
            rssFeed.Load(rssReader)

            ' Use the current date (UTC)
            Dim todayUtc As DateTime = DateTime.UtcNow.Date

            ' Iterate through <item> elements
            For Each item As XmlNode In rssFeed.SelectNodes("//item")
                Dim titleNode As XmlNode = item.SelectSingleNode("title")
                Dim linkNode As XmlNode = item.SelectSingleNode("link")
                Dim pubDateNode As XmlNode = item.SelectSingleNode("pubDate")
                Dim descriptionNode As XmlNode = item.SelectSingleNode("description")

                Dim title As String = If(titleNode IsNot Nothing, titleNode.InnerText, "Titre inconnu")
                Dim link As String = If(linkNode IsNot Nothing, linkNode.InnerText, "")
                Dim description As String = If(descriptionNode IsNot Nothing, descriptionNode.InnerText, "Pas de description disponible.")

                ' Date management
                Dim pubDate As DateTime
                Dim pubDateStr As String = If(pubDateNode IsNot Nothing, pubDateNode.InnerText, "")
                Dim success As Boolean = False

                For Each format As String In acceptedFormats
                    If DateTimeOffset.TryParseExact(pubDateStr, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, Nothing) Then
                        pubDate = DateTimeOffset.ParseExact(pubDateStr, format, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal).UtcDateTime.Date
                        success = True
                        Exit For
                    End If
                Next

                If Not success Then
                    pubDate = todayUtc ' Default to today if parsing fails
                End If

                ' If the date matches today and the link is new
                If pubDate = todayUtc AndAlso Not SeenEntries.Contains(link) Then
                    SeenEntries.Add(link)
                    ShowNotification(title, If(showDescription, description, "Click to open the RSS link"), link)
                End If
            Next
        Catch ex As Exception
            'MsgBox("Error " & url & " : " & ex.Message)
        End Try
    End Sub

#End Region


#Region "notifications & form stuffs"

    ' Function to display Windows notifications
    Private Sub ShowNotification(title As String, text As String, link As String)
        NotifyIcon1.BalloonTipTitle = "CTI notifier : " & title
        NotifyIcon1.BalloonTipText = text
        NotifyIcon1.BalloonTipIcon = ToolTipIcon.Info

        NotifyIcon1.ShowBalloonTip(5000)
        AddHandler NotifyIcon1.BalloonTipClicked, Sub() Process.Start(link)
    End Sub

    Private Sub ExitProgram(sender As Object, e As EventArgs)
        NotifyIcon1.Visible = False
        Application.Exit()
    End Sub

    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles MyBase.Resize
        If Me.WindowState = FormWindowState.Minimized Then
            Me.ShowInTaskbar = False
            NotifyIcon1.Visible = True
        End If
    End Sub

#End Region


End Class
