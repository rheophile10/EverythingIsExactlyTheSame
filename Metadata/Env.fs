namespace Metadata

module Env =
    
    open DotNetEnv

    let getConnectionString (connectionStringEnvKey:string): string =
        DotNetEnv.Env.Load() |> ignore
        let connString = DotNetEnv.Env.GetString(connectionStringEnvKey)
        connString