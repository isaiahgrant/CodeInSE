# Code in SE Framework
This document is written assuming that you have read the [README](README.md).

This serves as formal documentation for the framework.

The framework is made of three major components:
* [Modules](#Modules)
* [Base classes](#Base-Classes)
* [Implementation](#Implementation)

# Modules
A base set of functionality that comprises most of the framework. All modules are members of the Program class and as such are quickly accessible and managed.

Modules are listed in the order that they are initialized as they may
have dependencies on one another.

Refer to [Program](CodeInSE/Program.cs) for more information.

## Grid Module
Grid is a simple interface for the [GridTerminalSystem](https://github.com/malware-dev/MDK-SE/wiki/The-Grid-Terminal-System)

It features single function access to any blocks available to the PB.
Blocks can be acquired by block name, block group, and block type.

[reference](CodeInSE/Grid.cs)

## Logger Module
Logger is a module that manages the output of your program. Logger
interfaces with both the PB's output terminal (Echo) as well as
pre-configured LCDs.

Logger includes a static header as well as a body that can be
configured to behave as desired. For example you can choose to append logs 
vs. clearing logs each run.

You can also set a log level with each output. This allows you to output
logs to various audiences without having to change your code. This is
achieved through 5 log levels (SYSTEM, DEBUG, INFO, WARNING, and ERROR).
By setting the desired log level when initializing the Logger you can
effectively filter outputs to only see what you care about (and your users
can too!).

To interface a LCD with the logger simply add the LCD to a group named
after your Logger._globalIdentifier or add your Logger._globalIdentifier
to the name of the LCD.

[reference](CodeInSE/Logger.cs)

## Disk Module
The disk module interfaces with the Program's persistent Storage member.
Storage allows you to persist data across runs of your script, for example
when the game is saved and reloaded. CustomProgram is a child class of
Serializable and as such implements the LoadFields and SaveToFields
methods. These methods allow you to save/load any data (fields) you wish
to obtain the next time the script is loaded.

You can also read from/write to Disk independent of the save/load
mechanism.

[reference](CodeInSE/Disk.cs)

## Configuration Module
The configuration module interfaces with the PB's CustomData (much in the 
same way as the Disk Module does with Storage). The difference here is that
CustomData is more accessible to users and should really only be used for
configuration of your script.

Configuration of your script gives you (and users) the ability to change
the functionality of your script without changing the code driving the
behavior. This can be utilized for custom timing, configuring which block
to retrieve, etc.

[reference](CodeInSE/Configuration.cs)

## Command Module
Arguments are the string passed to your program when it is ran by a user.
Commands are arguments that the script expects and has a logic handle for
(code to execute based on the command). The command module interprets
arguments and resolves a command and it's parameters. Through this method
the CustomProgram simply references the handle associated to the Command
resolved.

Command is also saved on Program save so that a script can continue
execution of the last known command once loaded again.

[reference](CodeInSE/Command.cs)

## Timer Module
Somewhat recently SE built in the ability for scripts to execute themselves
without the need for a Timer block. The timer module is in place to give
you the convenience of precision timing and self-throttling to give your
script the exact timing it needs while prevent your script from weighing
down the games performance.

The timer module has default configuration but can be managed by users so
that they can customize their preference. As a developer, you do not have
to code around timing but rather simply set your script to self-looping
and allow configuration to dictate the timing (though I do recommend
defaults to be sensible values based on the script).

[reference](CodeInSE/Timer.cs)

# Base/Utility Classes
Base classes refers more explicitly to Serializable and CustomProgram. These two classes allow whatever implementation you utilize inherit the function and utility of the framework. Serializable allows you to quickly and easily save and load your script, while CustomProgram is a base class for your script (much like Program is without the framework).

# Implementation
This is where your script/code will live. By implementing your solution in this hierarchy you can maximize the framework.
