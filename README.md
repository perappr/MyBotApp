# MyBotApp
A sample bot app that integrates with Yahoo Finance API and OpenWeatherMap API.
The sample app uses two class files from the sources below. 

* [An Interactive Bot Application With LUIS Using Microsoft Bot Framework](http://www.c-sharpcorner.com/article/an-interactive-bot-application-with-luis-using-microsoft-bot/) - Yahoo Finance API 
* [Introduction to the Bot Framework: Building a weather bot](https://github.com/mmgrt/streamcode) - OpenWeatherMap API


## Getting Started

* [Getting started with the Connector](https://docs.botframework.com/en-us/csharp/builder/sdkreference/gettingstarted.html) 

Replace the xxx values in the web.config file.

    <add key="BotId" value="xxx" /> 
    <add key="MicrosoftAppId" value="xxx" />
    <add key="MicrosoftAppPassword" value="xxx" />
    <add key="OpenWeatherMap_App_Id" value="xxx" />
    <add key="LuisURI" value="https://xxx.api.cognitive.microsoft.com/luis/v2.0/apps/xxx?subscription-key=xxx" />
