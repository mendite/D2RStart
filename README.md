# D2RStart
D2RStart is a tool for easy starting multible installations of D2R. For technical reasons, the tool only launches the launcher, not the actual game!
The tool does the same in the background as the description for command line at the following URL - reduced to one button click. You don't need to worry about which path is already in use.
https://us.forums.blizzard.com/en/d2r/t/how-to-multiple-d2r-instances-requires-two-accounts/60546

## Short Help
- A separate license/B-Net account is required for each D2R game instance launched
- For each D2R instance a complete copy of the game installation directory is required (you find it in D2R game settings of BNet launcher)
- Start the tool D2RStart.exe
- Download Handle at https://learn.microsoft.com/en-us/sysinternals/downloads/handle 
- Extract the executables from the downloaded zip archive to the D2RStart.exe directory
- Restart D2RStart.exe
- Create a directory entry for each startable game instance (minimum two)
- Save settings
- Start BNet launcher
- Open settings
- Go to category App
- Change "On Game Launch2 to "Exit Battle.net completely"
- Enable "Allow multible instances of Battle.net"
- For every instance you want to run click in tool D2RStart button "Start D2R-Launcher", login with wanted account and start D2R game (the launcher must close afterwards so that the next instance can be opened)
- In order to avoid jerking when several instances are started at the same time, I have turned on retro graphics for all but one