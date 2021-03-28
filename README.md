Welcome to ValourBank repository

ValourBank - we hold your password in plaintext!

# # # # # # # # #

Build using: Visual Studio 2019
.NET Version: 4.8

API runs on localhost:8080 by default.
Port number can be changed by modifying ValourBankApi/Program.cs:13 line:
var srvConf = new ServerConfiguration(8081);

To run test, open .sln file in VS2019 and navigate to Test Explorer. Run API (e.g. from .exe or from other VS instance) and then launch test.

Client handles login, cash withdraw or deposit, and logout while quitting application. 