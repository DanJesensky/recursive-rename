# recursive-rename

This is a small script I put together to help someone recover files from an ext4
file system when all they have now is a Windows machine (NTFS file system, which
disallows several characters).

## What it does and doesn't do

The script will:
* Recursively evaluate each file in the directory for invalid NTFS file names and replace any invalid characters with an underscore
* Change any files whose names would be duplicated by invalid character replacement to include additional underscores (up to 10).

What it will NOT do:
* Back up your data for you. Do this before you attempt usage.
* Check for files whose names have been duplicated due to casing.

## Usage
**Make sure you have backed up your data before using it.**

With the dotnet 5.0 SDK installed, replace `<path>` below with the target path and
run this command from the project root:

```
dotnet run --project RecursiveRename/RecursiveRename.fsproj "<path>"
```

If you get an error, something went wrong. If this happens, make sure the command above
is correct, and that your provided path exists.

Otherwise, you will get informational messages saying what files (if any) were renamed,
and how long it took to complete.