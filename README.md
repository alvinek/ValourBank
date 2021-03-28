# ValourBank - we hold your password in plaintext!

We rethinked our decisions and now we hold your password in encrypted form. Don't worry, you will be still valour, because password to the database is in the code files, and it is publicly visible on GitHub.


Build using: Visual Studio 2019

.NET Version: 4.8



API runs on localhost:8080 by default.
Port number can be changed by modifying ValourBankApi/Program.cs:13 line:

`var srvConf = new ServerConfiguration(8081);`

To run test, open .sln file in VS2019 and navigate to Test Explorer. Run API seperately on your machine (e.g. from .exe or from other VS instance) and then launch test.

This was written as an assignment to show that we both can write client and server application.

Client handles login, cash withdraw or deposit, and logout while quitting application. Tests are bombarding API to see if it survive.
