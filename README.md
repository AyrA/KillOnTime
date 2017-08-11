# KillOnTime
Executes a program and kills it if it still runs after a set amount of seconds

## Usage

    KillOnTime <time> <exe> [args]

Runs a process for a maximum given amount of time.
This will kill the process if it does not exits on time.
Any children of the killed process will be left running.

- **time**: required, maximum time in seconds to wait
- **exe**: required, exe file to start
- **args**: optional, arguments for the executable

Use `/?` to get more help,especially regarding the exit codes.

## Finding exe files

The application will try to find the given executable in the current directory and then in every directory from the `PATH` environment variable.
This means that if you can run the application from everywhere by just typing the name,
this tool will find it too. The only difference is that you have to specify the extension.
This means that you should use `xyz.exe` and not just `xyz`

## Executing Console Applications

If you execute a console application it will not open a new console window but instead use the current window.
At the time of writing there is no argument to change that. You can change `UseShellExecute=true` to `false` if you need it this way.
