# SaneNetCore
A .Net Core scanner web app that interfaces with the SANE CLI

This is a COVID inspired project. With family members now working and learning from home I've converted my old ScanSnap scanner to be a web enabled scanner via a Raspberry PI and the SANE packages.

This is .Net Core 3.1 web app that uses the SANE scanimage CLI to access a scanner. I use it on a Raspberry PI, so it should work for you on any linux flavor where you can run scanimage and .Net Core. Being a web app, you can access the scanner from any device that has a browser. It should render fine on a phone.

Not all of the scanimage features are implemented yet.

## Considerations
1. There is no access control. If you can access the web app, you can kick off a scan and download any scanned documents.
1. There is also no hardening of the file download process. It's possible that you could download any files available to the web app user.  
1. Converting the pages into a PDF takes a VERY LONG time on my PI. There is room for improvement here, but I took a shortcut and so that's the tradeoff.
1. Ideally this should talk directly to a SANE server, which would let you run this on a separate machine and perhaps access multiple scanners. This will come later as I could not find any SANE libraries for .Net Core.

## Building
I'm using Visual Studio 2019 to build this project. However, it's a very simple build, so you should also be able to use the dotnet tool to build and publish. This readme assumes you're quite familiar with Linux and .Net core. Apologies if these assumptions make things harder on you. I will clean up the documentation as time allows.

## Running on a Raspberry PI
This web app runs fine on a Raspberry PI 2 Model B. I tried getting it to run on an original Raspberry PI Model B+, but the .Net core runtime seg faulted when I tried to run it there, so until .Net core runs on a Model B+, this is the lowest model of the Raspberry PI that will work.