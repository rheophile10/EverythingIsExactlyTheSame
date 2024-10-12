namespace Metadata

module Env =
    
    open System
    open System.IO

    let readEnvFile filePath =
        if File.Exists filePath then
            File.ReadAllLines(filePath)
            |> Array.choose (fun line -> 
                let parts = line.Split('=')
                if parts.Length = 2 then Some(parts.[0].Trim(), parts.[1].Trim()) else None)
            |> Map.ofArray
        else
            Map.empty

    let getConnectionString (connectionStringEnvKey:string): string =
        let envFilePath = ".env"
        envFilePath
        |> readEnvFile
        |> Map.tryFind connectionStringEnvKey
        |> function
            | Some connString -> connString
            | None -> failwithf "Connection string with key %s not found." connectionStringEnvKey