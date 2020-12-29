# L'zack Laser CNC

The goal for this project is to create a firmware for CNC machines as a final project, required for my school graduation. The ,a

------
- [L'zack Laser CNC](#lzack-laser-cnc)
  - [Projects](#projects)
  - [Documentation](#documentation)
  - [Work in progress](#work-in-progress)
  - [Glossary](#glossary)
  - [FTS Emulator](#fts-emulator)
    - [What is it ?](#what-is-it-)
  - [L'Zack Host](#lzack-host)
    - [What is it ?](#what-is-it--1)
  - [L'Zack Firmware](#lzack-firmware)
    - [What is it ?](#what-is-it--2)

## Projects

| Project         | Description           | Status  |
|-----------------|-----------------------|---------|
| docs            | Documentation website | [None]  |
| [FTS Emulator](https://github.com/RafaelEstevamReis/lzak/tree/main/FTSEmulator) | ` Fast Testing Suite` _Emulator_ | [None]  |
| [L'Zack Host](https://github.com/RafaelEstevamReis/lzak/tree/main/LZackHost)     | CNC Host software     | [None]  |
| [L'Zack Firmware](https://github.com/RafaelEstevamReis/lzak/tree/main/LZackFirmware) | CNC firmware          | [None]  |


## Documentation

The solution hence put forth is a CNC software suite. It aims to manipulate a CNC machine via GCODE¹ specifications.

It is a three-piece *software*, namely, 1) the firmware itself for arduinos, 2) the Fast Testing (FTS) Suite, an CNC hardware emulator for easy testing and a 3) multi-platform .NET host software, which will be responsible for sending commands to the firmware.

It'll be ready right when it'll be ready.


¹ GCODE descriptions: https://en.wikipedia.org/wiki/G-code

## Work in progress

This piece of software is an university Final Project and is not production ready.

At this time any collaboration is limited

## Glossary

* `CNC`: Originally, *Computer-Generated Code*; currently it refers to machines capable of receiving commands from a computer to manufacture physical objects through automated machines.
* `GCODE`: Accronym for *Geometric Code*; also known as RS-274), it's a set of instructions widely used as CNC programming language. Basically, a GCODE instructions script contains positional commands which the firmware can read and instruct the machine to reposition itself.
* `Firmware`: it's a low-level computer software that works by manipulating instructions for a specific hardware in order to provide a standard operating environment. In some cases, it works as a basic software to which all subsequent software refer to; in others - as in our case - it'll work as the complete operating system.
* `Host`: will do this one later
* `mcco`: its a small computer, 
* ...

## FTS Emulator

### What is it ?

The Fast Testing Suite will work as a CNC hardware emulator. The objective is to provide a computer solution that enables an easy-to-use, easy-to-test environment by mimicking a three-axis CNC machine, including the stepper motor hardware controller, the limit switches and all the needed features. This will allow for development even while the machine is not physically available; also, avoids hardware damage in case of in-development bugs.


## L'Zack Host

### What is it ?

Host description

## L'Zack Firmware

### What is it ?

Firmware description



