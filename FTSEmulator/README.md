# Emulator

This project will be a CNC emulator written in C# for GCode parsing test and general suite development as software (usually) don't catch fire

------
- [Emulator](#emulator)
  - [Documentation](#documentation)
  - [Why it exists ?](#why-it-exists-)
  - [How to use ?](#how-to-use-)
  - [What it can do ?](#what-it-can-do-)
  - [What is the hardware layout being emulated ?](#what-is-the-hardware-layout-being-emulated-)
  - [What (GCode) commands it supports ?](#what-gcode-commands-it-supports-)

## Documentation

Documentation intro

## Why it exists ?

I need it to learn how to build an firmware without exploding a real CNC machine nor burning my house

## How to use ?

Press F5 and learn for yourself

## What it can do ?

I hpe it:
* Have a minimally nice _(usable)_ `console` UI
  * Move head
  * Start laser
  * Stop laser
  * Pulse laser
  * Software Stop
  * Emergency Stop (simulate mcco reset)
* Receive via TCP or Serial some GCode commands
* Show where the hardware stuff is
  * Head position (mm)
  * Motor position (steps)
  * Laser status

## What is the hardware layout being emulated ?

This emulator mimics an 2.5D CNC machine, it is a 2D movement plane plus a third binary dimension for Laser On/Off

The movement plane are called X and Y

Each axis have two tri-state endstops and should trigger a FAILURE/Emergency if:
* Any loose signal
* Try to move beyond it's position

The Laser is called "tool"


## What (GCode) commands it supports ?

(Each command should have a link to the documentation page explaining how it works)

* Cmd 1: aaa
* Cmd 2: bbb
* Cmd 3: ccc
* Cmd 4: D:
