open System.Diagnostics
open System.IO

let InvalidChars = seq{ ':'; '?'; '|'; '\\'; '/'; '*'; '<'; '>'; '^'; '\"' }

let rec rename (fileOrDirectory: FileSystemInfo): unit =
    let isDirectory = match fileOrDirectory.Attributes with | FileAttributes.Directory -> true | _ -> false
    let mutable newName = fileOrDirectory.Name

    for invalidChar in InvalidChars do
        newName <- newName.Replace(invalidChar, '_')

    let parentDirectory = Path.GetDirectoryName fileOrDirectory.FullName
    let mutable uniqueFullPath = Path.Combine(parentDirectory, newName)
    let renameAttempt = ref 0

    if not (newName.Equals fileOrDirectory.Name) then
        // Attempt to fix file name collisions.
        // This won't fix names with same name but different casing (a problem on Windows due to case insensitivity),
        // but the only way to fix that is to enumerate all files, which is likely too expensive.
        while File.Exists uniqueFullPath && renameAttempt < ref 10 do
            uniqueFullPath <- Path.Combine(parentDirectory, $"{renameAttempt.Value}{newName}")
            incr renameAttempt

        if isDirectory then
            Directory.Move(fileOrDirectory.FullName, uniqueFullPath)
        else
            File.Move(fileOrDirectory.FullName, uniqueFullPath)
        printfn $"Rewrote [%s{fileOrDirectory.FullName}] as [%s{uniqueFullPath}]"

    if isDirectory then
        for child: string in Directory.GetFileSystemEntries uniqueFullPath do
            rename(DirectoryInfo(child))

[<EntryPoint>]
let main argv =
    let sw = Stopwatch.StartNew()
    
    let directory = DirectoryInfo(argv.[0])
    if not directory.Exists then
        printfn $"Directory [%s{directory.FullName}] does not exist."
        exit 1
    
    rename (directory)
    sw.Stop |> ignore

    printfn $"Done in %d{sw.ElapsedMilliseconds}ms"
    0