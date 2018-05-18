# cloud-shell
Code from my ['Build your Cloud Shell'](https://www.phdays.com/en/program/reports/build-your-own-cloud-shell/) talk from PHDays 2018.

[Demo video](https://www.youtube.com/watch?v=POrizEypceA&feature=youtu.be) with Google Drive as communication channel between attacker and attacked host.

[Slides](https://www.slideshare.net/GermanNamestnikov/build-your-own-cloudshell).

# Description
CloudShell is a small project that allows you to build remote access shells working via different Cloud Services. Usually, that allows you to avoid IDS/IPS detection during your operations.

# GoogleDriveShell
GoogleDrive Shell is an implementation of CloudShell working with - surprise! - Google Drive service. 
Check the next files to understand it:
* GoogleDriveManager__Server.cs / GoogleDriveManager__Client.cs
* GoogleDriveSessionManager__Server.cs / GoogleDriveSessionManager__Client.cs

GoogleDriveSessionManager__*(string serviceAccountEmail, string certificateBase64) constructors require email and p12 certificate of your Google Drive service account. 

In two words, service accounts are created to build server-to-server communications in Google Drive, you can read more [here](https://developers.google.com/identity/protocols/OAuth2ServiceAccount).

# How to build my own cloud shell?
Nice question! Understand the CloudShell workflow first, and implement ISessionManager__Server / ISessionManager__Clients for your service.
