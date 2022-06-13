# FileMover App
FileMoverApp provides functionality to move all files from one folder to another in parallel async manner.

- files are put in queue and proccesed in background.
- the files are grouped by extensions/type - pdf, jpg, txt...etc. 
- groups are processed in parallel.
- files in each group are processed sequentially.

# Running
Load solution and run FileMoverApp project

# Commands
- move -s '{srcPath}' -d '{destPath}' 
  {srcPath} - source folder - ./src, c:/temp/src ... etc
  {destPath} - source dest - ./dest, c:/temp/dest ... etc
  
- exit



