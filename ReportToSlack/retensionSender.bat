"c:\Program files\Python36\python.exe" c:\project\KServer\ReportToSlack\retension_report.py

"c:\Program files\Python36\python.exe" c:\project\KServer\ReportToSlack\daily_report.py

"c:\windows\system32\curl" -F file=@c:\project\KServer\ReportToSlack\retension.csv -F "initial_comment=Retension Report" -F channels=report -H "Authorization: Bearer xoxp-329114544966-482964524117-550530164467-54e01b1305fe4b403f784abaa272fd78" https://slack.com/api/files.upload

"c:\windows\system32\curl" -F file=@c:\project\KServer\ReportToSlack\retension_count.csv -F "initial_comment=Retension Count Report" -F channels=report -H "Authorization: Bearer xoxp-329114544966-482964524117-550530164467-54e01b1305fe4b403f784abaa272fd78" https://slack.com/api/files.upload

"c:\windows\system32\curl" -F file=@c:\project\KServer\ReportToSlack\sales_report.csv -F "initial_comment=Sales Report " -F channels=report -H "Authorization: Bearer xoxp-329114544966-482964524117-550530164467-54e01b1305fe4b403f784abaa272fd78" https://slack.com/api/files.upload

"c:\windows\system32\curl" -F file=@c:\project\KServer\ReportToSlack\dau_sales_report.csv -F "initial_comment=Sales Dau Report " -F channels=report -H "Authorization: Bearer xoxp-329114544966-482964524117-550530164467-54e01b1305fe4b403f784abaa272fd78" https://slack.com/api/files.upload
