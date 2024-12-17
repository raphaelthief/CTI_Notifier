# CTI Notifier - Real-time RSS feed monitor for Windows OS

## 📝 Description

CTI Notifier is a lightweight Windows application that monitors a list of RSS feeds for cybersecurity-related updates or any other topics you choose. It periodically fetches news from the configured RSS feeds, checks for newly published content, and displays notifications in the system tray

The application is designed to process RSS feeds every 15 seconds and introduces a 1 hour pause after processing all feeds, ensuring a balanced system resource usage


## 🚀 Features

- Customizable RSS feed list : Easily add or modify RSS feeds of interest
- Configurable descriptions : Decide whether to display the feed descriptions in notifications
- Windows system tray notifications : Get non-intrusive balloon notifications with titles, descriptions, and direct clickable links

- Efficient scheduling : Processes RSS feeds at a 15 second interval & Introduces a 1 hour pause after completing the feed list before restarting

- Duplicate content management : Avoids notifying for the same link twice by tracking processed entries
- Silent operation : The application minimizes to the system tray and runs seamlessly in the background


## ⚙️ How It Works

1. RSS feed setup : Add your preferred RSS feed URLs and decide whether to display their descriptions (True or False)


![Add RSS](https://github.com/raphaelthief/CTI_Notifier/blob/main/Pictures/Add_RSS.JPG "Add_RSS")


2. Automated fetching :

- The program reads a single feed every 15 seconds
- A 1 hour pause is triggered after processing all feeds
- Feeds are re-queued for the next cycle


3. Notifications :

- Displays a system tray balloon with the feed title and description (if enabled)
- Clicking the notification opens the corresponding link in the default browser


4. Minimized UI :

- The application minimizes to the system tray and can be exited via a right-click context menu


## 🛠️ Requirements

- Windows OS (tested on Windows 10+)
- .NET Framework (version 4.7.2 or higher)


## 📂 How to Use

- Download the source code (Form1.vb)
- Compile it in Visual Studio 
- Edit the RssFeeds list to include your preferred RSS URLs and configure their display options
- Build and run the project


## 🖥️ UI Behavior

- The program starts minimized
- Notifications appear in the system tray for new entries
- Use the 'Close' option in the tray icon context menu to exit the application


## 🛡️ Example Use Cases

- Cyber Threat Intelligence : Stay updated with cybersecurity news, breaches, and alerts
- Monitoring blogs or news : Fetch and track news updates from your favorite blogs or websites
- IT security alerts : Integrate feeds from security organizations like Microsoft, CERT, or ZDI


## 📜 Code Highlights

1. Efficient Timer Management :

- RssTimer : Processes feeds at 15 second intervals
- PauseTimer : Manages a 1 hour pause after processing all feeds


2. Duplicate Prevention :

- Uses a HashSet to track already seen links


3. Robust RSS Parsing :

- Supports multiple date formats, including standard RSS, UTC (Z), and ISO 8601


## 🛡️ Sample Notification

When a new entry is detected :


![Notification](https://github.com/raphaelthief/CTI_Notifier/blob/main/Pictures/Notification.JPG "Notification")


