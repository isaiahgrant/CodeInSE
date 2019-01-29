# CodeInSE
A C#-based framework, built on Malware's MDK, used for development of Ingame scripts in Keen Software House's game Space Engineers.

KSH: https://www.spaceengineersgame.com/
MDK: https://github.com/malware-dev/MDK-SE/wiki -- I highly suggest you familiarize yourself with this, I will reference it regularly.

To borrow a line from Malware:
Space Engineers is trademarked to Keen Software House. This framework is fan-made, and its developer has no relation to Keen Software House.

## Basic Usage
Here we will walk through the simple example program provided with this project.

Common terms used are defined as follows:
* SE: Space Engineers, silly :).
* Program, base Program: The base class of any SE script.
* PB: The ingame programmable block that contains and executes your SE script.
* Custom Program: The base class of any functionality you write using this framework.
* Command: Arguments are the string passed to the PB when the PB is ran. Commands are arguments that your script expects and have logic (code) to handle.
* Hello Program: The example program (derives from Custom Program).

### Hello Program
Hello Program utilizes most of the functionality you require when writing scripts in SE. Lets explore those and the simplicity of the framework.

#### Constructor
The Hello Program is initialized as part of the base Program's constructor. A snippet from the MDK in reference to the base Program constructor:
```
It is used for one-time initialization of your script, and is run once every time your script is instantiated. This means it's run once after your game is loaded, or after you recompile your script. Recompiling happens when you have edited your script or when you press the Recompile button. The constructor is optional, you don't need it to have a working script.

https://github.com/malware-dev/MDK-SE/wiki/The-Anatomy-of-a-Script
```

In this example we utilize the constructor to define our Custom Program. Our definition includes:
* Whether or not our custom program will loop (run itself again and again) or only run once.
* Commands (arguments) that our program will response to and the functions that it will use to handle them.
* Writes to the Logging module (more later) that initilization is complete.
```
public HelloProgram()
{
    // This program only runs once before terminating (does not loop).
    // See Base class (CustomProgram) for more information.
    _program.Runtime.UpdateFrequency = UpdateFrequency.None;

    // Declare how your program will handle commands (map a function to a command).
    _commandHandlers = new Dictionary<string, System.Action>();
    _commandHandlers[Command.SETUP] = Setup;
    _commandHandlers[Command.NONE] = None;
    _commandHandlers[Command.RESET] = Reset;
    _commandHandlers[Command.HELLO] = Hello;

    _program._logger.SysLog<HelloProgram>("initialized.");
}
```
#### Hello Command Handler
The other command handlers are simple examples, so we will spend our time focusing on the Hello command handler.
The purpose of this function is to showcase the simplicity of the framework. Here are all the things accomplished in the small amount of code you see here:
* The program outputs a log header and body to the PB's output (Echo) _and_ any pre-configured LCDs through simple Logger module functions.
* The program utilizes data that was saved from a previous execution of the script.
* The program utilizes configuration through the PB's CustomData to allow customizable behavior. In this case simply setting the name of the PB to the value set in configuration.
* Finally, the program logs what it accomplished.
```
public void Hello()
{
    // Write a message from your program.
    _program._logger.SetLogHeader("CodeInSE is easy\n================");
    _program._logger.AppendToLogBody("Hello Space!");
    // Utilize saved data.
    if (!String.IsNullOrEmpty(_lastTimeRan))
    {
        _program._logger.AppendToLogBody($"Detected a save. Last time program was ran was: {_lastTimeRan}");
    }
    // Utilize configured data.
    _program.Me.CustomName = _customName;
    _program._logger.AppendToLogBody($"Set PB's custom name to: {_customName}");
}
```
#### In Closing
That is all you need to know to start programming in SE. Of course this script is not very useful, but it is a gateway into a world of efficient and automated systems in SE! Below we will explore the framework in more detail, so keep reading if you like deets and the nitty gritty (or just want to know more about advanced usage)!
