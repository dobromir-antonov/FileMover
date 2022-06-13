# FileMover
FileMover provides functionality to move files from one folder to another in parallel async manner.

- files are put in a queue and processed in background.
- the files are grouped by extension/type - pdf, jpg, txt...etc. 
- groups are processed in parallel.
- files in each group are processed sequentially.

# Running
run FileMover.App project

# Commands
- move -s '{srcPath}' -d '{destPath}' 
  {srcPath} - source folder - ./src, c:/temp/src ... etc
  {destPath} - source dest - ./dest, c:/temp/dest ... etc
  
- exit



