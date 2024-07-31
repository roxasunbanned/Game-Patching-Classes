# Game Patching Classes

## Description

This project provides classes i've built over time for game patching, including `gameapi` and `codecave`. These classes help in modifying game behavior by interacting with the game's memory and code.

This is currently in use on my [PSO-Map-Seed-Patcher.](https://github.com/roxasunbanned/PSO-Map-Seed-Patcher)

## Installation

1. Clone the repository:
    ```sh
    git clone https://github.com/roxasunbanned/game-patching-classes.git
    ```
2. Navigate to the project directory:
    ```sh
    cd game-patching-classes
    ```
3. Build the project using your preferred method (e.g., Visual Studio, `dotnet build`).
4. Import the classes to your project

## Usage

### GameAPI

The `gameapi` class provides methods to interact with the game's API, allowing you to read and write memory, call functions, create jumps and more.

### CodeCave

The `codecave` class allows you to create code caves, which are sections of memory where you can inject custom code to modify game behavior.

## Classes

### GameAPI

#### Methods

- `ReadMemory`: Reads memory from the game process.
- `WriteMemory`: Writes memory to the game process.
- `CreateCodeCave`: Creates a code cave at a specified location.
- `CreateJump`: Creates a Jump instruction from and to specific memory locations.
- `CalculateJump`: Calculates how many bytes required to jump from origin to target location.
- `ScanMemory`: Scans memory for a specific byte string (AOB Scan).
- + Various other read/write methods for specific data types and conversions.

### CodeCave

#### Methods

- `doesExist`: Checks if the Code Cave has been allocated.

## Examples

### Using GameAPI

```csharp
using game;
gameapi game = new gameapi("process_name")
IntPtr readWriteAddr = 0x333080;
byte[] writeData = new byte[4]
{
    0x01, 0x02, 0x03, 0x04
}
game.ReadBytes((int)readWriteAddr, 4);
game.WriteBytes((int)readWriteAddr, writeData);


codecave firstCodeCave = new codecave();
IntPtr targetAddress = 0x171881;
int nopLength = 8;
byte[] injectedBytes = new byte[12]
{
    0x81, 0xFF, 0x80, 0x30, 0x73, 0x00,
    0x74, 0x75, 0x0F, 0x1F, 0x40, 0x00 
};
nameCodeCave = pso.createCodeCave(targetAddress, nopLength, firstCodeCave, injectedBytes);
```